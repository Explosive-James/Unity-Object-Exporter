using ObjectExporter.Exporting.Formatting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace ObjectExporter.Exporting
{
    /// <summary>
    /// A facade to convert a mesh to the .obj and .mtl format.
    /// </summary>
    // ToDo: Prevent multiple material names?
    public sealed class ObjExporter : IDisposable
    {
        #region Data
        private readonly TextStream _meshStream;
        private readonly TextStream _materialStream;

        private readonly List<Material> _writtenMaterials;
        private readonly Dictionary<object, string> _uniqueNames;

        private int _totalVertexCount;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the Obj Exporter.
        /// </summary>
        /// <param name="meshStream">Stream to write the .obj file data to.</param>
        /// <param name="materialStream">Stream to write the .mtl file data to.</param>
        /// <param name="materialLibraryName">The file name (no file extension included) of the .mtl file.</param>
        public ObjExporter(Stream meshStream, Stream materialStream, string materialLibraryName)
        {
            _meshStream = new TextStream(meshStream);
            _materialStream = new TextStream(materialStream);

            if (materialLibraryName != null) {
                _meshStream.Write($"mtllib {materialLibraryName}.mtl\n");
            }

            _writtenMaterials = new List<Material>();
            _uniqueNames = new Dictionary<object, string>();
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Converts the mesh and material data into the obj format.
        /// </summary>
        /// <param name="mesh">Mesh to convert to obj.</param>
        /// <param name="materials">Materials used by the mesh and stored in the mtl file.</param>
        public void ExportMesh(Mesh mesh, params Material[] materials)
        {
            foreach (Material material in materials)
                if (!_writtenMaterials.Contains(material)) {

                    WriteMaterial(material);
                    _writtenMaterials.Add(material);
                }

            /* This defines a new mesh object with the name of the mesh.*/
            _meshStream.Write($"g {GetUniqueName(mesh, mesh.name)}\n");

            /* Writing the vertices, normals and uv coordinates.*/
            foreach (Vector3 vertex in mesh.vertices)
                _meshStream.Write($"v {ObjString.Stringify(vertex)}\n");
            foreach (Vector3 normal in mesh.normals)
                _meshStream.Write($"vn {ObjString.Stringify(normal)}\n");
            foreach (Vector2 uvCoord in mesh.uv)
                _meshStream.Write($"vt {ObjString.Stringify(uvCoord)}\n");

            for (int submeshIndex = 0; submeshIndex < mesh.subMeshCount; submeshIndex++) {

                SubMeshDescriptor submesh = mesh.GetSubMesh(submeshIndex);

                /* The submesh ideally needs to reference the material found in the .mtl file,
                 * however it is not nessersary, Unity might import this incorrectly though.*/
                // ToDo: Check we dson't need a dud materials
                string materialName = $"Material_{submeshIndex}";
                if(submeshIndex < materials.Length) {
                    materialName = GetUniqueName(materials[submeshIndex], materials[submeshIndex].name);
                }

                _meshStream.Write($"usemtl {materialName}");

                for (int triangle = 0; triangle < submesh.indexCount; triangle++) {

                    /* Every third index creates a new triangle and needs a new line.*/
                    if (triangle % 3 == 0) _meshStream.Write("\nf ");

                    /* Because the coordinate system is flipped (left-handed vs right-handed) the triangles
                     * need to be written in reverse order so the faces are facing the correct direction.*/
                    int triangleIndex = submesh.indexStart + submesh.indexCount - triangle - 1;
                    /* The vertex index of the triangle, plus one because obj use one-based arrays.*/
                    int vertexIndex = mesh.triangles[triangleIndex] + 1 + _totalVertexCount;

                    string coordText = vertexIndex <= mesh.uv.Length ? vertexIndex.ToString() : string.Empty;
                    string normText = vertexIndex <= mesh.normals.Length ? vertexIndex.ToString() : string.Empty;

                    _meshStream.Write($"{vertexIndex}/{coordText}/{normText} ");
                }

                _meshStream.Write("\n");
            }

            _totalVertexCount += mesh.vertexCount;
        }
        /// <summary>
        /// Disposes the mesh and material streams.
        /// </summary>
        public void Dispose()
        {
            _meshStream.Dispose();
            _materialStream.Dispose();
        }
        #endregion

        #region Private Functions
        private void WriteMaterial(Material material)
        {
            Color materialColor = Color.white;

            if (material.HasColor("_Color")) {
                materialColor = material.color;
            }

            string materialName = GetUniqueName(material, material.name);

            _materialStream.Write($"newmtl {materialName}\n" +
                $"ka {ObjString.Stringify(materialColor)}\n" +
                $"kd {ObjString.Stringify(materialColor)}\n");

            /* renderQueue 2000 is the opaque pass value.*/
            if (material.renderQueue != 2000) {

                _materialStream.Write(
                    $"d {material.color.a:0.00}\n" +
                    $"illum 4\n");
            }

            _materialStream.Write("\n");
        }

        private string GetUniqueName(object reference, string originalName)
        {
            if (_uniqueNames.ContainsKey(reference)) {

                return _uniqueNames[reference];
            }
            if (!_uniqueNames.Values.Contains(originalName)) {

                _uniqueNames.Add(reference, originalName);
                return originalName;
            }

            for (int i = 1; ; i++) {

                string targetName = $"{originalName}_{i}";

                if (!_uniqueNames.Values.Contains(targetName)) {

                    _uniqueNames.Add(reference, targetName);
                    return targetName;
                }
            }
        }
        #endregion
    }
}

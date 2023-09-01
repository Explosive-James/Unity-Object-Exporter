using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ObjectExporter.MeshCombining.DataTypes
{
    /// <summary>
    /// Expandable container for the mesh data.
    /// </summary>
    public class PrototypeMesh
    {
        #region Properties
        public List<Vector3> Vertices { get; private set; }
        public List<Vector3> Normals { get; private set; }

        public List<int> Triangles { get; private set; }
        public List<Vector2> UVCoords { get; private set; }

        public List<SubMeshDescriptor> SubMeshes { get; private set; }
        #endregion

        #region Constructor
        public PrototypeMesh(int initialCapacity)
        {
            Vertices = new List<Vector3>(initialCapacity);
            Normals = new List<Vector3>(initialCapacity);

            Triangles = new List<int>(initialCapacity * 3);
            UVCoords = new List<Vector2>(initialCapacity);

            SubMeshes = new List<SubMeshDescriptor>();
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Converts the PrototypeMesh into a UnityEngine.Mesh.
        /// </summary>
        /// <param name="meshName">The name of the mesh.</param>
        public Mesh ConvertToMesh(string meshName)
        {
            meshName ??= string.Empty;

            Mesh result = new Mesh() {

                vertices = Vertices.ToArray(),
                normals = Normals.ToArray(),

                triangles = Triangles.ToArray(),
                uv = UVCoords.ToArray(),

                name = meshName,
                subMeshCount = SubMeshes.Count,
            };

            result.SetSubMeshes(SubMeshes);
            return result;
        }
        #endregion
    }
}

using ObjectExporter.MeshCombining.DataTypes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace ObjectExporter.MeshCombining
{
    /// <summary>
    /// Combines meshes, by materials if possible, into a single mesh.
    /// </summary>
    public sealed class MeshCombiner
    {
        #region Data
        /// <summary>
        /// Tracks how many vertices will be in the final mesh.
        /// </summary>
        private int _combinedVertexCount;

        /// <summary>
        /// Materials along withthe submeshes that use them.
        /// </summary>
        private readonly Dictionary<Material, List<SubMeshInfo>> _submeshGraph = new();
        /// <summary>
        /// Submeshes that aren't using materials and cannot be merged.
        /// </summary>
        private readonly List<SubMeshInfo> _submeshes = new();
        #endregion

        #region Properties
        /// <summary>
        /// Gets all the unique meshes in the combiner.
        /// </summary>
        public Material[] GetUniqueMaterials => _submeshGraph.Keys.ToArray();
        #endregion

        #region Public Functions
        /// <summary>
        /// Inserts a mesh to be combined with any other mesh in the combiner.
        /// </summary>
        /// <param name="mesh">Mesh to insert.</param>
        /// <param name="materials">The shared materials used by the mesh.</param>
        public void InsertMesh(Mesh mesh, params Material[] materials)
        {
            for (int i = 0; i < mesh.subMeshCount; i++) {

                if (materials?.Length <= i) {

                    _submeshes.Add(new SubMeshInfo(mesh, i));
                    continue;
                }

                _submeshGraph.TryAdd(materials[i], new List<SubMeshInfo>());
                _submeshGraph[materials[i]].Add(new SubMeshInfo(mesh, i));
            }

            _combinedVertexCount += mesh.vertexCount;
        }

        /// <summary>
        /// Combines all meshes that were inserted into the mesh combiner.
        /// </summary>
        /// <param name="meshName">The final name of the outputted mesh.</param>
        public Mesh CombineInsertedMeshes(string meshName)
        {
            /* When the vertices are added to the prototype mesh they are in a different order from their original 
             * mesh indices, the int[] maps their original index positions with their new position in the mesh.
             * When the triangles reference an index we check the remapped values to get the new, correct result.*/
            Dictionary<Mesh, int[]> vertexRemapper = new Dictionary<Mesh, int[]>();
            PrototypeMesh prototypeMesh = new PrototypeMesh(_combinedVertexCount);

            foreach (Material material in _submeshGraph.Keys) {

                /* The start of the submesh that uses this one material.*/
                int submeshStart = prototypeMesh.Triangles.Count;

                foreach (SubMeshInfo submesh in _submeshGraph[material]) {
                    WriteSubmesh(submesh, vertexRemapper, prototypeMesh);
                }

                prototypeMesh.SubMeshes.Add(new SubMeshDescriptor(submeshStart,
                    prototypeMesh.Triangles.Count - submeshStart));
            }

            foreach(SubMeshInfo submesh in _submeshes) {

                int submeshStart = prototypeMesh.Triangles.Count;
                WriteSubmesh(submesh, vertexRemapper, prototypeMesh);

                prototypeMesh.SubMeshes.Add(new SubMeshDescriptor(submeshStart,
                    prototypeMesh.Triangles.Count - submeshStart));
            }

            return prototypeMesh.ConvertToMesh(meshName);
        }
        #endregion

        #region Private Functions
        private void WriteSubmesh(SubMeshInfo submesh, Dictionary<Mesh, int[]> remapper, PrototypeMesh mesh)
        {
            /* Ensuring that the remapped array exists that's as long as the mesh itself.*/
            remapper.TryAdd(submesh.mesh, new int[submesh.mesh.vertexCount]);

            for (int i = 0; i < submesh.description.vertexCount; i++) {

                int vertexIndex = i + submesh.description.firstVertex;
                remapper[submesh.mesh][vertexIndex] = mesh.Vertices.Count;

                mesh.Vertices.Add(submesh.mesh.vertices[vertexIndex]);

                /* Adding a default normal value if the mesh is missing one.*/
                mesh.Normals.Add(Vector3.zero);
                if (submesh.mesh.normals.Length > vertexIndex) {
                    mesh.Normals[^1] = submesh.mesh.normals[vertexIndex];
                }
                /* Adding a default uv coordinate if the mesh is missing one.*/
                mesh.UVCoords.Add(Vector2.zero);
                if (submesh.mesh.uv.Length > vertexIndex) {
                    mesh.UVCoords[^1] = submesh.mesh.uv[vertexIndex];
                }
            }
            for (int i = 0; i < submesh.description.indexCount; i++) {

                mesh.Triangles.Add(remapper[submesh.mesh]
                    [submesh.mesh.triangles[i + submesh.description.indexStart]]);
            }
        }
        #endregion
    }
}

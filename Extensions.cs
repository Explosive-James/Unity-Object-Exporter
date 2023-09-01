using System.Collections.Generic;
using UnityEngine;

namespace ObjectExporter
{
    public static class Extensions
    {
        #region Public Functions
        /// <summary>
        /// Multiplies the verices and normals of a mesh by the Matrix4x4.
        /// </summary>
        public static Mesh MultiplyPoint(this Matrix4x4 matrix, Mesh mesh)
        {
            Vector3[] vertices = new Vector3[mesh.vertexCount];
            Vector3[] normals = new Vector3[mesh.normals.Length];

            for (int i = 0; i < vertices.Length; i++) {
                vertices[i] = matrix.MultiplyPoint(mesh.vertices[i]);
            }
            for (int i = 0; i < normals.Length; i++) {
                normals[i] = matrix.MultiplyVector(mesh.normals[i]);
            }

            Mesh results = Object.Instantiate(mesh);
            results.name = mesh.name;

            results.vertices = vertices;
            results.normals = normals;

            results.RecalculateBounds();
            return results;
        }

        /// <summary>
        /// Gets all children attached to the transform including children of children.
        /// </summary>
        public static IEnumerable<Transform> GetChildrenRecursive(this Transform root)
        {
            foreach (Transform child in root) {

                yield return child;

                foreach (Transform grandchild in child.GetChildrenRecursive()) {
                    yield return grandchild;
                }
            }
        }
        #endregion
    }
}

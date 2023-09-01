using UnityEngine;
using UnityEngine.Rendering;

namespace ObjectExporter.MeshCombining.DataTypes
{
    /// <summary>
    /// Container that stores a submesh with the mesh data.
    /// </summary>
    public readonly struct SubMeshInfo
    {
        #region Data
        public readonly Mesh mesh;
        public readonly SubMeshDescriptor description;
        #endregion

        #region Constructor
        public SubMeshInfo(Mesh mesh, int submeshIndex)
        {
            this.mesh = mesh;
            description = mesh.GetSubMesh(submeshIndex);
        }
        public SubMeshInfo(Mesh mesh, SubMeshDescriptor description)
        {
            this.mesh = mesh;
            this.description = description;
        }
        #endregion
    }
}

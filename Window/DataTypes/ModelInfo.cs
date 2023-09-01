using UnityEngine;

namespace ObjectExporter.Window.DataTypes
{
    /// <summary>
    /// Stores the mesh along with the materials it uses.
    /// </summary>
    public struct ModelInfo
    {
        #region Data
        public Mesh mesh;
        public Material[] materials;
        #endregion

        #region Constructor
        public ModelInfo(Mesh mesh, Material[] materials)
        {
            this.mesh = mesh;
            this.materials = materials;
        }
        public ModelInfo(GameObject gameObject)
        {
            mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            materials = new Material[0];

            if (gameObject.TryGetComponent(out MeshRenderer renderer)) {
                materials = renderer.sharedMaterials;
            }
        }
        #endregion
    }
}

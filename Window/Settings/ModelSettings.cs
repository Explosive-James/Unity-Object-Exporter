using ObjectExporter.MeshCombining;
using ObjectExporter.Window.DataTypes;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectExporter.Window.Settings
{
    /// <summary>
    /// Defines how to modify meshes before they are ready to be exported.
    /// </summary>
    internal sealed class ModelSettings : SettingsObject
    {
        #region Data
        /// <summary>
        /// Should the meshes be merged into one single mesh.
        /// </summary>
        public bool combineMeshes = true;
        /// <summary>
        /// Should the normals be recalculated.
        /// </summary>
        public bool recalculateNormals = true;

        [Space]
        /// <summary>
        /// Should the mesh have the root transform applied to it.
        /// </summary>
        public bool applyRootTransform = false;
        #endregion

        #region Public Functions
        /// <summary>
        /// Updates the meshes based on the settings, combining the meshes into one or applying the root transform.
        /// </summary>
        public ModelInfo[] UpdateModels(IEnumerable<ModelInfo> modelInfos, Transform rootTransform, string modelName)
        {
            List<ModelInfo> outputModels = new List<ModelInfo>(modelInfos);

            if (!applyRootTransform)
                for (int i = 0; i < outputModels.Count; i++) {

                    outputModels[i] = new ModelInfo(rootTransform.worldToLocalMatrix.
                        MultiplyPoint(outputModels[i].mesh), outputModels[i].materials);
                }

            if (combineMeshes) {

                MeshCombiner combiner = new MeshCombiner();

                foreach (ModelInfo modelInfo in outputModels) {
                    combiner.InsertMesh(modelInfo.mesh, modelInfo.materials);
                }

                outputModels = new List<ModelInfo>() {
                    new  ModelInfo(
                        combiner.CombineInsertedMeshes(modelName), 
                        combiner.GetUniqueMaterials)
                };
            }

            if (recalculateNormals)
                for(int i = 0; i < outputModels.Count; i++) {
                    outputModels[i].mesh.RecalculateNormals();
                }

            return outputModels.ToArray();
        }

        public override bool IsValid() => true;
        #endregion
    }
}

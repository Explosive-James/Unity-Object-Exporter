using ObjectExporter.Window.DataTypes;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectExporter.Window.Settings
{
    /// <summary>
    /// Defines where the meshes come from that get exported.
    /// </summary>
    internal sealed class TransformSettings : SettingsObject
    {
        #region Enums
        public enum Targets { AllObjects, RootOnly, ChildOnly }
        #endregion

        #region Data
        /// <summary>
        /// The hierarchy to grab meshes and materials from.
        /// </summary>
        [Header("Transform Settings")]
        public Transform rootObject;
        /// <summary>
        /// What part of the hierarchy from to meshes and materials from.
        /// </summary>
        public Targets modelTargets;
        #endregion

        #region Public Functions
        /// <summary>
        /// Gets all the meshes and models from the selected transforms.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModelInfo> GetModelInformation()
        {
            if (modelTargets == Targets.RootOnly || modelTargets == Targets.AllObjects)
                if (TryGetModelInformation(rootObject, out ModelInfo modelInfo)) {

                    yield return modelInfo;
                }
            if (modelTargets == Targets.ChildOnly || modelTargets == Targets.AllObjects)
                foreach (Transform child in rootObject.GetChildrenRecursive())
                    if (TryGetModelInformation(child, out ModelInfo modelInfo)) {

                        yield return modelInfo;
                    }
        }

        public override bool IsValid() => rootObject != null;
        #endregion

        #region Private Functions
        private bool TryGetModelInformation(Transform transform, out ModelInfo modelInfo)
        {
            modelInfo = new ModelInfo();

            if (transform.TryGetComponent(out MeshFilter filter) && filter.sharedMesh != null) {
                 modelInfo.mesh = transform.localToWorldMatrix.MultiplyPoint(filter.sharedMesh);
            }
            if (transform.TryGetComponent(out MeshRenderer renderer)) {
                modelInfo.materials = renderer.sharedMaterials;
            }

            return modelInfo.mesh != null;
        }
        #endregion
    }
}

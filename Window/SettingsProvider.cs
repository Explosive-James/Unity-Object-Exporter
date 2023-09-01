using ObjectExporter.Window.Settings;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ObjectExporter.Window
{
    /// <summary>
    /// Manages the different settings found in the export window.
    /// </summary>
    internal sealed class SettingsProvider
    {
        #region Data
        private readonly SettingsObject[] _settingObjs;
        #endregion

        #region Properties
        /// <summary>
        /// Transform settings define what transforms to grab meshes and materials from.
        /// </summary>
        public TransformSettings TransformSettings { get; private set; }
        /// <summary>
        /// Model settings is how to modify the meshes to be exported.
        /// </summary>
        public ModelSettings ModelSettings { get; private set; }
        /// <summary>
        /// export settings define where the files are and what they're called.
        /// </summary>
        public ExportSettings ExportSettings { get; private set; }
        #endregion

        #region Constructor
        public SettingsProvider()
        {
            TransformSettings = ScriptableObject.CreateInstance<TransformSettings>();
            ModelSettings = ScriptableObject.CreateInstance<ModelSettings>();
            ExportSettings = ScriptableObject.CreateInstance<ExportSettings>();

            _settingObjs = new SettingsObject[] { TransformSettings, ModelSettings, ExportSettings };
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Do all the setting objects have valid information.
        /// </summary>
        public bool AreSettingsValid()
        {
            for(int i = 0; i < _settingObjs.Length; i++)
                if (!_settingObjs[i].IsValid()) {
                    return false;
                }

            return true;
        }

        /// <summary>
        /// Returns the serialized objects of the settings.
        /// </summary>
        public IEnumerable<SerializedObject> GetSettingsObjects()
        {
            for(int i = 0; i < _settingObjs.Length; i++) {
                yield return _settingObjs[i].SerializedObject;
            }
        }
        #endregion
    }
}

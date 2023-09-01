using ObjectExporter.Exporting;
using ObjectExporter.Window.DataTypes;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ObjectExporter.Window
{
    internal sealed class ModelExporterWindow : EditorWindow
    {
        #region Data
        private SettingsProvider _settingsProvider;
        #endregion

        #region Unity Functions
        [MenuItem("Window/General/Object Exporter")]
        static void CreateWindow() => GetWindow<ModelExporterWindow>("Object Exporter");

        private void OnGUI()
        {
            /* Creating the settings if they don't already exist.*/
            _settingsProvider ??= new SettingsProvider();

            foreach (SerializedObject settingsObject in _settingsProvider.GetSettingsObjects()) {

                /* The iterator gives every serializable property inside the settings object.*/
                SerializedProperty properties = settingsObject.GetIterator();
                EditorGUILayout.Space();

                /* This draws the properties in the GUI, the first step needs to be skipped over.*/
                if (properties.NextVisible(true))
                    while (properties.NextVisible(false)) {
                        EditorGUILayout.PropertyField(properties);
                    }

                /* If this object has been modified it needs to update the underlying data object.*/
                if (settingsObject.hasModifiedProperties) {
                    settingsObject.ApplyModifiedProperties();
                }
            }

            /* If data hasn't been assigned or assigned correctly the button needs to be disabled.*/
            EditorGUI.BeginDisabledGroup(!_settingsProvider.AreSettingsValid());
            EditorGUILayout.Space();

            if (GUILayout.Button("Export Model(s)")) {

                /* Finding every mesh / material in the parent hierarchy and converting them based 
                 * on the transformation settings, including combining the meshes into one.*/
                ModelInfo[] outputModels = _settingsProvider.ModelSettings.UpdateModels(
                    _settingsProvider.TransformSettings.GetModelInformation(),
                    _settingsProvider.TransformSettings.rootObject,
                    _settingsProvider.ExportSettings.fileName);

                if (outputModels.Length > 0) {

                    /* Creating the directory for the model files to be put into.*/
                    if (!Directory.Exists(_settingsProvider.ExportSettings.GetFolderDirectory())) {
                        Directory.CreateDirectory(_settingsProvider.ExportSettings.GetFolderDirectory());
                    }

                    /* Creating the file streams for the .obj and .mtl files.*/
                    using (ObjExporter exporter = new ObjExporter(
                        File.Create(_settingsProvider.ExportSettings.GetFileDirectory(".obj")),
                        File.Create(_settingsProvider.ExportSettings.GetFileDirectory(".mtl")),
                        _settingsProvider.ExportSettings.fileName)) {

                        foreach (ModelInfo modelInfo in outputModels) {
                            exporter.ExportMesh(modelInfo.mesh, modelInfo.materials);
                        }
                    }

                    AssetDatabase.Refresh();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
        #endregion
    }
}

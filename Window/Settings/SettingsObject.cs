using UnityEditor;
using UnityEngine;

namespace ObjectExporter.Window.Settings
{
    internal abstract class SettingsObject : ScriptableObject
    {
        #region Data
        private SerializedObject _serializedObject;
        #endregion

        #region Properties
        public SerializedObject SerializedObject {
            get {
                _serializedObject = new SerializedObject(this);
                return _serializedObject;
            }
            protected set {
                _serializedObject = value;
            }
        }
        #endregion

        #region Public Functions
        public abstract bool IsValid();
        #endregion
    }
}

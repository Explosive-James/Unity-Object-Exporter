using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectExporter.Window.Settings
{
    /// <summary>
    /// Defines where the files are located and what they are called.
    /// </summary>
    internal sealed class ExportSettings : SettingsObject
    {
        #region Data
        /// <summary>
        /// The name of the file minus file extension.
        /// </summary>
        [Header("Export Settings")]
        public string fileName = "ModelName";
        /// <summary>
        /// The folder path in the project folder to place the files.
        /// </summary>
        public string folderPath = "";

        /// <summary>
        /// Hashsets to quickly find invalid characters.
        /// </summary>
        private HashSet<char> _invalidFileCharacters,
            _invalidDirectoryCharacters;
        #endregion

        #region Unity Functions
        private void OnValidate()
        {
            _invalidFileCharacters ??= new HashSet<char>(Path.GetInvalidFileNameChars());
            _invalidDirectoryCharacters ??= new HashSet<char>(Path.GetInvalidPathChars());

            fileName = GetValidString(fileName.Split('.')[0], _invalidFileCharacters);
            folderPath = GetValidString(folderPath, _invalidDirectoryCharacters);

            SerializedObject = new UnityEditor.SerializedObject(this);
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Gets the full directory for the files.
        /// </summary>
        /// <param name="fileExtension">Extension of the file.</param>
        public string GetFileDirectory(string fileExtension)
        {
            if (fileExtension[0] != '.') {
                fileExtension = '.' + fileExtension;
            }

            return Path.Combine(Application.dataPath, folderPath, fileName) + fileExtension;
        }
        /// <summary>
        /// Gets the full directory the files are located in.
        /// </summary>
        public string GetFolderDirectory()
        {
            return Path.Combine(Application.dataPath, folderPath);
        }

        public override bool IsValid() => !string.IsNullOrEmpty(fileName);
        #endregion

        #region Private Functions
        /// <summary>
        /// returns a string without the invalid characters from the input string.
        /// </summary>
        private string GetValidString(string value, HashSet<char> invalidCharacters)
        {
            string results = string.Empty;

            for (int i = 0; i < value.Length; i++)
                if (!invalidCharacters.Contains(value[i])) {
                    results += value[i];
                }

            return results;
        }
        #endregion
    }
}

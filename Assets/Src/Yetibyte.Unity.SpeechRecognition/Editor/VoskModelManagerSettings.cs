using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Yetibyte.Unity.SpeechRecognition.ModelManagement;

namespace Yetibyte.Unity.SpeechRecognition.Editor
{
    public class VoskModelManagerSettings : ScriptableObject
    {
        private static readonly string[] SETTINGS_KEYWORDS = new[] { "Vosk", "Model", "Vosk Model", "Settings", "Vosk Settings", "Vosk Model Settings" };

        public const string SETTINGS_PATH = "Assets/_voskModelManagerSettings.asset";
        public const string DEFAULT_MODEL_PATH = "VoskModels";

        private const string MENU_ITEM_PATH = "Project/VoskSettings";
        private const string SETTINGS_LABEL = "Vosk Settings";
        private const string MODEL_VALID_CHECK_DIR_NAME = "ivector";

        [SerializeField]
        private string _modelPath;

        public string ModelPath
        {
            get
            {
                if(!IsValidModelPath)
                {
                    LogInvalidModelPath();
                    return DEFAULT_MODEL_PATH;
                }

                return _modelPath;
            }
        }


        public string AbsoluteModelDirectoryPath => System.IO.Path.Combine(Application.dataPath, ModelPath);

        public bool IsValidModelPath => !string.IsNullOrWhiteSpace(_modelPath) && !_modelPath.Any(c => System.IO.Path.GetInvalidPathChars().Concat(new char[] { '?' }).Contains(c));

        public string GetAbsoluteModelPath(string modelName) => string.IsNullOrWhiteSpace(modelName) ? null : System.IO.Path.Combine(AbsoluteModelDirectoryPath, modelName);
    

        public IEnumerable<string> GetModelNames()
        {
            if (!System.IO.Directory.Exists(AbsoluteModelDirectoryPath))
                return Array.Empty<string>();

            return System.IO.Directory.EnumerateDirectories(AbsoluteModelDirectoryPath)
                .Where(IsValidModelDirectory)
                .Select(d => System.IO.Path.GetFileName(d));

        }

        public IEnumerable<VoskModelFile> GetModelFiles()
        {
            if (!System.IO.Directory.Exists(AbsoluteModelDirectoryPath))
                return Array.Empty<VoskModelFile>();

            return System.IO.Directory.EnumerateDirectories(AbsoluteModelDirectoryPath)
                .Where(IsValidModelDirectory)
                .Select(d => new VoskModelFile(d));
        }

        private void LogInvalidModelPath()
        {
            Debug.LogWarning($"Vosk Settings: Model Path '{_modelPath}' is not a valid directory path. Falling back to default path '{DEFAULT_MODEL_PATH}'.");
        }

        private static bool IsValidModelDirectory(string path)
        {

            var subDirs = System.IO.Directory.EnumerateDirectories(path);

            bool isValid = subDirs.Any(d => System.IO.Path.GetFileName(d).ToLower() == MODEL_VALID_CHECK_DIR_NAME);

            return isValid;
        }

        internal static VoskModelManagerSettings GetOrCreateSettings()
        {
            VoskModelManagerSettings settings = AssetDatabase.LoadAssetAtPath<VoskModelManagerSettings>(SETTINGS_PATH);

            if (settings == null)
            {
                settings = CreateInstance<VoskModelManagerSettings>();
                settings._modelPath = DEFAULT_MODEL_PATH;

                AssetDatabase.CreateAsset(settings, SETTINGS_PATH);
                AssetDatabase.SaveAssets();

            }

            settings.EnsureModelPath();

            return settings;
        }

        internal static SerializedObject GetSerializedSettings() => new SerializedObject(GetOrCreateSettings());

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {

            var settings = GetSerializedSettings();
            var provider = new SettingsProvider(MENU_ITEM_PATH, SettingsScope.Project)
            {
                label = SETTINGS_LABEL,

                guiHandler = (searchContext) =>
                {
                    EditorGUILayout.PropertyField(settings.FindProperty("_modelPath"), new GUIContent("Model Path"));
                    settings.ApplyModifiedPropertiesWithoutUndo();
                },

                keywords = new HashSet<string>(SETTINGS_KEYWORDS)
            };

            return provider;
        }

        public bool EnsureModelPath()
        {
            if(!IsValidModelPath)
            {
                LogInvalidModelPath();
                _modelPath = DEFAULT_MODEL_PATH;
            }

            if (!System.IO.Directory.Exists(AbsoluteModelDirectoryPath))
            {
                System.IO.Directory.CreateDirectory(AbsoluteModelDirectoryPath);
                AssetDatabase.Refresh();

                return true;
            }

            return false;
        }

        private void OnEnable()
        {
        }

        private void OnValidate()
        {
        }

    }
}

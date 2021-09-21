using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Yetibyte.Unity.SpeechRecognition.ModelManagement;
using Yetibyte.Unity.SpeechRecognition.Util;

namespace Yetibyte.Unity.SpeechRecognition.Editor
{
    public class VoskModelManagerSettings : ScriptableObject
    {
        private static readonly string[] SETTINGS_KEYWORDS = new[] { "Vosk", "Model", "Vosk Model", "Settings", "Vosk Settings", "Vosk Model Settings" };

        private const string MENU_ITEM_PATH = "Project/VoskSettings";

        public const string SETTINGS_PATH = "Assets/_voskModelManagerSettings.asset";
        public const string DEFAULT_MODEL_PATH = "VoskModels";

        public const string SETTINGS_LABEL = "Vosk Settings";

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

        public string GetAbsoluteModelPathByName(string modelName) => string.IsNullOrWhiteSpace(modelName) ? null : System.IO.Path.Combine(AbsoluteModelDirectoryPath, modelName);    

        public IEnumerable<string> GetModelNames()
        {
            if (!System.IO.Directory.Exists(AbsoluteModelDirectoryPath))
                return Array.Empty<string>();

            return System.IO.Directory.EnumerateDirectories(AbsoluteModelDirectoryPath)
                .Where(ModelUtil.IsValidModelDirectory)
                .Select(d => System.IO.Path.GetFileName(d));

        }

        public IEnumerable<string> GetRelativeModelPaths()
        {
            if (!System.IO.Directory.Exists(AbsoluteModelDirectoryPath))
                return Array.Empty<string>();

            return System.IO.Directory.EnumerateDirectories(AbsoluteModelDirectoryPath)
                .Where(ModelUtil.IsValidModelDirectory)
                .Select(d => System.IO.Path.Combine(ModelPath, System.IO.Path.GetFileName(d)));

        }

        public IEnumerable<VoskModelFile> GetModelFiles()
        {
            if (!System.IO.Directory.Exists(AbsoluteModelDirectoryPath))
                return Array.Empty<VoskModelFile>();

            return System.IO.Directory.EnumerateDirectories(AbsoluteModelDirectoryPath)
                .Where(ModelUtil.IsValidModelDirectory)
                .Select(d => new VoskModelFile(d));
        }

        private void LogInvalidModelPath()
        {
            Debug.LogWarning($"Vosk Settings: Model Path '{_modelPath}' is not a valid directory path. Falling back to default path '{DEFAULT_MODEL_PATH}'.");
        }

        public static VoskModelManagerSettings GetOrCreateSettings()
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

        public bool ModelExists(string modelName)
        {
            string absolutePath = GetAbsoluteModelPathByName(modelName);

            return ModelUtil.IsValidModelDirectory(absolutePath);

        }



        public static SerializedObject GetSerializedSettings() => new SerializedObject(VoskModelManagerSettings.GetOrCreateSettings());

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var settings = GetSerializedSettings();
            var provider = new SettingsProvider(MENU_ITEM_PATH, SettingsScope.Project)
            {
                label = VoskModelManagerSettings.SETTINGS_LABEL,

                guiHandler = (searchContext) =>
                {
                    EditorGUILayout.PropertyField(settings.FindProperty("_modelPath"), new GUIContent("Model Path"));
                    settings.ApplyModifiedPropertiesWithoutUndo();
                },

                keywords = new HashSet<string>(SETTINGS_KEYWORDS)
            };

            return provider;
        }

        private void OnEnable()
        {
        }

        private void OnValidate()
        {
        }

    }
}

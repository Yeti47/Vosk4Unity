using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Yetibyte.Unity.SpeechRecognition.ModelManagement;

namespace Yetibyte.Unity.SpeechRecognition.Editor
{
    public class ModelManagerWindow : EditorWindow
    {
        private class DetailWindow : EditorWindow
        {
            private const int MIN_WIDTH = 300;
            private const int MIN_HEIGHT = 200;

            private const int MAX_WIDTH = 500;
            private const int MAX_HEIGHT = 200;

            public IVoskModelInfo ModelInfo { get; private set; }

            public static DetailWindow ShowWindow(IVoskModelInfo modelInfo, Vector2? position = null)
            {
                DetailWindow window = EditorWindow.GetWindow<DetailWindow>(true, $"{modelInfo.Name} - Details", true);
                window.ModelInfo = modelInfo;

                if (position.HasValue)
                    window.position = new Rect(position.Value - new Vector2(window.position.size.x / 2, 0), window.position.size);

                window.maxSize = new Vector2(MAX_WIDTH, MAX_HEIGHT);
                window.minSize = new Vector2(MIN_WIDTH, MIN_HEIGHT);

                window.Show();

                return window;
            }

            private void OnGUI()
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel("Details");

                if(ModelInfo != null)
                    EditorGUILayout.SelectableLabel(ModelInfo.Details.TechnicalDetails, EditorStyles.textArea, GUILayout.Height(60));

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel("Notes");

                if (ModelInfo != null)
                    EditorGUILayout.SelectableLabel(ModelInfo.Details.Notes, EditorStyles.textArea, GUILayout.Height(80));

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel("License");

                if (ModelInfo != null)
                    EditorGUILayout.SelectableLabel(ModelInfo.Details.License, EditorStyles.textField);

                EditorGUILayout.EndHorizontal();

            }

        }

        private const int MIN_WIDTH = 480;
        private const int MIN_HEIGHT = 520;

        private const string MODEL_SIZE_UNIT = "MB";
        private const string BUTTON_LABEL_SHOW_MODELS = "Show available models";
        private const string WINDOW_TITLE = "Vosk Model Manager";

        private readonly List<IVoskModelProvider> _modelProviders = new List<IVoskModelProvider>();

        private int _selectedProviderIndex = -1;
        private Vector2 _scrollPosLocalModels = Vector2.zero;
        private Vector2 _scrollPosImportModels = Vector2.zero;

        private static IVoskModelInfo _modelToImport;

        #region Props


        private static GUIStyle HeaderGuiStyle => new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold };

        private static GUIStyle SuperHeaderGuiStyle => new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 14,
            margin = new RectOffset(
                    GUIStyle.none.margin.left + 4,
                    GUIStyle.none.margin.right + 4,
                    GUIStyle.none.margin.top + 4,
                    GUIStyle.none.margin.bottom + 2
                )
        };

        private IVoskModelProvider SelectedModelProvider => _selectedProviderIndex >= 0 && _selectedProviderIndex < _modelProviders.Count ? _modelProviders[_selectedProviderIndex] : null;

        #endregion

        #region Methods

        [MenuItem("Yetibyte/Vosk4Unity/Vosk Model Manager")]
        private static void Init()
        {
            ModelManagerWindow window = (ModelManagerWindow)EditorWindow.GetWindow(typeof(ModelManagerWindow));
            window.titleContent = new GUIContent(WINDOW_TITLE);
            window.Show();
        }

        private void LoadModelProviders()
        {
            Debug.Log("Loading Vosk model providers...");

            _modelProviders.Clear();

            // Always add the Http model provider.
            _modelProviders.Add(new HttpVoskModelProvider());

            foreach(var provider in _modelProviders)
            {
                Debug.Log($"Loaded model provider '{provider.Name}'.");
            }

            _selectedProviderIndex = 0;
        }

        private void DrawLocalModels()
        {
            DrawHeader("Local models");

            var modelFiles = VoskModelManagerSettings.GetOrCreateSettings().GetModelFiles();

            GUILayout.BeginHorizontal("box");

            GUILayout.Label("Model", new GUIStyle(HeaderGuiStyle) { fixedWidth = this.position.width * 0.4f });
            GUILayout.Label("Size", HeaderGuiStyle);
            GUILayout.Label("Action", new GUIStyle(HeaderGuiStyle) { 
                alignment = TextAnchor.MiddleRight,
                margin = new RectOffset(HeaderGuiStyle.margin.left, HeaderGuiStyle.margin.right + 92, HeaderGuiStyle.margin.top, HeaderGuiStyle.margin.bottom)

            });

            GUILayout.EndHorizontal();

            _scrollPosLocalModels = GUILayout.BeginScrollView(_scrollPosLocalModels, false, true, GUILayout.Height(100));

            GUIStyle nameStyle = new GUIStyle(EditorStyles.label) { fixedWidth = this.position.width * 0.4f, margin = new RectOffset(4, 2, 0, 0) };
            GUIStyle sizeStyle = new GUIStyle(EditorStyles.label);
            GUIStyle actionStyle = new GUIStyle(GUI.skin.button) { fixedWidth = 120 };

            foreach (var modelFile in modelFiles)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(modelFile.Name, nameStyle);
                GUILayout.Label(modelFile.Size.ToString() + " " + MODEL_SIZE_UNIT, sizeStyle);

                if(GUILayout.Button("Delete", actionStyle))
                {
                    HandleDeleteButtonClick(modelFile);
                }

                GUILayout.EndHorizontal();

                DrawLine(1);

            }

            GUILayout.EndScrollView();
        }

        private void HandleDeleteButtonClick(VoskModelFile modelFile)
        {
            if (modelFile == null)
                return;

            if(!EditorUtility.DisplayDialog($"{modelFile.Name} - Delete", $"Are you sure you want to delete the Vosk model '{modelFile.Name}'?", "Delete", "Cancel")) {
                return;
            }

            string modelDirectory = VoskModelManagerSettings.GetOrCreateSettings().AbsoluteModelDirectoryPath;
            string modelPath = System.IO.Path.Combine(modelDirectory, modelFile.Name);

            System.IO.Directory.Delete(modelPath, true);
            System.IO.File.Delete(modelPath + ".meta");

            Debug.Log($"Deleted model '{modelFile.Name}' at path '{modelPath}'.");

            AssetDatabase.Refresh();

            Repaint();
        }

        private void DrawHeader(string text)
        {

            GUILayout.Label(text, SuperHeaderGuiStyle);
            //EditorGUILayout.Space();
        }

        private void DrawModelProviders()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Source");
            _selectedProviderIndex = EditorGUILayout.Popup(_selectedProviderIndex, _modelProviders.Select(p => p.Source).ToArray());

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();

            if(GUILayout.Button(BUTTON_LABEL_SHOW_MODELS) && SelectedModelProvider != null)
            {
                if(!SelectedModelProvider.FetchModels())
                {
                    Debug.LogError($"Error loading models from model provider '{SelectedModelProvider.Name}'.");
                }
            }

            EditorGUILayout.EndHorizontal();

        }

        private void DrawImportProgressArea()
        {
            if (_modelToImport == null)
                return;

            EditorGUILayout.HelpBox($"Importing model '{_modelToImport.Name}' ({_modelToImport.Size} {MODEL_SIZE_UNIT}).\r\nPlease wait...", MessageType.Info);
        }

        private void DrawModelImportTable(IVoskModelProvider modelProvider)
        {

            GUILayout.BeginHorizontal("box");

            GUIStyle nameHeaderStyle = new GUIStyle(HeaderGuiStyle) { fixedWidth = position.width * 0.30f };
            GUIStyle categoryHeaderStyle = new GUIStyle(HeaderGuiStyle) { fixedWidth = position.width * 0.20f };
            GUIStyle sizeHeaderStyle = new GUIStyle(HeaderGuiStyle) { fixedWidth = position.width * 0.15f };
            GUIStyle licenseHeaderStyle = new GUIStyle(HeaderGuiStyle) { fixedWidth = position.width * 0.20f };
            GUIStyle actionHeaderStyle = new GUIStyle(HeaderGuiStyle) { fixedWidth = position.width * 0.15f };

            GUILayout.Label("Name", nameHeaderStyle);
            GUILayout.Label("Category", categoryHeaderStyle);
            GUILayout.Label("Size", sizeHeaderStyle);
            GUILayout.Label("License", licenseHeaderStyle);

            GUILayout.Label("Action", actionHeaderStyle);

            GUILayout.EndHorizontal();

            GUIStyle colStyle = new GUIStyle(EditorStyles.label) { clipping = TextClipping.Clip, margin = new RectOffset(4, 2, 0, 0) };

            GUIStyle nameStyle = new GUIStyle(colStyle) { fixedWidth = nameHeaderStyle.fixedWidth };
            GUIStyle categoryStyle = new GUIStyle(colStyle) { fixedWidth = categoryHeaderStyle.fixedWidth };
            GUIStyle sizeStyle = new GUIStyle(colStyle) { fixedWidth = sizeHeaderStyle.fixedWidth };
            GUIStyle licenseStyle = new GUIStyle(colStyle) { fixedWidth = licenseHeaderStyle.fixedWidth };
            GUIStyle actionStyle = new GUIStyle(GUI.skin.button) { fixedWidth = actionHeaderStyle.fixedWidth - 50 };

            _scrollPosImportModels = GUILayout.BeginScrollView(_scrollPosImportModels, false, true, GUILayout.Height(240));

            foreach (var model in modelProvider.Models)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(model.Name, nameStyle);
                GUILayout.Label(model.Category, categoryStyle);
                GUILayout.Label(model.Size.ToString() + " " + MODEL_SIZE_UNIT, sizeStyle);
                GUILayout.Label(model.Details.License, licenseStyle);

                GUILayout.BeginVertical();

                if (GUILayout.Button("Import", actionStyle))
                {
                    HandleImportButtonClick(model);
                }

                if (GUILayout.Button("Details", actionStyle))
                {
                    DetailWindow.ShowWindow(model, EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition));
                }

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();

                GUILayout.Space(2);
                DrawLine(1);

            }

            GUILayout.EndScrollView();
        }

        private static void DrawLine(int thickness)
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(thickness));
        }

        private void HandleImportButtonClick(IVoskModelInfo model)
        {
            if (_modelToImport != null)
            {
                Debug.LogWarning("Please wait until the current import process is finished.");
                return;
            }

            _modelToImport = model;
            _modelToImport.ImportComplete += modelToImport_ImportComplete;

            string modelPath = VoskModelManagerSettings.GetOrCreateSettings().ModelPath;

            bool importSuccess = model.Import(modelPath, out string importMessage);

            if (importSuccess)
            {
                Debug.Log($"Importing model '{model.Name}'.");
            }
            else
            {
                _modelToImport.ImportComplete -= modelToImport_ImportComplete;
                _modelToImport = null;
                Debug.LogError(importMessage);
            }
        }

        private static void modelToImport_ImportComplete(object sender, VoskModelImportEventArgs e)
        {
            _modelToImport.ImportComplete -= modelToImport_ImportComplete;

            if(e.IsError)
            {
                Debug.LogError($"Error during model import: {e.Message}");
            }
            else
            {
                AssetDatabase.Refresh();

                string modelPath = VoskModelManagerSettings.GetOrCreateSettings().ModelPath;

                Debug.Log($"Successfully imported model '{_modelToImport.Name}' into project's model path '{modelPath}'.");
            }

            _modelToImport = null;

            EditorWindow.GetWindow<ModelManagerWindow>().Repaint();
        }

        #endregion

        #region Unity Message Methods

        private void OnEnable()
        {
            minSize = new Vector2(MIN_WIDTH, MIN_HEIGHT);

            LoadModelProviders();
        }

        private void OnGUI()
        {
            DrawLocalModels();

            DrawLine(2);

            DrawHeader("Import new models");

            if (_modelToImport == null)
            {
                DrawModelProviders();

                EditorGUILayout.Space(4);

                if (SelectedModelProvider != null)
                    DrawModelImportTable(SelectedModelProvider);
            }
            else
            {
                DrawImportProgressArea();
            }

        }

        #endregion
    }
}

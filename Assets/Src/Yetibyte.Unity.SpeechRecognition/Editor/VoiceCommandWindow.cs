using UnityEditor;
using UnityEngine;

namespace Yetibyte.Unity.SpeechRecognition.Editor
{
    public class VoiceCommandWindow : EditorWindow
    {
        private const int MIN_WIDTH = 300;
        private const int MIN_HEIGHT = 250;

        private const int MAX_WIDTH = 600;
        private const int MAX_HEIGHT = 800;

        [SerializeField]
        private SerializedProperty _serializedVoiceCommand;

        private Vector2 _scrollPosition;

        private SerializedProperty SerializedNameProperty => _serializedVoiceCommand.FindPropertyRelative("_name");
        private SerializedProperty SerializedKeywordSettings => _serializedVoiceCommand.FindPropertyRelative("_keywordSettings");
        private SerializedProperty SerializedCallback => _serializedVoiceCommand.FindPropertyRelative("_callback");
        
        
        public static void CloseAll()
        {
            var window = EditorWindow.GetWindow<VoiceCommandWindow>();

            if (window != null)
                window.Close();
        }

        public static VoiceCommandWindow ShowWindow(int commandIndex, SerializedProperty serializedVoiceCommand, Vector2? position = null)
        {
            string title = $"Edit Voice Command [{commandIndex}]";

            VoiceCommandWindow window = EditorWindow.GetWindow<VoiceCommandWindow>(false, title, true);
            window._serializedVoiceCommand = serializedVoiceCommand;
            window.titleContent = new GUIContent(title);

            if (position.HasValue)
                window.position = new Rect(position.Value - new Vector2(window.position.size.x / 2, 0), window.position.size);

            window.maxSize = new Vector2(MAX_WIDTH, MAX_HEIGHT);
            window.minSize = new Vector2(MIN_WIDTH, MIN_HEIGHT);

            window.Show();

            return window;
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            {
                SerializedNameProperty.stringValue = EditorGUILayout.TextField("Name", SerializedNameProperty.stringValue);
            }
            bool hasDetectedChange = EditorGUI.EndChangeCheck();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.PropertyField(SerializedKeywordSettings);
                }
                hasDetectedChange |= EditorGUI.EndChangeCheck();
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(SerializedCallback);
            }
            hasDetectedChange |= EditorGUI.EndChangeCheck();

            if (hasDetectedChange)
            {
                _serializedVoiceCommand.serializedObject.ApplyModifiedProperties();
                _serializedVoiceCommand.serializedObject.Update();
            }

        }

    }
}

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Yetibyte.Unity.SpeechRecognition.KeywordDetection;

namespace Yetibyte.Unity.SpeechRecognition.Editor
{

    [CustomPropertyDrawer(typeof(VoiceCommandList))]
    public class CommandListDrawer : PropertyDrawer
    {

        #region Constants

        private const int COMMAND_CONFIG_WINDOW_WIDTH = 320;
        private const int COMMAND_CONFIG_WINDOW_HEIGHT = 240;

        private const string VOICE_COMMANDS_FIELD_NAME = "_voiceCommands";
        private const string VOICE_COMMAND_NAME_FIELD_NAME = "_name";

        private const string VOICE_COMMAND_NAME_PREFIX = "VoiceCommand_";

        #endregion

        #region Fields

        private bool _isOpen = false;

        #endregion

        #region Props

        #endregion

        #region Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float propHeight = EditorGUIUtility.singleLineHeight;

            if (_isOpen)
                propHeight += GetCommandCount(property) * EditorGUIUtility.singleLineHeight + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            return propHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            _isOpen = EditorGUI.Foldout(position, _isOpen, label);

            if(_isOpen)
            {
                EditorGUI.indentLevel = 1;
                position = EditorGUI.IndentedRect(position);

                for (int i = 0; i < GetCommandCount(property); i++)
                {
                    position.y += EditorGUIUtility.singleLineHeight;

                    DrawCommandItem(position, property, i);
                }

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                if(GUI.Button(position, "Add Voice Command"))
                {
                    AddNewVoiceCommand(property);
                }

            }

        }

        private void DrawCommandItem(Rect position, SerializedProperty property, int index)
        {
            GUI.Box(position, string.Empty);

            const float textFieldSizePercent = 0.6f;
            const float marginX = 1f;

            var serializedCommand = GetSerializedCommandAt(index, property);

            serializedCommand.FindPropertyRelative(VOICE_COMMAND_NAME_FIELD_NAME).stringValue = GUI.TextField(
                new Rect(position) { width = position.width * textFieldSizePercent - marginX, x = position.x + marginX },
                serializedCommand.FindPropertyRelative(VOICE_COMMAND_NAME_FIELD_NAME).stringValue
            );

            bool isEditClicked = GUI.Button(new Rect(position) { width = (position.width - position.width * textFieldSizePercent) / 2f - marginX, x = position.x + position.width * textFieldSizePercent }, 
                "Edit"
            );

            bool isRemoveClicked = GUI.Button(new Rect(position) { width = (position.width - position.width * textFieldSizePercent) / 2f - marginX, x = position.x + position.width * textFieldSizePercent + (position.width - position.width * textFieldSizePercent) / 2f - marginX },
                "Remove"
            );

            if (isEditClicked)
            {
                VoiceCommandWindow.ShowWindow(index, serializedCommand);
            }

            if(isRemoveClicked)
            {
                property.FindPropertyRelative(VOICE_COMMANDS_FIELD_NAME).DeleteArrayElementAtIndex(index);
                VoiceCommandWindow.CloseAll();
            }

        }

        private SerializedProperty AddNewVoiceCommand(SerializedProperty property)
        {
            property.FindPropertyRelative(VOICE_COMMANDS_FIELD_NAME).InsertArrayElementAtIndex(GetCommandCount(property));

            var serializedVoiceCmd = property.FindPropertyRelative(VOICE_COMMANDS_FIELD_NAME).GetArrayElementAtIndex(GetCommandCount(property) - 1);

            serializedVoiceCmd.FindPropertyRelative(VOICE_COMMAND_NAME_FIELD_NAME).stringValue = FindNewCommandName(property);

            return serializedVoiceCmd;
        
        }

        private string FindNewCommandName(SerializedProperty property)
        {
            int index = GetCommandCount(property) - 1;
            string name = $"{VOICE_COMMAND_NAME_PREFIX}{index}";

            Func<string, bool> existsFunc = n =>
            {
                for (int i = 0; i < GetCommandCount(property); i++)
                {
                    var currSerializedCommand = GetSerializedCommandAt(i, property);

                    if (currSerializedCommand.FindPropertyRelative(VOICE_COMMAND_NAME_FIELD_NAME).stringValue == n)
                        return true;

                }

                return false;
            };

            while(existsFunc.Invoke(name))
            {
                name = $"{VOICE_COMMAND_NAME_PREFIX}{++index}";
            }

            return name;
        }

        private SerializedProperty GetSerializedCommandAt(int index, SerializedProperty property)
        {
            return property.FindPropertyRelative(VOICE_COMMANDS_FIELD_NAME).GetArrayElementAtIndex(index);
        }

        private int GetCommandCount(SerializedProperty property) => property.FindPropertyRelative(VOICE_COMMANDS_FIELD_NAME).arraySize;


        #endregion
    }
}

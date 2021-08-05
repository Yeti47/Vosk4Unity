using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Yetibyte.Unity.SpeechRecognition.Editor
{

    [CustomPropertyDrawer(typeof(ModelPathAttribute))]
    public class ModelPathDrawer : PropertyDrawer
    {
        private const string MODEL_NAME_NONE = "-None-";

        private int _selectedModelIndex = -1;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight *
                (VoskModelManagerSettings.GetOrCreateSettings().ModelExists(property.stringValue) || string.IsNullOrWhiteSpace(property.stringValue) ? 1 : 4);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginChangeCheck();

            _selectedModelIndex = EditorGUI.Popup(
                position,
                property.displayName,
                GetSelectedModelIndex(property),
                GetModelNames().ToArray()
            );

            if(EditorGUI.EndChangeCheck())
            {
                property.stringValue = GetSelectedModelName(_selectedModelIndex);
            }

            if(!string.IsNullOrWhiteSpace(property.stringValue) && !VoskModelManagerSettings.GetOrCreateSettings().ModelExists(property.stringValue))
            {
                EditorGUI.HelpBox(
                    new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight * 3),
                    $"Model '{property.stringValue}' does not exist. Import the model into the project's model path or select a different one.", 
                    MessageType.Error
                );
            }

        }

        private string GetSelectedModelName(int index) => index <= 0 ? string.Empty : GetModelNames().ElementAt(index);

        private int GetSelectedModelIndex(SerializedProperty property)
        {
            if (string.IsNullOrWhiteSpace(property.stringValue))
                return 0;

            int index = -1;

            foreach(var modelName in GetModelNames())
            {
                index++;

                if (modelName == property.stringValue)
                    return index;
            }

            return -1;
        }

        private IEnumerable<string> GetModelNames()
        {
            return new[] { MODEL_NAME_NONE }.Concat(VoskModelManagerSettings.GetOrCreateSettings().GetModelNames());
        }
    }
}

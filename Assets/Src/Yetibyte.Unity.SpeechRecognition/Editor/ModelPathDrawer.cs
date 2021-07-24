using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Yetibyte.Unity.SpeechRecognition.Editor
{
    [CustomPropertyDrawer(typeof(ModelPathAttribute))]
    public class ModelPathDrawer : PropertyDrawer
    {

        private int _selectedModelIndex = -1;

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

        }

        private string GetSelectedModelName(int index) => GetModelNames().ElementAt(index);

        private int GetSelectedModelIndex(SerializedProperty property)
        {
            int index = -1;

            foreach(var modelName in GetModelNames())
            {
                index++;

                if (modelName == property.stringValue)
                    return index;
            }

            return -1;
        }

        private IEnumerable<string> GetModelNames() => VoskModelManagerSettings.GetOrCreateSettings().GetModelNames();

    }
}

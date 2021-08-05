using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Yetibyte.Unity.SpeechRecognition.Editor
{
    [CustomPropertyDrawer(typeof(RecordingDeviceAttribute))]
    public class RecordingDevicePropertyDrawer  : PropertyDrawer
    {
        [Serializable]
        public class RecordingDeviceSelectionItem
        {
            [SerializeField]
            private string _text;

            [SerializeField]
            private string _value;

            public string Text => _text;
            public string Value => _value;

            public static RecordingDeviceSelectionItem Default { get; } = new RecordingDeviceSelectionItem("Default", string.Empty);

            public RecordingDeviceSelectionItem(string text, string value)
            {
                _text = text;
                _value = value;
            }

            public RecordingDeviceSelectionItem(string text) : this(text, text)
            {

            }
        }

        private const int APPLY_BUTTON_WIDTH = 72;
        private const string APPLY_BUTTON_TEXT = "Apply";

        private int _deviceItemIndex;

        private RecordingDeviceSelectionItem SelectedDeviceItem
        {
            get
            {
                var deviceItems = DeviceItems;
                int itemIndex = _deviceItemIndex > 0 && _deviceItemIndex < deviceItems.Count() ? _deviceItemIndex : 0;
                return deviceItems.ElementAt(itemIndex);
            }
        }

        private static IEnumerable<RecordingDeviceSelectionItem> DeviceItems
        {
            get
            {
                return new[] { RecordingDeviceSelectionItem.Default }
                    .Concat(Microphone.devices.Select(d => new RecordingDeviceSelectionItem(d)));     
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(position, property, label);

            Rect popupPos = new Rect(
                position.x + EditorGUIUtility.labelWidth, 
                position.y + EditorGUIUtility.singleLineHeight, 
                position.width - EditorGUIUtility.labelWidth - APPLY_BUTTON_WIDTH, 
                position.height
            );

            _deviceItemIndex = EditorGUI.Popup(
                popupPos,
                _deviceItemIndex, 
                DeviceItems.Select(d => d.Text).ToArray()
            );

            Rect buttonPos = new Rect(popupPos.x + popupPos.width, popupPos.y, APPLY_BUTTON_WIDTH, popupPos.height);

            if (GUI.Button(buttonPos, APPLY_BUTTON_TEXT))
            {
                property.stringValue = SelectedDeviceItem.Value;
            }

        }

    }
}

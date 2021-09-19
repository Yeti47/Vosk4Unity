using UnityEditor;
using UnityEngine;

namespace Yetibyte.Unity.SpeechRecognition.Editor
{
    [CustomPropertyDrawer(typeof(VoskExampleReadmeAttribute))]
    public class VoskExampleReadmeDrawer : PropertyDrawer
    {
        private const string EXAMPLE_MODEL_NAME = "vosk-model-small-en-us-0.15";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 500;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.y += 4;
            position.height = 24;

            GUIStyle headlineStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 22 };

            EditorGUI.LabelField(position, "Read Me", headlineStyle);

            position.y += 28;
            position.height = 400;

            EditorGUI.LabelField(position, $"To use this example, make sure the model <b>'{EXAMPLE_MODEL_NAME}'</b> is " +
                $"selected in the <i>'Speech Recognition Settings'</i> section of the Vosk Listener component (Game Object <i>VoskExample</i>)." + 
                $"\n\nIf you do not have that model, you can download it by following these simple steps:" +
                $"\n\n1. Go to Yetibyte -> Vosk4Unity -> Vosk Model Manager" +
                $"\n2. In the <i>Import New Models</i> section, select <b>alphacephei.com</b> as the source." +
                $"\n3. Click <i>Show available models</i>." +
                $"\n4. Search for the model named <b>'{EXAMPLE_MODEL_NAME}'</b> and click the <i>Download</i> button in the corresponding row." + 
                $"\n5. Wait for the download to finish. The model should now be listed under <i>Local models</i>." + 
                $"\n\n<size=18><b>Voice Commands</b></size>" + 

                $"\n\nThe following voice commands are available in this demo:" +
                $"\n\n<b>spawn cube</b>: Spawns a new cube at a random position." +
                $"\n<b>make cube jump</b>: Makes the last spawned cube jump." +
                $"\n<b>jump for me</b>: Same as 'make cube jump'." +
                $"\n<b>Tint cube [red/green/blue]</b>: Tints the last spawned cube in the specified color.", 
                new GUIStyle(EditorStyles.wordWrappedLabel) { richText = true}
            );


        }
    }
}

using UnityEditor;
using UnityEngine;

namespace Yetibyte.Unity.SpeechRecognition.Editor
{
    public class Vosk4UnityLicenseWindow : LicenseWindow
    {
        public override Vector2 MinSize => new Vector2(600, 300);

        public override Vector2 MaxSize => new Vector2(600, 300);

        public override string Title => "Vosk4Unity License";

        protected override string LicenseFileName => "VOSK4UNITY_LICENSE";

        protected override string LicenseFilePath => "Assets";

        protected override float LicenseTextHeight => 380;


        [MenuItem("Yetibyte/Vosk4Unity/Vosk4Unity License", priority = 10000)]
        private static void Init()
        {
            Vosk4UnityLicenseWindow window = EditorWindow.GetWindow<Vosk4UnityLicenseWindow>(true);
            window.Initialize();

            window.Show();
        }
    }
}

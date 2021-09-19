using UnityEditor;
using UnityEngine;

namespace Yetibyte.Unity.SpeechRecognition.Editor
{
    public class VoskLicenseWindow : LicenseWindow
    {
        public override Vector2 MinSize => new Vector2(500, 300);

        public override Vector2 MaxSize => new Vector2(500, 300);

        public override string Title => "Vosk License";

        protected override string LicenseFileName => "VOSK_LICENSE";

        protected override string LicenseFilePath => "Assets/lib";

        protected override float LicenseTextHeight => 2800;


        [MenuItem("Yetibyte/Vosk4Unity/Vosk License", priority = 10001)]
        private static void Init()
        {
            VoskLicenseWindow window = EditorWindow.GetWindow<VoskLicenseWindow>(true);
            window.Initialize();

            window.Show();
        }
    }
}

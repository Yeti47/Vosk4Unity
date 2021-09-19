using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Yetibyte.Unity.SpeechRecognition.Editor
{


    public class VoskAboutWindow : EditorWindow
    {
        private const string COMPANY_NAME = "Yetibyte";
        private const string COMPANY_CONTACT = "info@yetibyte.net";
        private const string DEVELOPER_NAME = "Alexander Herrfurth";
        private const string VERSION_NUMBER = "0.1";
        private const string PRODUCT_NAME = "Vosk4Unity";

        private const string VOSK_LICENSE = "Apache 2.0";
        private const string VOSK4UNITY_LICENSE = "MIT";

        private const string GITHUB_LINK_VOSK4UNITY = "https://github.com/yeti47/vosk4unity";
        private const string GITHUB_LINK_VOSK = "https://github.com/alphacep/vosk-api";

        private const string WINDOW_TITLE = "About Vosk4Unity";

        private const string LOGO_TEXTURE_NAME = "yetibyte_logo";

        private static readonly Vector2 FIXED_SIZE = new Vector2(400, 420);

        private Texture2D _yetibyteLogo;

        [MenuItem("Yetibyte/Vosk4Unity/About Vosk4Unity")]
        private static void Init()
        {
            VoskAboutWindow window = EditorWindow.GetWindow<VoskAboutWindow>(true);
            window.titleContent = new GUIContent(WINDOW_TITLE);

            window.minSize = FIXED_SIZE;
            window.maxSize = FIXED_SIZE;

            window.Show();
        }

        #region Unity Message Methods

        private void OnEnable()
        {
            if(!_yetibyteLogo)
            {
                var assetsGuids = AssetDatabase.FindAssets(LOGO_TEXTURE_NAME);

                if(assetsGuids.Length > 0)
                {
                    _yetibyteLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(assetsGuids.First()));
                }

            }
        }

        private void OnGUI()
        {

            EditorGUILayout.Space(2);

            EditorGUILayout.LabelField(PRODUCT_NAME, new GUIStyle(EditorStyles.boldLabel) { fontSize = 20 }, GUILayout.Height(30));
            EditorGUILayout.LabelField($"Version {VERSION_NUMBER}", EditorStyles.miniLabel);

            EditorGUILayout.Space(6);

            EditorGUI.LabelField(new Rect(152, 16, 232, 20), "Powered by Vosk", new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12
            });            
            EditorGUI.LabelField(new Rect(152, 30, 232, 20), "- an open source speech recognition toolkit -", new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10
            });

            EditorGUILayout.Space(10);

            if(_yetibyteLogo)
                GUI.DrawTexture(new Rect(120, 75, 160, 160), _yetibyteLogo);

            EditorGUILayout.Space(180);

            EditorGUILayout.BeginHorizontal();

            GUIStyle labelStyle = new GUIStyle(EditorStyles.label) { fixedWidth = 140, stretchWidth = false };

            EditorGUILayout.PrefixLabel("Developed by:", labelStyle);
            EditorGUILayout.LabelField(DEVELOPER_NAME, EditorStyles.label);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Contact:", labelStyle);
            EditorGUILayout.LabelField(COMPANY_CONTACT, EditorStyles.linkLabel);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Vosk4Unity on GitHub:", labelStyle);
            EditorGUILayout.LabelField(GITHUB_LINK_VOSK4UNITY, EditorStyles.linkLabel);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Vosk on GitHub:", labelStyle);
            EditorGUILayout.LabelField(GITHUB_LINK_VOSK, EditorStyles.linkLabel);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{PRODUCT_NAME} is distributed under the {VOSK4UNITY_LICENSE} license.", new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter } );
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(1);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Vosk is distributed under the {VOSK_LICENSE} license.", new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter });

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8);

            EditorGUILayout.LabelField($"(c) {DateTime.Now.Year} {COMPANY_NAME}", new GUIStyle(EditorStyles.label) { fontSize = 12, alignment = TextAnchor.MiddleCenter });

        }

        #endregion

    }
}

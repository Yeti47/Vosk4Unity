using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Yetibyte.Unity.SpeechRecognition.Editor
{
    public abstract class LicenseWindow : EditorWindow
    {
        protected Vector2 _scrollPos;
        protected TextAsset _licenseTextFile;

        public abstract Vector2 MinSize { get; }
        public abstract Vector2 MaxSize { get; }

        public abstract string Title { get; }

        protected abstract string LicenseFileName { get; }
        protected abstract string LicenseFilePath { get; }

        protected abstract float LicenseTextHeight { get; }

        protected virtual void Initialize()
        {
            minSize = MinSize;
            maxSize = MaxSize;

            titleContent = new GUIContent(Title);
        }

        protected virtual void OnEnable()
        {
            if (!_licenseTextFile)
            {
                var assetsGuids = AssetDatabase.FindAssets(LicenseFileName, new[] { LicenseFilePath });

                if (assetsGuids.Length > 0)
                {
                    _licenseTextFile = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(assetsGuids.First()));
                }

            }
        }

        protected virtual void OnGUI()
        {
            if (!_licenseTextFile)
                return;

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(Mathf.Clamp(MinSize.y - 5, 5, 9999)));
            {
                EditorGUILayout.SelectableLabel(_licenseTextFile.text, EditorStyles.textField, GUILayout.Height(LicenseTextHeight));
            }
            EditorGUILayout.EndScrollView();
        }

    }
}

using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Yetibyte.Unity.SpeechRecognition.Editor;

namespace Yetibyte.Unity.SpeechRecognition.KeywordDetection
{

    [Serializable]
    public class KeywordSetting
    {
        [SerializeField]
        [ModelPath]
        private string _modelName;

        [SerializeField]
        private string _keyword;

        [SerializeField]
        private KeywordMatchMode _matchMode = KeywordMatchMode.Text;

        public string ModelName => _modelName ?? string.Empty;
        public string Keyword => _keyword ?? string.Empty;

        public KeywordMatchMode MatchMode => _matchMode;

        protected KeywordSetting() { }

        public KeywordSetting(string modelName, string keyword)
        {
            _modelName = modelName;
            _keyword = keyword;
        }

        public bool IsMatch(string modelName, string detectedText)
        {
            detectedText = detectedText ?? string.Empty;

            return (string.IsNullOrWhiteSpace(ModelName) || ModelName.Equals(modelName, StringComparison.OrdinalIgnoreCase))
                && HasTextualMatch(detectedText);
        }

        public bool HasTextualMatch(string detectedText)
        {

            if(_matchMode == KeywordMatchMode.Text)
            {
                return !string.IsNullOrWhiteSpace(detectedText)
                    && detectedText.Trim().ToLower().Contains(Keyword.Trim().ToLower());
            }
            else if(_matchMode == KeywordMatchMode.RegularExpression)
            {
                return Regex.IsMatch(detectedText, Keyword);
            }

            throw new InvalidOperationException("Unsupported Keyword Match Mode!");
                
        }

    }

}


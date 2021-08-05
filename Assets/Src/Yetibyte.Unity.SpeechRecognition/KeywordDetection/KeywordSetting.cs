using System;
using System.Linq;
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

        public string ModelName => _modelName ?? string.Empty;
        public string Keyword => _keyword ?? string.Empty;

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
            return !string.IsNullOrWhiteSpace(detectedText)
                && detectedText.Trim().ToLower().Contains(Keyword.Trim().ToLower());
        }

    }

}


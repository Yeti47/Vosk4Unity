using System;
using UnityEngine;

namespace Yetibyte.Unity.SpeechRecognition.KeywordDetection
{
    [Serializable]
    public class KeywordDetectionSettings
    {
        private const float DEFAULT_CONFIDENCE_THRESHOLD = 100;

        [SerializeField]
        private KeywordDetectionMode _mode = KeywordDetectionMode.Smart;

        [SerializeField]
        [Min(0)]
        private float _confidenceThreshold = 100f;

        public KeywordDetectionMode Mode { get => _mode; set => _mode = value; }

        public float ConfidenceThreshold { get => _confidenceThreshold; set => _confidenceThreshold = value; }

        public static KeywordDetectionSettings CreateDefault()
        {
            return new KeywordDetectionSettings
            {
                _mode = KeywordDetectionMode.Smart,
                _confidenceThreshold = DEFAULT_CONFIDENCE_THRESHOLD
            };
        }

    }

}


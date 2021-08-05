using System;

namespace Yetibyte.Unity.SpeechRecognition.KeywordDetection
{
    [Serializable]
    public enum KeywordDetectionMode
    {
        [UnityEngine.Tooltip("Disable keyword detection.")]
        Off,
        [UnityEngine.Tooltip("Only process full speech recognition results.")]
        Smart,
        [UnityEngine.Tooltip("Process partial speech recognition results.")]
        Fast
    }

}


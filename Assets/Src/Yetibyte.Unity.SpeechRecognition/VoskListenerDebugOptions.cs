using System;
using UnityEngine;

namespace Yetibyte.Unity.SpeechRecognition
{
    [Serializable]
    public class VoskListenerDebugOptions
    {

        [SerializeField]
        private bool _logResults;

        [SerializeField]
        private bool _logPartialResults;

        [SerializeField]
        private bool _logModelLoad;

        [SerializeField]
        private bool _logRecording;

        public bool LogResults { get => _logResults; set => _logResults = value; }
        public bool LogPartialResults { get => _logPartialResults; set => _logPartialResults = value; }
        public bool LogModelLoad { get => _logModelLoad; set => _logModelLoad = value; }
        public bool LogRecording { get => _logRecording; set => _logRecording = value; }

        public static VoskListenerDebugOptions CreateAllDisabled()
        {
            return new VoskListenerDebugOptions
            {
                LogResults = false,
                LogPartialResults = false,
                LogModelLoad = false,
                LogRecording = false
            };
        }
    }
}

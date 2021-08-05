using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Yetibyte.Unity.SpeechRecognition.KeywordDetection
{

    [Serializable]
    public class VoiceCommandEvent : UnityEvent<VoiceCommand> { }

    [Serializable]
    public class VoiceCommand
    {
        [SerializeField]
        private string _name;

        [SerializeField]
        private List<KeywordSetting> _keywordSettings;

        [SerializeField]
        private VoiceCommandEvent _callback;

        public string Name => _name;

        protected VoiceCommand() { }

        public VoiceCommand(string name)
        {
            _name = name;
        }

        public bool HasMatch(string modelName, string detectedText)
        {
            return _keywordSettings.Any(k => k.IsMatch(modelName, detectedText));
        }

        public void Execute()
        {
            if(_callback != null)
            {
                _callback.Invoke(this);
            }
        }


    }

}


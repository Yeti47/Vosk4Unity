using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Yetibyte.Unity.SpeechRecognition.KeywordDetection
{

    [Serializable]
    public class VoiceCommandEvent : UnityEvent<VoiceCommand, string> { }

    [Serializable]
    public class VoiceCommand
    {
#pragma warning disable CS0649 // BEGIN serializable fields

        [SerializeField]
        private string _name;

        [SerializeField]
        private List<KeywordSetting> _keywordSettings = new List<KeywordSetting>();

        [SerializeField]
        private VoiceCommandEvent _callback;

#pragma warning restore CS0649 // END serializable fields

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

        public void Execute(string detectedText)
        {
            if(_callback != null)
            {
                _callback.Invoke(this, detectedText);
            }
        }


    }

}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Yetibyte.Unity.SpeechRecognition.Util;

namespace Yetibyte.Unity.SpeechRecognition.KeywordDetection
{
    public class KeywordDetector : MonoBehaviour
    {

        #region Constants

        private const string ERR_NO_LISTENER = "No Vosk Listener found. Keyword detection not possible.";

        #endregion

        #region Fields

#pragma warning disable CS0649 // BEGIN serializable fields

        [SerializeField]
        [Tooltip("The Vosk listener to use for speech recognition.")]
        private VoskListener _listener;

        [SerializeField]
        private KeywordDetectionSettings _settings = KeywordDetectionSettings.CreateDefault();

        [SerializeField]
        private List<VoiceCommand> _voiceCommands = new List<VoiceCommand>();

#pragma warning restore CS0649 // END serializable fields

        private bool _hasConsumedPartialResult = false;

        #endregion

        #region Props

        public KeywordDetectionSettings Settings => _settings;

        #endregion

        #region Events

        public event EventHandler<VoiceCommandExecutionEventArgs> ExecutingVoiceCommand;
        public event EventHandler<VoiceCommandExecutionEventArgs> ExecutedVoiceCommand;

        #endregion

        #region Methods

        private bool ProcessPartialResult(VoskPartialResult result)
        {
            if (_settings.Mode != KeywordDetectionMode.Fast)
            {
                return false;
            }

            var matchingVoiceCommands = FindMatchingCommands(result.Text);

            matchingVoiceCommands.ForEach(ExecuteVoiceCommand);

            if (matchingVoiceCommands.Any())
                _hasConsumedPartialResult = true;

            return true;
        }

        private void ExecuteVoiceCommand(VoiceCommand voiceCommand)
        {
            var voiceCommandEventsArgs = new VoiceCommandExecutionEventArgs(voiceCommand);

            OnExecutingVoiceCommand(voiceCommandEventsArgs);
            voiceCommand.Execute();
            OnExecutedVoiceCommand(voiceCommandEventsArgs);
        }

        private bool ProcessFullResult(VoskResult result)
        {
            if (_settings.Mode == KeywordDetectionMode.Off)
            {
                return false;
            }

            IEnumerable<VoskAlternative> candidateAlternatives = 
                result.Alternatives.Where(a => a.Confidence >= _settings.ConfidenceThreshold);

            var matchingVoiceCommands = candidateAlternatives.SelectMany(a => FindMatchingCommands(a.Text));

            matchingVoiceCommands.ForEach(ExecuteVoiceCommand);

            return true;
        }

        protected virtual void OnExecutingVoiceCommand(VoiceCommandExecutionEventArgs eventArgs)
        {
            var handler = ExecutingVoiceCommand;
            handler?.Invoke(this, eventArgs);
        }

        protected virtual void OnExecutedVoiceCommand(VoiceCommandExecutionEventArgs eventArgs)
        {
            var handler = ExecutedVoiceCommand;
            handler?.Invoke(this, eventArgs);
        }

        private void listener_PartialResultFound(object sender, VoskPartialResultEventArgs e)
        {

            ProcessPartialResult(e.PartialResult);

        }

        private void listener_ResultFound(object sender, VoskResultEventArgs e)
        {

            if(!_hasConsumedPartialResult)
            {
                ProcessFullResult(e.Result);
            }

            _hasConsumedPartialResult = false;
        }

        private IEnumerable<VoiceCommand> FindMatchingCommands(string detectedText)
        {
            return _voiceCommands.Where(v => v.HasMatch(_listener.ModelName, detectedText));
        }

        #endregion

        #region Unity Message Methods

        protected virtual void Awake()
        {
            if (_listener == null)
            {
                Debug.LogError(ERR_NO_LISTENER);
                return;
            }

            _listener.ResultFound += listener_ResultFound;
            _listener.PartialResultFound += listener_PartialResultFound;
        }

        #endregion
    }

}


using System;

namespace Yetibyte.Unity.SpeechRecognition.KeywordDetection
{
    public class VoiceCommandExecutionEventArgs : EventArgs
    {

        public VoiceCommand VoiceCommand { get; private set; }
        public string DetectedText { get; private set; }

        public VoiceCommandExecutionEventArgs(VoiceCommand voiceCommand, string detectedText)
        {
            VoiceCommand = voiceCommand;
            DetectedText = detectedText;
        }

    }

}


using System;

namespace Yetibyte.Unity.SpeechRecognition.KeywordDetection
{
    public class VoiceCommandExecutionEventArgs : EventArgs
    {

        public VoiceCommand VoiceCommand { get; private set; }

        public VoiceCommandExecutionEventArgs(VoiceCommand voiceCommand)
        {
            VoiceCommand = voiceCommand;
        }

    }

}


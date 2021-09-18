using UnityEngine;

namespace Yetibyte.Unity.SpeechRecognition.KeywordDetection
{
    public class TestVoiceCommandHandler : MonoBehaviour
    {
        public void OnVoiceCommandExecute(VoiceCommand voiceCommand, string detectedText)
        {
            Debug.Log($"{nameof(TestVoiceCommandHandler)} received voice command '{voiceCommand?.Name}'. Detected Text: '{detectedText}'");
        }
    }

}


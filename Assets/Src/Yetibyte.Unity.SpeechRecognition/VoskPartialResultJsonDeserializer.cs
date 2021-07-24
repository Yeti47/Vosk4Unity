using System;

namespace Yetibyte.Unity.SpeechRecognition.Serialization
{
    public class VoskPartialResultJsonDeserializer : IVoskResultDeserializer<VoskPartialResult>
    {
        [Serializable]
        public class VoskPartialResultData
        {
            public string partial;
        }

        public VoskPartialResult Deserialize(string input)
        {
            VoskPartialResultData partialResultData = UnityEngine.JsonUtility.FromJson<VoskPartialResultData>(input);

            return new VoskPartialResult(partialResultData?.partial ?? string.Empty);

        }

        IVoskResult IVoskResultDeserializer.Deserialize(string input) => this.Deserialize(input);

    }

}

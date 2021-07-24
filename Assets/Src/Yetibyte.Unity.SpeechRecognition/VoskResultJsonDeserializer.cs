using System;
using System.Linq;

namespace Yetibyte.Unity.SpeechRecognition.Serialization
{
    public class VoskResultJsonDeserializer : IVoskResultDeserializer<VoskResult>
    {
        [Serializable]
        public class VoskWordData
        {
            public float conf;
            public double start;
            public double end;
            public string word;

            public VoskWord ToVoskWord() => new VoskWord(this.word, this.start, this.end, this.conf);

        }

        [Serializable]
        public class VoskAlternativeData
        {
            public float confidence;
            public string text;
            public VoskWordData[] result;

            public VoskAlternative ToVoskAlternative()
            {
                return new VoskAlternative(this.confidence, this.text, result?.Select(w => w.ToVoskWord()));
            }
        }

        [Serializable]
        public class VoskResultData
        {
            public VoskAlternativeData[] alternatives;

            public VoskResult ToVoskResult() => new VoskResult(alternatives?.Select(a => a.ToVoskAlternative()));
        }

        public bool UseAlternatives { get; set; } = true;

        public VoskResult Deserialize(string input)
        {
            if(UseAlternatives)
            {
                VoskResultData voskResultData = UnityEngine.JsonUtility.FromJson<VoskResultData>(input);

                return voskResultData?.ToVoskResult();
            }

            VoskAlternativeData voskAlternativeData = UnityEngine.JsonUtility.FromJson<VoskAlternativeData>(input);

            VoskAlternative voskAlternative = voskAlternativeData?.ToVoskAlternative();

            return voskAlternative != null ? new VoskResult(voskAlternative) : VoskResult.Empty;
           
        }

        IVoskResult IVoskResultDeserializer.Deserialize(string input) => this.Deserialize(input);

    }

}

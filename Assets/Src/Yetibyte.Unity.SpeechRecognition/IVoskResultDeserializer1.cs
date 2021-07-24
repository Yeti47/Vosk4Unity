namespace Yetibyte.Unity.SpeechRecognition.Serialization
{
    public interface IVoskResultDeserializer<out T> : IVoskResultDeserializer where T : IVoskResult
    {
        new T Deserialize(string input);
    }

}

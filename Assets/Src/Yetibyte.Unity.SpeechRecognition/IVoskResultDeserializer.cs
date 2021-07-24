namespace Yetibyte.Unity.SpeechRecognition.Serialization
{
    public interface IVoskResultDeserializer
    {
        IVoskResult Deserialize(string input);
    }

}

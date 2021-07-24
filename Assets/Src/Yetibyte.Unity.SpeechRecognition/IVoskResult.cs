namespace Yetibyte.Unity.SpeechRecognition
{
    public interface IVoskResult
    {
        string Text { get; }

        bool IsEmpty { get; }
    }
}

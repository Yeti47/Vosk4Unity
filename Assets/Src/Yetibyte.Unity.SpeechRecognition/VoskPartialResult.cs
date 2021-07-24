namespace Yetibyte.Unity.SpeechRecognition
{
    public class VoskPartialResult : IVoskResult
    {
        public string Text { get; }
        public bool IsEmpty => string.IsNullOrWhiteSpace(Text);

        public VoskPartialResult(string text)
        {
            Text = text?.Trim() ?? string.Empty;
        }

        public override string ToString() => $"{{ Text: \"{Text}\" }}";

    }
}

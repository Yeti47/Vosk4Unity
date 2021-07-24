namespace Yetibyte.Unity.SpeechRecognition.ModelManagement
{
    public class VoskModelImportEventArgs
    {
        public bool IsError { get; private set; }
        public IVoskModelInfo VoskModelInfo { get; private set; }

        public string Message { get; private set; }

        public VoskModelImportEventArgs(IVoskModelInfo voskModelInfo, string message, bool isError = false)
        {
            IsError = isError;
            Message = message ?? string.Empty;
            VoskModelInfo = voskModelInfo;
        }
    }
}

using System;

namespace Yetibyte.Unity.SpeechRecognition.ModelManagement
{
    public interface IVoskModelInfo
    {
        string Category { get; }
        VoskModelDetails Details { get; }
        string Name { get; }
        float Size { get; }
        string Url { get; }

        event EventHandler<VoskModelImportEventArgs> ImportComplete;

        bool Import(string targetPath, out string message);

    }
}

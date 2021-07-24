using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Unity.SpeechRecognition.ModelManagement
{

    public enum VoskModelProviderStatus
    {
        Initial,
        Processing,
        Error,
        Done
    }

    public interface IVoskModelProvider
    {
        string Name { get; }

        string Source { get; }

        VoskModelProviderStatus Status { get; }

        IEnumerable<IVoskModelInfo> Models { get; }
        
        bool FetchModels();

    }
}

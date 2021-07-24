using System;

namespace Yetibyte.Unity.SpeechRecognition
{
    public abstract class VoskListenerEventArgs : EventArgs
    {

        public VoskListener Listener { get; }

        protected VoskListenerEventArgs(VoskListener listener)
        {
            Listener = listener;
        }

    }

    public class VoskPartialResultEventArgs : VoskListenerEventArgs
    {
        public VoskPartialResult PartialResult { get; }

        public VoskPartialResultEventArgs(VoskPartialResult partialResult, VoskListener listener) : base(listener)
        {
            PartialResult = partialResult ?? throw new ArgumentNullException(nameof(partialResult));
        }
    }

    public class VoskResultEventArgs : VoskListenerEventArgs
    {
        public VoskResult Result { get; private set; }

        public VoskResultEventArgs(VoskResult result, VoskListener listener) : base(listener)
        {
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }
    }

}

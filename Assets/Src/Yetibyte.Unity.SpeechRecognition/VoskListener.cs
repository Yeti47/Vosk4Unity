using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vosk;
using System.IO;
using Yetibyte.Unity.Audio;
using Yetibyte.Unity.SpeechRecognition.Serialization;
using Yetibyte.Unity.SpeechRecognition.Util;

namespace Yetibyte.Unity.SpeechRecognition
{

    public class VoskListener : MonoBehaviour
    {
        #region Constants

        public const int DEFAULT_SAMPLE_RATE = 16000;
        public const int DEFAULT_AUDIO_CHUNK_SIZE = 4096;

        private const byte FLOAT_BYTE_SIZE = 4;

        #endregion

        #region Fields

        #pragma warning disable CS0649

        [SerializeField]
        [Header("Speech Recognition Settings")]
        [Tooltip("The Vosk model to use for speech recognition.")]
        [ModelPath]
        private string _modelName;

        [SerializeField]
        [Tooltip("Whether or not the speech recognizer should be executed in word detection mode.")]
        private bool _isWordMode = true;

        [SerializeField]
        [Min(0)]
        private int _maxAlternatives = 0;

        [SerializeField]
        [Header("Behavior")]
        private bool _autoLoadModel = true;

        [SerializeField]
        private bool _autoStart = false;

        [SerializeField]
        [Header("Audio Settings")]
        private int _audioChunkSize = DEFAULT_AUDIO_CHUNK_SIZE;

        [SerializeField]
        private int _sampleRate = DEFAULT_SAMPLE_RATE;

        [SerializeField]
        [Min(1)]
        private int _audioClipBufferSeconds = 1;

        [SerializeField]
        [RecordingDevice]
        private string _listeningDevice = null;

        [SerializeField]
        [Header("Misc.")]
        private VoskListenerDebugOptions _debugOptions = VoskListenerDebugOptions.CreateAllDisabled();

        #pragma warning restore CS0649

        private VoskRecognizer _voskRecognizer;
        private Model _model;

        private AudioClip _microphoneAudio;
        private int _previousMicPosition = 0;

        private VoskPartialResult _previousPartialResult;

        private VoskPartialResultJsonDeserializer _partialResultDeserializer;
        private VoskResultJsonDeserializer _resultDeserializer;

        #endregion

        #region Props

        public IVoskResult LastResult { get; protected set; }
        public VoskPartialResult LastPartialResult { get; protected set; }
        public VoskResult LastFullResult { get; protected set; }

        public VoskListenerDebugOptions DebugOptions
        {
            get
            {
                if (_debugOptions is null)
                    _debugOptions = VoskListenerDebugOptions.CreateAllDisabled();

                return _debugOptions;
            }
        }

        public bool IsListening => Microphone.IsRecording(_listeningDevice);

        public string ListeningDevice
        {
            get => _listeningDevice;
            set => _listeningDevice = value;
        }

        public bool IsReady => _voskRecognizer != null && _model != null;

        public string ModelName => _modelName;

        public string AbsoluteModelPath => System.IO.Path.Combine(Application.dataPath, ModelName);

        public bool IsWordMode
        {
            get => _isWordMode;
            set => _voskRecognizer?.SetWords(_isWordMode = value);
        }

        public int MaxAlternatives
        {
            get => _maxAlternatives;
            set => _voskRecognizer?.SetMaxAlternatives(_maxAlternatives = Math.Max(0, value));
        }

        public int SampleRate => _sampleRate;

        public int AudioChunkSize => _audioChunkSize;

        public bool AutoLoadModel => _autoLoadModel;

        #endregion

        #region Events

        public event EventHandler<VoskResultEventArgs> ResultFound;
        public event EventHandler<VoskPartialResultEventArgs> PartialResultFound;

        #endregion

        #region Unity Message Methods

        protected virtual void Awake()
        {
            _partialResultDeserializer = new VoskPartialResultJsonDeserializer();
            _resultDeserializer = new VoskResultJsonDeserializer();
        }

        protected virtual void Start()
        {
            if (_autoLoadModel)
            {
               LoadModel();

            }
            
            if(IsReady && _autoStart)
            {
                StartListening();
            }
        }

        protected virtual void Update()
        {
            if (IsListening && IsReady)
            {
                int micPos = Microphone.GetPosition(_listeningDevice);

                int sampleDelta = micPos >= _previousMicPosition ? 
                    (micPos - _previousMicPosition) 
                    : (_microphoneAudio.samples * _microphoneAudio.channels - (_previousMicPosition - micPos));

                if (sampleDelta * FLOAT_BYTE_SIZE >= AudioChunkSize)
                {
                    byte[] waveData = _microphoneAudio.GetWavData(sampleDelta, _previousMicPosition);

                    int bufferCount = Mathf.CeilToInt(waveData.Length / (float)AudioChunkSize);

                    bool cancel = false;

                    for (int i = 0; i < bufferCount; i++)
                    {
                        byte[] buffer = waveData.Skip(i * AudioChunkSize).Take(AudioChunkSize).ToArray();

                        string result = string.Empty;

                        if (_voskRecognizer.AcceptWaveform(buffer, buffer.Length))
                        {
                            _previousPartialResult = null;

                            result = _voskRecognizer.Result();

                            _resultDeserializer.UseAlternatives = MaxAlternatives > 0;
                            VoskResult voskResult = _resultDeserializer.Deserialize(result);

                            OnResultFound(voskResult);
                        }
                        else
                        {
                            result = _voskRecognizer.PartialResult();

                            VoskPartialResult partialResult = _partialResultDeserializer.Deserialize(result);

                            if(partialResult != null && partialResult.Text != _previousPartialResult?.Text)
                            {
                                cancel = OnPartialResultFound(partialResult);
                                _previousPartialResult = partialResult;

                                if(cancel)
                                {
                                    result = _voskRecognizer.FinalResult();

                                    _resultDeserializer.UseAlternatives = MaxAlternatives > 0;
                                    VoskResult voskResult = _resultDeserializer.Deserialize(result);

                                    OnResultFound(voskResult);
                                }
                            }

                        }
                    }

                    _previousMicPosition = micPos;
                }
            }
        }

        #endregion

        #region Methods

        [ContextMenu("Toggle Listening")]
        public virtual void ToggleListening()
        {

            bool success = IsListening ? StopListening() : StartListening();

            if (UnityEngine.Application.isEditor)
            {
                if (success)
                    Debug.Log($"Listening mode toggled {(IsListening ? "on" : "off")}.");
                else
                    Debug.LogError("Error toggling listening mode.");
            }

        }

        [ContextMenu("Load Model")]
        public virtual bool LoadModel()
        {
            // If there is already a recognizer and a model, dispose of them.
            UnloadModel();

            try
            {
                if (string.IsNullOrWhiteSpace(ModelName) || string.IsNullOrWhiteSpace(AbsoluteModelPath))
                    throw new Exception("Model not specified.");

                //if (!VoskModelManagerSettings.GetOrCreateSettings().ModelExists(ModelName))
                if (!ModelUtil.ModelPathExists(ModelName))
                    throw new Exception($"Model '{ModelName}' does not exist at path '{ModelUtil.GetAbsoluteModelPathByRelativePath(ModelName)}'.");

                Model model = new Model(AbsoluteModelPath);
                _model = model;

                _voskRecognizer = new VoskRecognizer(_model, _sampleRate);
                _voskRecognizer.SetMaxAlternatives(MaxAlternatives);
                _voskRecognizer.SetWords(IsWordMode);
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);

                Debug.LogError("Vosk model could not be loaded.");

                return false;
            }

            if(DebugOptions.LogModelLoad)
                Debug.Log("Successfully loaded Vosk model.");

            return true;

        }

        public bool StartListening()
        {
            if (IsListening)
                return false;

            _microphoneAudio = Microphone.Start(_listeningDevice, true, _audioClipBufferSeconds, SampleRate);

            if (_microphoneAudio == null)
            {
                Debug.LogError($"An error occurred while trying to start recording from {(string.IsNullOrWhiteSpace(_listeningDevice) ? "default device" : ("device '" + _listeningDevice + "'"))}.");
                return false;
            }

            if (DebugOptions.LogRecording)
                Debug.Log($"Vosk Listener started recording audio from {(string.IsNullOrWhiteSpace(_listeningDevice) ? "default device" : ("device '" + _listeningDevice + "'"))}.");

            return true;
        }

        public bool StopListening()
        {
            if (!IsListening)
                return false;

            Microphone.End(_listeningDevice);

            if(DebugOptions.LogRecording)
                Debug.Log($"Vosk Listener stopped recording audio from {(string.IsNullOrWhiteSpace(_listeningDevice) ? "default device" : ("device '" + _listeningDevice + "'"))}.");

            return true;
        }

        [ContextMenu("Unload Model")]
        public void UnloadModel()
        {
            bool willUnload = _voskRecognizer != null || _model != null;

            _voskRecognizer?.Dispose();
            _voskRecognizer = null;

            _model?.Dispose();
            _model = null;

            if (DebugOptions.LogModelLoad && willUnload)
                Debug.Log("Vosk model unloaded.");

        }

        protected virtual void OnResultFound(VoskResult result)
        {
            if (result is null || result.IsEmpty)
                return;

            if (DebugOptions.LogResults)
                Debug.Log($"Raising ResultFound event. Result: {result}");

            LastResult = LastFullResult = result;

            VoskResultEventArgs voskResultEventArgs = new VoskResultEventArgs(result, this);

            var handler = ResultFound;
            handler?.Invoke(this, voskResultEventArgs);
        }

        protected virtual bool OnPartialResultFound(VoskPartialResult partialResult)
        {
            if (partialResult is null || partialResult.IsEmpty)
                return false;

            if(DebugOptions.LogPartialResults)
                Debug.Log($"Raising PartialResultFound event. Result: {partialResult}");

            LastResult = LastPartialResult = partialResult;

            VoskPartialResultEventArgs voskPartialResultEventArgs = new VoskPartialResultEventArgs(partialResult, this);

            var handler = PartialResultFound;
            handler?.Invoke(this, voskPartialResultEventArgs);

            return voskPartialResultEventArgs.Cancel;
        }

        #endregion

    }
}

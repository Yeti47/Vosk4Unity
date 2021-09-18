using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Yetibyte.Unity.SpeechRecognition.KeywordDetection
{
    [Serializable]
    public class VoiceCommandList : IList<VoiceCommand>
    {
        #region Fields

        [UnityEngine.SerializeField]
        private List<VoiceCommand> _voiceCommands = new List<VoiceCommand>();

        #endregion

        #region Indexers
        public VoiceCommand this[int index] {
            get => index >= 0 && index < Count ? _voiceCommands[index] : throw new IndexOutOfRangeException();
            set
            {
                if (index >= 0 && index < Count)
                    _voiceCommands[index] = value;
                else
                    throw new IndexOutOfRangeException();
            }
        }

        #endregion

        #region Props

        public bool IsEmpty => Count <= 0;

        public int Count => _voiceCommands.Count;

        public bool IsReadOnly => false;

        #endregion

        #region Methods

        public void Add(VoiceCommand item)
        {
            if(item == null)
                throw new ArgumentNullException(nameof(item));

            _voiceCommands.Add(item);
        }

        public void Clear() => _voiceCommands.Clear();

        public bool Contains(VoiceCommand item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return _voiceCommands.Contains(item);
        }

        public void CopyTo(VoiceCommand[] array, int arrayIndex) => _voiceCommands.CopyTo(array, arrayIndex);

        public IEnumerator<VoiceCommand> GetEnumerator() => _voiceCommands.GetEnumerator();

        public int IndexOf(VoiceCommand item) => _voiceCommands.IndexOf(item);

        public void Insert(int index, VoiceCommand item)
        {
            if (index < 0 || index > Count)
                throw new IndexOutOfRangeException();

            _voiceCommands.Insert(index, item ?? throw new ArgumentNullException(nameof(item)));
        }

        public bool Remove(VoiceCommand item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return _voiceCommands.Remove(item);
        }

        public void RemoveAt(int index) => _voiceCommands.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => _voiceCommands.GetEnumerator();

        public IEnumerable<VoiceCommand> FindMatchingCommands(string modelName, string detectedText)
        {
            return _voiceCommands.Where(v => v.HasMatch(modelName, detectedText));
        }

        #endregion
    }

}


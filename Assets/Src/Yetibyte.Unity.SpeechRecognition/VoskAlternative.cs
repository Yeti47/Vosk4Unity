using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Yetibyte.Unity.SpeechRecognition
{
    public class VoskAlternative
    {

        public float Confidence { get; }
        public string Text { get; }
        public IReadOnlyCollection<VoskWord> Words { get; }

        public VoskAlternative(float confidence, string text, IEnumerable<VoskWord> words = null)
        {
            words = words ?? Array.Empty<VoskWord>();

            Confidence = Math.Max(0, confidence);
            Text = text?.Trim() ?? string.Empty;
            Words = new ReadOnlyCollection<VoskWord>(words.ToList());
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("{ ");

            stringBuilder.Append("Text: \"");
            stringBuilder.Append(Text);
            stringBuilder.Append("\", ");

            stringBuilder.Append("Confidence: ");
            stringBuilder.Append(Confidence);
            stringBuilder.Append(", ");

            stringBuilder.Append("Words: ");
            stringBuilder.Append("[ ");

            foreach (var word in Words)
            {
                stringBuilder.Append(word);
                stringBuilder.Append(", ");
            }

            if (Words.Count() > 0)
                stringBuilder.Length -= 2;

            stringBuilder.Append(" ]");

            stringBuilder.Append(" }");

            return stringBuilder.ToString();
        }
    }
}

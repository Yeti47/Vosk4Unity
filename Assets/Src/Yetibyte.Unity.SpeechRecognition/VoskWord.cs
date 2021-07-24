using System;
using System.Collections.Generic;
using System.Text;

namespace Yetibyte.Unity.SpeechRecognition
{
    public class VoskWord
    {
        public const float DEFAULT_CONFIDENCE = 1.0f;

        public string Text { get; }
        public float Confidence { get; }
        public double Start { get; }
        public double End { get; }

        public TimeSpan Duration => TimeSpan.FromSeconds(End - Start);

        public VoskWord(string text, double start, double end, float confidence = DEFAULT_CONFIDENCE)
        {
            Text = text;
            Confidence = confidence;
            Start = start;
            End = end;
        }

        public bool Matches(string word) => !string.IsNullOrWhiteSpace(word) && word.Trim().Equals(Text.Trim(), StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || (obj is string str && this.Matches(str));

        public override int GetHashCode()
        {
            int hashCode = 1940102928;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Text);
            hashCode = hashCode * -1521134295 + Confidence.GetHashCode();
            hashCode = hashCode * -1521134295 + Start.GetHashCode();
            hashCode = hashCode * -1521134295 + End.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(VoskWord voskWord, string word) => ReferenceEquals(voskWord, word) || (!(voskWord is null) && voskWord.Matches(word));

        public static bool operator !=(VoskWord voskWord, string word) => !(voskWord == word);

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

            stringBuilder.Append("Start: ");
            stringBuilder.Append(Start);
            stringBuilder.Append(", ");

            stringBuilder.Append("End: ");
            stringBuilder.Append(End);

            stringBuilder.Append(" }");

            return stringBuilder.ToString();
        }
    }

}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Yetibyte.Unity.SpeechRecognition
{

    public class VoskResult : IVoskResult
    {
        public static VoskResult Empty => new VoskResult(Array.Empty<VoskAlternative>());

        public IReadOnlyCollection<VoskAlternative> Alternatives { get; }
        public VoskAlternative TopResult => Alternatives.FirstOrDefault();
        public string Text => TopResult?.Text ?? string.Empty;

        public bool IsEmpty => string.IsNullOrWhiteSpace(Text);

        public VoskResult(IEnumerable<VoskAlternative> alternatives)
        {
            Alternatives = new ReadOnlyCollection<VoskAlternative>((alternatives ?? Array.Empty<VoskAlternative>()).OrderByDescending(a => a.Confidence).ToList());
        }

        public VoskResult(VoskAlternative singleAlternative) 
            : this(new VoskAlternative[] { singleAlternative ?? throw new ArgumentNullException(nameof(singleAlternative)) })
        {

        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("{ ");
            stringBuilder.Append("Text: \"");
            stringBuilder.Append(Text);
            stringBuilder.Append("\", ");

            stringBuilder.Append(" TopResult: ");
            stringBuilder.Append(TopResult);
            stringBuilder.Append(", ");

            stringBuilder.Append(" Alternatives: ");
            stringBuilder.Append("[ ");

            foreach (var alternative in Alternatives)
            {
                stringBuilder.Append(alternative);
                stringBuilder.Append(", ");
            }

            if (Alternatives.Count() > 0)
                stringBuilder.Length -= 2;

            stringBuilder.Append(" ]");

            stringBuilder.Append(" }");

            return stringBuilder.ToString();

        }

    }
}

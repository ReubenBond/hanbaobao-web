using System;
using System.Text;

namespace DictionaryApp
{
    internal class TermDefinition
    {
        public string Simplified { get; set; }
        public string Traditional { get; set; }
        public string Pinyin { get; set; }
        public string Definition { get; set; }
        public string Classifier { get; set; }
        public string Concept { get; set; }
        public int HskLevel { get; set; }
        public string Topic { get; set; }
        public string ParentTopic { get; set; }
        public string Notes { get; set; }
        public double Frequency { get; set; }
        public long PartOfSpeech { get; set; }

        public string ToDisplayString()
        {
            var result = new StringBuilder();
            if (Simplified is { Length: > 0 }) result.Append($"Simplified: {Simplified} ");
            if (Traditional is { Length: > 0 }) result.Append($"Tradiional: {Traditional} ");
            if (Pinyin is { Length: > 0 }) result.Append($"Pinyin: {Pinyin} ");
            if (Definition is { Length: > 0 }) result.Append($"Definition: {Definition} ");
            if (HskLevel > 0) result.Append($"HSK Level: {HskLevel} ");
            if (Concept is { Length: > 0 }) result.Append($"Concept: {Concept} ");
            if (Classifier is { Length: > 0 }) result.Append($"Classifier: {Classifier} ");
            // if (Topic is { Length: > 0 }) result.Append($"Topic: {Topic} ");
            //if (ParentTopic is { Length: > 0 }) result.Append($"Parent Topic: {ParentTopic} ");
            // if (Notes is { Length: > 0 }) result.Append($"Notes: {Notes} ");
            if (Math.Abs(Frequency) > double.Epsilon) result.Append($"Frequency: {Frequency} ");
            if (PartOfSpeech != 0) foreach (var pos in ReferenceDataService.PartOfSpeechToStrings(PartOfSpeech)) result.Append($" {pos}");
            return result.ToString();
        }
    }
}

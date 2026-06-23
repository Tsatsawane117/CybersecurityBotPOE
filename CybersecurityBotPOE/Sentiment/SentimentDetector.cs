namespace CybersecurityBotPOE.Sentiment
{
    public enum SentimentType { Neutral, Worried, Frustrated, Curious, Happy, Confused }

    public class SentimentResult
    {
        public SentimentType Type { get; init; } = SentimentType.Neutral;
        public string Label { get; init; } = "Neutral";
    }

    /// <summary>
    /// Keyword-based sentiment detector. Returns the dominant emotional tone
    /// found in the user's input so responses can be appropriately empathetic.
    /// </summary>
    public class SentimentDetector
    {
        private static readonly string[] WorriedKeywords = { "worried", "scared", "afraid", "nervous", "anxious", "fear", "terrified", "concerned", "unsafe", "vulnerable", "hacked", "danger", "at risk", "help me", "panicking" };
        private static readonly string[] FrustratedKeywords = { "frustrated", "annoyed", "angry", "upset", "useless", "not working", "terrible", "hate", "ridiculous", "confusing", "i give up", "impossible", "ugh", "argh", "fed up" };
        private static readonly string[] CuriousKeywords = { "curious", "interested", "want to know", "wondering", "how does", "what is", "explain", "tell me about", "what are", "how do", "why is", "i want to learn" };
        private static readonly string[] HappyKeywords = { "great", "awesome", "amazing", "love it", "thank you", "thanks", "helpful", "brilliant", "excellent", "fantastic", "glad", "happy", "perfect", "wonderful" };
        private static readonly string[] ConfusedKeywords = { "confused", "don't understand", "not sure", "what do you mean", "unclear", "lost", "don't get it", "huh", "what?", "i don't follow", "explain again" };

        public SentimentResult Detect(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new SentimentResult { Type = SentimentType.Neutral };

            string lower = input.ToLower();

            if (WorriedKeywords.Any(k => lower.Contains(k))) return new SentimentResult { Type = SentimentType.Worried, Label = "Worried" };
            if (FrustratedKeywords.Any(k => lower.Contains(k))) return new SentimentResult { Type = SentimentType.Frustrated, Label = "Frustrated" };
            if (ConfusedKeywords.Any(k => lower.Contains(k))) return new SentimentResult { Type = SentimentType.Confused, Label = "Confused" };
            if (CuriousKeywords.Any(k => lower.Contains(k))) return new SentimentResult { Type = SentimentType.Curious, Label = "Curious" };
            if (HappyKeywords.Any(k => lower.Contains(k))) return new SentimentResult { Type = SentimentType.Happy, Label = "Happy" };

            return new SentimentResult { Type = SentimentType.Neutral, Label = "Neutral" };
        }
    }
}
namespace CybersecurityBotPOE.Quiz
{
    /// <summary>Represents a single cybersecurity quiz question.</summary>
    public class QuizQuestion
    {
        public string QuestionText { get; init; } = string.Empty;
        public string[] Options { get; init; } = Array.Empty<string>();
        public int CorrectIndex { get; init; }        // 0-based index into Options
        public string Explanation { get; init; } = string.Empty;
        public bool IsTrueFalse { get; init; } = false;
    }
}
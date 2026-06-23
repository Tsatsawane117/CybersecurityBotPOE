namespace CybersecurityBotPOE.ActivityLog
{
    /// <summary>
    /// Records a timestamped log of all significant chatbot actions.
    /// Supports viewing the last 5–10 entries and a full history option.
    /// </summary>
    public class ActivityLogger
    {
        private readonly List<LogEntry> _entries = new();
        private const int DefaultDisplayCount = 10;

        // ─── Log Entry Model ──────────────────────────────────────────────────────

        public class LogEntry
        {
            public DateTime Timestamp { get; init; } = DateTime.Now;
            public string Category { get; init; } = string.Empty;
            public string Description { get; init; } = string.Empty;

            public string Formatted =>
                $"[{Timestamp:HH:mm:ss}]  {Category,-12}  {Description}";
        }

        // ─── Logging Methods ──────────────────────────────────────────────────────

        public void LogTaskAdded(string title, string? reminder = null)
        {
            Add("TASK ADDED",
                $"'{title}'" + (reminder != null ? $" — Reminder: {reminder}" : string.Empty));
        }

        public void LogTaskCompleted(string title)
            => Add("TASK DONE", $"Marked complete: '{title}'");

        public void LogTaskDeleted(string title)
            => Add("TASK DELETED", $"Deleted: '{title}'");

        public void LogReminderSet(string title, string reminder)
            => Add("REMINDER", $"Set for '{title}': {reminder}");

        public void LogQuizStarted()
            => Add("QUIZ", "Quiz session started");

        public void LogQuizCompleted(int score, int total)
            => Add("QUIZ", $"Completed — Score: {score}/{total}  ({score * 100 / total}%)");

        public void LogNlpAction(string intent, string input)
            => Add("NLP", $"Intent '{intent}' detected in: \"{TruncateInput(input)}\"");

        public void LogChatMessage(string topic)
            => Add("CHAT", $"Topic discussed: {topic}");

        public void LogCustom(string category, string description)
            => Add(category, description);

        // ─── Retrieval ────────────────────────────────────────────────────────────

        /// <summary>Returns the most recent N log entries (default 10).</summary>
        public IReadOnlyList<LogEntry> GetRecent(int count = DefaultDisplayCount)
            => _entries.TakeLast(count).Reverse().ToList().AsReadOnly();

        /// <summary>Returns the complete log history.</summary>
        public IReadOnlyList<LogEntry> GetAll()
            => _entries.AsReadOnly();

        /// <summary>Total number of logged entries.</summary>
        public int TotalCount => _entries.Count;

        // ─── Formatted Output ─────────────────────────────────────────────────────

        /// <summary>Builds a formatted string of recent actions for display in the chat.</summary>
        public string GetFormattedSummary(bool showAll = false)
        {
            var entries = showAll ? GetAll() : GetRecent();

            if (!entries.Any())
                return "No activity recorded yet. Start chatting, add tasks, or take the quiz!";

            var lines = new System.Text.StringBuilder();
            lines.AppendLine($"📋 Activity Log ({(showAll ? "full history" : $"last {entries.Count}")} entries)");
            lines.AppendLine(new string('─', 55));

            int i = 1;
            foreach (var entry in entries)
                lines.AppendLine($"  {i++,2}. {entry.Formatted}");

            if (!showAll && TotalCount > DefaultDisplayCount)
                lines.AppendLine($"\n  ... and {TotalCount - DefaultDisplayCount} more. Type 'show full log' to see all.");

            return lines.ToString();
        }

        // ─── Helpers ──────────────────────────────────────────────────────────────

        private void Add(string category, string description)
        {
            _entries.Add(new LogEntry
            {
                Timestamp = DateTime.Now,
                Category = category,
                Description = description
            });
        }

        private static string TruncateInput(string input)
            => input.Length > 50 ? input[..50] + "..." : input;
    }
}
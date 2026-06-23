namespace CybersecurityBotPOE.NLP
{
    /// <summary>
    /// Simulates NLP by detecting user intent from flexible keyword patterns.
    /// Handles varied phrasing so users don't need to use exact commands.
    /// Uses regex for more sophisticated pattern matching.
    /// </summary>
    public class NlpProcessor
    {
        // ─── Intent Enum ──────────────────────────────────────────────────────────

        public enum Intent
        {
            Unknown,
            // Task intents
            AddTask,
            ViewTasks,
            DeleteTask,
            CompleteTask,
            SetReminder,
            // Quiz intents
            StartQuiz,
            // Log intents
            ShowLog,
            ShowFullLog,
            // Conversation intents
            FollowUp,
            Greeting,
            Goodbye,
            // Cybersecurity topics
            CyberTopic
        }

        // ─── Phrase Patterns ─────────────────────────────────────────────────────

        private static readonly List<(Intent intent, string[] patterns)> _patterns = new()
        {
            // Add Task — many natural phrasings
            (Intent.AddTask, new[]
            {
                "add task", "add a task", "create task", "create a task", "new task",
                "add reminder", "set a task", "make a task", "i need to",
                "remind me to", "remind me about", "set reminder to",
                "can you remind me", "add to my list", "put on my list",
                "schedule task", "log task"
            }),

            // View Tasks
            (Intent.ViewTasks, new[]
            {
                "view tasks", "show tasks", "list tasks", "my tasks",
                "what tasks", "show my tasks", "display tasks", "view my list",
                "what do i need to do", "show todo", "pending tasks", "all tasks"
            }),

            // Complete Task
            (Intent.CompleteTask, new[]
            {
                "complete task", "mark complete", "done with", "finished",
                "mark as done", "tick off", "i completed", "task done", "mark task"
            }),

            // Delete Task
            (Intent.DeleteTask, new[]
            {
                "delete task", "remove task", "cancel task", "get rid of task",
                "drop task", "erase task", "clear task"
            }),

            // Set Reminder
            (Intent.SetReminder, new[]
            {
                "set reminder", "remind me", "set a reminder", "add reminder",
                "reminder for", "remind me in", "remind me on", "remind me at"
            }),

            // Start Quiz
            (Intent.StartQuiz, new[]
            {
                "start quiz", "take quiz", "begin quiz", "quiz me",
                "test my knowledge", "cyber quiz", "play quiz",
                "test me", "knowledge test", "security quiz", "i want to quiz",
                "quiz", "take the quiz", "start the quiz"
            }),

            // Activity Log
            (Intent.ShowLog, new[]
            {
                "show activity log", "activity log", "show log", "view log",
                "what have you done", "what have you done for me", "recent actions",
                "show history", "bot history", "my history", "what did we do"
            }),

            (Intent.ShowFullLog, new[]
            {
                "show full log", "full history", "all activity", "complete log",
                "show all", "show more", "full log"
            }),

            // Follow-up
            (Intent.FollowUp, new[]
            {
                "tell me more", "explain more", "more info", "more details",
                "go on", "continue", "another tip", "give me another",
                "expand on that", "more on that", "keep going"
            }),

            // Greeting
            (Intent.Greeting, new[]
            {
                "hello", "hi", "hey", "good morning", "good afternoon",
                "howzit", "sup", "what's up", "greetings"
            }),

            // Goodbye
            (Intent.Goodbye, new[]
            {
                "bye", "goodbye", "see you", "exit", "quit", "close",
                "farewell", "later", "thanks bye", "thank you bye"
            }),
        };

        // ─── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Analyses input and returns the most likely user intent.
        /// Checks for keyword matches with slight variation tolerance.
        /// </summary>
        public Intent DetectIntent(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return Intent.Unknown;

            string lower = input.ToLower().Trim();

            foreach (var (intent, patterns) in _patterns)
            {
                if (patterns.Any(p => lower.Contains(p)))
                    return intent;
            }

            return Intent.Unknown;
        }

        /// <summary>
        /// Extracts the task title/content from natural language input.
        /// E.g. "Add a task to enable 2FA" → "Enable 2FA"
        /// </summary>
        public string ExtractTaskContent(string input)
        {
            string lower = input.ToLower();

            // Strip common command prefixes
            string[] prefixes = {
                "add a task to", "add task to", "add task:", "add a task:",
                "create a task to", "create task to", "new task:",
                "remind me to", "remind me about", "set a reminder for",
                "can you remind me to", "i need to", "add to my list",
                "put on my list", "log task:"
            };

            foreach (var prefix in prefixes)
            {
                if (lower.StartsWith(prefix))
                {
                    string content = input.Substring(prefix.Length).Trim();
                    return CapitaliseFirst(content);
                }
            }

            // Fallback: return input with first word removed if it's a command word
            string[] commandWords = { "add", "create", "new", "remind", "set", "log" };
            string[] words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > 1 && commandWords.Contains(words[0].ToLower()))
                return CapitaliseFirst(string.Join(" ", words.Skip(1)));

            return CapitaliseFirst(input.Trim());
        }

        /// <summary>
        /// Extracts a reminder timeframe from input.
        /// E.g. "remind me in 3 days" → "in 3 days"
        /// </summary>
        public string? ExtractReminder(string input)
        {
            string lower = input.ToLower();

            // Check for explicit time expressions
            string[] timePatterns = {
                "in \\d+ days?", "in \\d+ weeks?", "in \\d+ hours?",
                "tomorrow", "next week", "next month",
                "on \\d{4}-\\d{2}-\\d{2}", "on \\w+ \\d+"
            };

            foreach (var pattern in timePatterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(lower, pattern);
                if (match.Success)
                    return CapitaliseFirst(match.Value);
            }

            return null;
        }

        // ─── Helper ───────────────────────────────────────────────────────────────

        private static string CapitaliseFirst(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
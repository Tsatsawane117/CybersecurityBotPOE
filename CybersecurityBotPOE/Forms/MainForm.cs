using CybersecurityBotPOE.ActivityLog;
using CybersecurityBotPOE.Database;
using CybersecurityBotPOE.Helpers;
using CybersecurityBotPOE.Memory;
using CybersecurityBotPOE.NLP;
using CybersecurityBotPOE.Quiz;
using CybersecurityBotPOE.Responses;
using CybersecurityBotPOE.Sentiment;
using CybersecurityBotPOE.Tasks;

namespace CybersecurityBotPOE.Forms
{

    public class MainForm : Form
    {
        //Controls
        private RichTextBox _chatDisplay = null!;
        private TextBox _inputBox = null!;
        private Button _sendBtn = null!;
        private Button _clearBtn = null!;
        private Button _tasksBtn = null!;
        private Button _quizBtn = null!;
        private Button _logBtn = null!;
        private Label _statusLabel = null!;
        private Label _headerLabel = null!;
        private Panel _headerPanel = null!;
        private Panel _inputPanel = null!;
        private Panel _sidePanel = null!;
        private ListBox _taskListBox = null!;
        private Button _addTaskBtn = null!;
        private Button _completeBtn = null!;
        private Button _deleteTaskBtn = null!;
        private Label _taskPanelLbl = null!;

        //Services 
        private readonly DatabaseManager _db;
        private readonly TaskManager _taskManager;
        private readonly ActivityLogger _logger;
        private readonly NlpProcessor _nlp;
        private readonly ResponseEngine _responseEngine;
        private readonly UserMemory _memory;
        private readonly SentimentDetector _sentiment;
        private readonly AudioHelper _audio;

        // Quiz State 
        private List<QuizQuestion> _quizQuestions = new();
        private int _quizIndex = 0;
        private int _quizScore = 0;
        private bool _quizActive = false;

        // Conversation State 
        private bool _nameCollected = false;
        private bool _awaitingReminder = false;
        private string _pendingTaskTitle = string.Empty;
        private string _pendingTaskDesc = string.Empty;
        private string _lastTopic = string.Empty;

        //  Colours 
        private static readonly Color BgDark = Color.FromArgb(13, 17, 23);
        private static readonly Color BgMid = Color.FromArgb(22, 27, 34);
        private static readonly Color BgPanel = Color.FromArgb(30, 37, 46);
        private static readonly Color AccentCyan = Color.FromArgb(0, 210, 211);
        private static readonly Color AccentGreen = Color.FromArgb(56, 211, 159);
        private static readonly Color AccentYellow = Color.FromArgb(255, 213, 79);
        private static readonly Color AccentRed = Color.FromArgb(255, 100, 100);
        private static readonly Color TextWhite = Color.FromArgb(230, 237, 243);
        private static readonly Color TextGray = Color.FromArgb(139, 148, 158);
        private static readonly Color BotBg = Color.FromArgb(33, 43, 54);
        private static readonly Color UserBg = Color.FromArgb(20, 40, 60);

        public MainForm()
        {
            _db = new DatabaseManager();
            _db.Initialise();
            _taskManager = new TaskManager(_db);
            _logger = new ActivityLogger();
            _nlp = new NlpProcessor();
            _responseEngine = new ResponseEngine();
            _memory = new UserMemory();
            _sentiment = new SentimentDetector();
            _audio = new AudioHelper();

            InitialiseComponents();
            Task.Run(() => _audio.PlayVoiceGreeting());
            ShowWelcome();
        }


        private void InitialiseComponents()
        {
            Text = "🔒 Cybersecurity Awareness Bot — POE";
            Size = new Size(1100, 720);
            MinimumSize = new Size(900, 600);
            BackColor = BgDark;
            ForeColor = TextWhite;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Consolas", 10f);

            //  Header 
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 130,
                BackColor = BgMid,
            };

            _headerLabel = new Label
            {
                Text = GetAsciiLogo(),
                Font = new Font("Consolas", 6.8f, FontStyle.Bold),
                ForeColor = AccentCyan,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
            };
            _headerPanel.Controls.Add(_headerLabel);

            //  Status bar 
            _statusLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 26,
                BackColor = Color.FromArgb(18, 22, 28),
                ForeColor = AccentGreen,
                Font = new Font("Consolas", 8.5f),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0),
                Text = "  🟢  Bot online  |  Type a message, or use the buttons on the right"
            };

            //  Chat display 
            _chatDisplay = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = BgDark,
                ForeColor = TextWhite,
                Font = new Font("Consolas", 10.5f),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Padding = new Padding(10),
                WordWrap = true,
            };

            // Input panel 
            _inputPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                BackColor = BgPanel,
                Padding = new Padding(8, 7, 8, 7),
            };

            _inputBox = new TextBox
            {
                Dock = DockStyle.Fill,
                BackColor = BgMid,
                ForeColor = TextWhite,
                Font = new Font("Consolas", 11f),
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Type a message...",
            };
            _inputBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; ProcessInput(); } };

            _sendBtn = new Button
            {
                Text = "Send  ➤",
                Dock = DockStyle.Right,
                Width = 105,
                BackColor = AccentCyan,
                ForeColor = BgDark,
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
            };
            _sendBtn.FlatAppearance.BorderSize = 0;
            _sendBtn.Click += (s, e) => ProcessInput();

            _clearBtn = new Button
            {
                Text = "Clear",
                Dock = DockStyle.Right,
                Width = 68,
                BackColor = BgMid,
                ForeColor = TextGray,
                Font = new Font("Consolas", 9f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
            };
            _clearBtn.FlatAppearance.BorderSize = 0;
            _clearBtn.Click += (s, e) => _chatDisplay.Clear();

            _inputPanel.Controls.Add(_inputBox);
            _inputPanel.Controls.Add(_sendBtn);
            _inputPanel.Controls.Add(_clearBtn);

            // Side panel (task list + buttons) 
            _sidePanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 280,
                BackColor = BgMid,
                Padding = new Padding(8),
            };

            _taskPanelLbl = new Label
            {
                Text = "📋  Task Assistant",
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                ForeColor = AccentYellow,
                Dock = DockStyle.Top,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
            };

            _taskListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = BgDark,
                ForeColor = TextWhite,
                Font = new Font("Consolas", 9f),
                BorderStyle = BorderStyle.None,
                SelectionMode = SelectionMode.One,
                HorizontalScrollbar = true,
            };

            // Side buttons panel
            var btnPanel = new Panel { Dock = DockStyle.Bottom, Height = 200, BackColor = BgMid };

            _addTaskBtn = MakeSideButton("➕  Add Task", AccentGreen);
            _completeBtn = MakeSideButton("✅  Complete", AccentCyan);
            _deleteTaskBtn = MakeSideButton("🗑️  Delete", AccentRed);
            _quizBtn = MakeSideButton("🎯  Start Quiz", AccentYellow);
            _logBtn = MakeSideButton("📋  View Log", TextGray);

            _addTaskBtn.Dock = DockStyle.Top;
            _completeBtn.Dock = DockStyle.Top;
            _deleteTaskBtn.Dock = DockStyle.Top;
            _quizBtn.Dock = DockStyle.Top;
            _logBtn.Dock = DockStyle.Top;

            _addTaskBtn.Click += AddTask_Click;
            _completeBtn.Click += CompleteTask_Click;
            _deleteTaskBtn.Click += DeleteTask_Click;
            _quizBtn.Click += (s, e) => StartQuiz();
            _logBtn.Click += (s, e) => ShowActivityLog(false);

            // Add in reverse order for DockStyle.Top stacking
            btnPanel.Controls.Add(_logBtn);
            btnPanel.Controls.Add(_quizBtn);
            btnPanel.Controls.Add(_deleteTaskBtn);
            btnPanel.Controls.Add(_completeBtn);
            btnPanel.Controls.Add(_addTaskBtn);

            _sidePanel.Controls.Add(_taskListBox);
            _sidePanel.Controls.Add(btnPanel);
            _sidePanel.Controls.Add(_taskPanelLbl);

            //  Assemble 
            Controls.Add(_chatDisplay);
            Controls.Add(_statusLabel);
            Controls.Add(_inputPanel);
            Controls.Add(_sidePanel);
            Controls.Add(_headerPanel);

            RefreshTaskList();
            _inputBox.Select();
        }

        private Button MakeSideButton(string text, Color back)
        {
            var btn = new Button
            {
                Text = text,
                Height = 36,
                BackColor = back,
                ForeColor = BgDark,
                Font = new Font("Consolas", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 2, 0, 2),
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }


        private void ShowWelcome()
        {
            BotSay(
                "Welcome to the Cybersecurity Awareness Bot!\n" +
                "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                "I'm your personal cybersecurity assistant. I can:\n" +
                "  🔒  Answer cybersecurity questions\n" +
                "  📋  Help you manage security tasks\n" +
                "  🎯  Quiz your cybersecurity knowledge\n" +
                "  📜  Keep an activity log of what we do\n\n" +
                "Before we begin — what's your name?");
        }


        private void ProcessInput()
        {
            string raw = _inputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(raw)) return;
            _inputBox.Clear();
            UserSay(raw);

            // Name collection
            if (!_nameCollected)
            {
                _memory.SetName(raw);
                _nameCollected = true;
                _logger.LogCustom("SESSION", $"User identified as: {_memory.Name}");
                BotSay(
                    $"Great to meet you, {_memory.Name}! 😊\n\n" +
                    "You can chat with me, add tasks, start the quiz, or type 'show log' to see what we've done.\n" +
                    "Try: 'tell me about phishing', 'add a task to enable 2FA', or 'start quiz'.");
                return;
            }

            // Quiz mode intercepts all input
            if (_quizActive)
            {
                HandleQuizAnswer(raw);
                return;
            }

            // Awaiting reminder for a task
            if (_awaitingReminder)
            {
                HandleReminderResponse(raw);
                return;
            }

            // NLP intent detection
            var intent = _nlp.DetectIntent(raw);
            _logger.LogNlpAction(intent.ToString(), raw);

            switch (intent)
            {
                case NlpProcessor.Intent.AddTask: HandleAddTask(raw); break;
                case NlpProcessor.Intent.ViewTasks: HandleViewTasks(); break;
                case NlpProcessor.Intent.CompleteTask: HandleCompleteTaskNlp(raw); break;
                case NlpProcessor.Intent.DeleteTask: HandleDeleteTaskNlp(raw); break;
                case NlpProcessor.Intent.SetReminder: HandleSetReminderNlp(raw); break;
                case NlpProcessor.Intent.StartQuiz: StartQuiz(); break;
                case NlpProcessor.Intent.ShowLog: ShowActivityLog(false); break;
                case NlpProcessor.Intent.ShowFullLog: ShowActivityLog(true); break;
                case NlpProcessor.Intent.FollowUp: HandleFollowUp(); break;
                case NlpProcessor.Intent.Goodbye:
                    BotSay($"Goodbye, {_memory.Name}! Stay safe online. 🔒");
                    break;
                default:
                    HandleCyberChat(raw);
                    break;
            }
        }

        // TASK HANDLING 

        private void HandleAddTask(string input)
        {
            string title = _nlp.ExtractTaskContent(input);
            if (string.IsNullOrWhiteSpace(title) || title.Length < 3)
            {
                BotSay($"What would you like to add as a task, {_memory.Name}? " +
                       "For example: 'Add a task to enable two-factor authentication'.");
                return;
            }

            string desc = GenerateTaskDescription(title);
            _pendingTaskTitle = title;
            _pendingTaskDesc = desc;

            string? reminder = _nlp.ExtractReminder(input);
            if (reminder != null)
            {
                FinaliseTask(title, desc, reminder);
            }
            else
            {
                _awaitingReminder = true;
                BotSay($"Task added: '{title}'\n\n" +
                       $"Description: {desc}\n\n" +
                       "Would you like to set a reminder for this task?\n" +
                       "Type a timeframe (e.g. 'in 3 days', 'tomorrow', 'next week') or 'no' to skip.");
            }
        }

        private void HandleReminderResponse(string input)
        {
            _awaitingReminder = false;
            string lower = input.ToLower().Trim();

            if (lower == "no" || lower == "skip" || lower == "none" || lower == "n")
            {
                FinaliseTask(_pendingTaskTitle, _pendingTaskDesc, null);
            }
            else
            {
                string reminder = _nlp.ExtractReminder(input) ?? input;
                FinaliseTask(_pendingTaskTitle, _pendingTaskDesc, reminder);
            }
        }

        private void FinaliseTask(string title, string desc, string? reminder)
        {
            var task = _taskManager.Add(title, desc, reminder);
            _logger.LogTaskAdded(title, reminder);
            RefreshTaskList();

            string msg = $"✅ Task saved!\n\n  📌 {title}\n  {desc}";
            if (reminder != null) msg += $"\n  🔔 Reminder: {reminder}";
            BotSay(msg);
        }

        private void HandleViewTasks()
        {
            var tasks = _taskManager.Tasks;
            if (!tasks.Any())
            {
                BotSay($"You have no tasks yet, {_memory.Name}. " +
                       "Try saying 'Add a task to review my passwords'!");
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"📋 Your tasks, {_memory.Name} ({_taskManager.PendingCount} pending):\n");
            foreach (var t in tasks)
            {
                sb.AppendLine($"  [{t.Id}]  {(t.IsComplete ? "✅" : "⬜")}  {t.Title}");
                sb.AppendLine($"        {t.Description}");
                if (t.Reminder != null) sb.AppendLine($"        🔔 Reminder: {t.Reminder}");
                sb.AppendLine();
            }
            BotSay(sb.ToString());
        }

        private void HandleCompleteTaskNlp(string input)
        {
            BotSay("Which task would you like to mark complete?\n" +
                   "Click the task in the list on the right, then press '✅ Complete'.\n" +
                   "Or type the task ID number.");
        }

        private void HandleDeleteTaskNlp(string input)
        {
            BotSay("Which task would you like to delete?\n" +
                   "Click the task in the list on the right, then press '🗑️ Delete'.");
        }

        private void HandleSetReminderNlp(string input)
        {
            string? reminder = _nlp.ExtractReminder(input);
            BotSay("Which task would you like to set a reminder for?\n" +
                   "Click the task in the list on the right, then use '✅ Complete' → or type:\n" +
                   $"'Set reminder for task [ID] to {reminder ?? "in X days'"}");
        }

        private void AddTask_Click(object? sender, EventArgs e)
        {
            using var dlg = new AddTaskDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var task = _taskManager.Add(dlg.TaskTitle, dlg.TaskDescription, dlg.TaskReminder);
                _logger.LogTaskAdded(dlg.TaskTitle, dlg.TaskReminder);
                RefreshTaskList();
                BotSay($"✅ Task added: '{dlg.TaskTitle}'" +
                       (dlg.TaskReminder != null ? $"\n🔔 Reminder set: {dlg.TaskReminder}" : string.Empty));
            }
        }

        private void CompleteTask_Click(object? sender, EventArgs e)
        {
            if (_taskListBox.SelectedItem is not string selected) return;
            var task = GetTaskFromListItem(selected);
            if (task == null) return;

            if (_taskManager.MarkComplete(task.Id))
            {
                _logger.LogTaskCompleted(task.Title);
                RefreshTaskList();
                BotSay($"✅ '{task.Title}' marked as complete! Great work, {_memory.Name}!");
            }
        }

        private void DeleteTask_Click(object? sender, EventArgs e)
        {
            if (_taskListBox.SelectedItem is not string selected) return;
            var task = GetTaskFromListItem(selected);
            if (task == null) return;

            var confirm = MessageBox.Show(
                $"Delete task: '{task.Title}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes && _taskManager.Delete(task.Id))
            {
                _logger.LogTaskDeleted(task.Title);
                RefreshTaskList();
                BotSay($"🗑️ Task '{task.Title}' has been deleted.");
            }
        }

        private void RefreshTaskList()
        {
            _taskListBox.Items.Clear();
            foreach (var t in _taskManager.Tasks)
                _taskListBox.Items.Add(t.DisplayText + $"  [ID:{t.Id}]");

            UpdateStatus();
        }

        private CyberTask? GetTaskFromListItem(string item)
        {
            // Extract ID from end of display string "[ID:X]"
            var match = System.Text.RegularExpressions.Regex.Match(item, @"\[ID:(\d+)\]");
            if (!match.Success) return null;
            int id = int.Parse(match.Groups[1].Value);
            return _taskManager.Tasks.FirstOrDefault(t => t.Id == id);
        }

        private static string GenerateTaskDescription(string title)
        {
            string lower = title.ToLower();
            if (lower.Contains("2fa") || lower.Contains("two-factor") || lower.Contains("authentication"))
                return "Enable two-factor authentication to add an extra layer of security to your accounts.";
            if (lower.Contains("password"))
                return "Review and update your passwords using a strong, unique combination for each account.";
            if (lower.Contains("backup"))
                return "Create a secure backup of your important data following the 3-2-1 backup rule.";
            if (lower.Contains("update") || lower.Contains("patch"))
                return "Apply the latest security updates and patches to your software and operating system.";
            if (lower.Contains("vpn"))
                return "Set up and configure a VPN to protect your internet traffic on public networks.";
            if (lower.Contains("privacy"))
                return "Review and tighten privacy settings on your accounts and devices.";
            return $"Complete the cybersecurity task: {title}.";
        }

        //  QUIZ FEATURE 

        private void StartQuiz()
        {
            _quizQuestions = QuizBank.GetShuffled(10);
            _quizIndex = 0;
            _quizScore = 0;
            _quizActive = true;
            _logger.LogQuizStarted();

            BotSay(
                $"🎯 Cybersecurity Quiz — Let's go, {_memory.Name}!\n" +
                "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                $"10 questions | Multiple choice & True/False\n" +
                "Type the letter of your answer (A, B, C, or D)\n\n" +
                "Starting now...");

            ShowNextQuestion();
        }

        private void ShowNextQuestion()
        {
            if (_quizIndex >= _quizQuestions.Count)
            {
                EndQuiz();
                return;
            }

            var q = _quizQuestions[_quizIndex];
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Question {_quizIndex + 1} of {_quizQuestions.Count}:");
            sb.AppendLine($"{'─',0}{new string('─', 45)}");
            sb.AppendLine(q.QuestionText);
            sb.AppendLine();
            foreach (var opt in q.Options)
                sb.AppendLine($"  {opt}");

            BotSay(sb.ToString());
        }

        private void HandleQuizAnswer(string input)
        {
            var q = _quizQuestions[_quizIndex];
            char answer = input.Trim().ToUpper().FirstOrDefault();

            int userIndex = answer switch
            {
                'A' => 0,
                'B' => 1,
                'C' => 2,
                'D' => 3,
                _ => -1
            };

            if (userIndex == -1)
            {
                BotSay("Please answer with A, B, C, or D.");
                return;
            }

            bool correct = userIndex == q.CorrectIndex;
            if (correct) _quizScore++;

            string correctLetter = (char)('A' + q.CorrectIndex) + "";
            string feedback = correct
                ? $"✅ Correct! Well done, {_memory.Name}!\n\n💡 {q.Explanation}"
                : $"❌ Not quite. The correct answer was {correctLetter}) {q.Options[q.CorrectIndex].Substring(3)}\n\n💡 {q.Explanation}";

            BotSay(feedback);
            _quizIndex++;

            if (_quizIndex < _quizQuestions.Count)
            {
                // Short pause effect by showing next question in same call
                System.Threading.Thread.Sleep(100);
                ShowNextQuestion();
            }
            else
            {
                EndQuiz();
            }
        }

        private void EndQuiz()
        {
            _quizActive = false;
            int pct = _quizScore * 100 / _quizQuestions.Count;
            _logger.LogQuizCompleted(_quizScore, _quizQuestions.Count);

            string grade = pct switch
            {
                >= 90 => "🏆 Outstanding! You're a cybersecurity expert!",
                >= 70 => "🌟 Great job! You're a cybersecurity pro!",
                >= 50 => "👍 Good effort! Keep learning to stay safe online.",
                _ => "📚 Keep learning — cybersecurity knowledge is your best defence!"
            };

            BotSay(
                $"Quiz complete!\n" +
                $"━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                $"Score: {_quizScore}/{_quizQuestions.Count}  ({pct}%)\n\n" +
                $"{grade}\n\n" +
                "Type 'start quiz' to try again, or keep chatting!");
        }



        private void ShowActivityLog(bool showAll)
        {
            string summary = _logger.GetFormattedSummary(showAll);
            BotSay(summary);
        }

        //  CYBERSECURITY CHAT 

        private void HandleCyberChat(string input)
        {
            var sentimentResult = _sentiment.Detect(input);
            string prefix = BuildSentimentPrefix(sentimentResult);

            string response = _responseEngine.GetResponse(input, _memory.Name, _memory);

            string? topic = _responseEngine.GetLastDetectedTopic();
            if (topic != null)
            {
                _lastTopic = topic;
                _memory.SetFavouriteTopic(topic);
                _logger.LogChatMessage(topic);
            }

            string full = string.IsNullOrEmpty(prefix) ? response : prefix + "\n\n" + response;
            BotSay(full);
        }

        private void HandleFollowUp()
        {
            if (string.IsNullOrEmpty(_lastTopic))
            {
                BotSay($"I'm not sure which topic to expand on, {_memory.Name}. " +
                       "Could you mention the topic again?");
                return;
            }
            string more = _responseEngine.GetFollowUp(_lastTopic, _memory.Name);
            BotSay($"More on {_lastTopic}:\n\n{more}");
        }

        private string BuildSentimentPrefix(SentimentResult s) => s.Type switch
        {
            SentimentType.Worried => $"I completely understand feeling worried, {_memory.Name} — let me help put your mind at ease. 💙",
            SentimentType.Frustrated => $"I hear you, {_memory.Name} — let's work through this together. 💪",
            SentimentType.Curious => $"Great curiosity, {_memory.Name}! That's the right mindset. 🌟",
            SentimentType.Happy => $"Love the energy, {_memory.Name}! 😄",
            SentimentType.Confused => $"No worries, {_memory.Name} — I'll explain as clearly as possible. 🙂",
            _ => string.Empty
        };



        private void UserSay(string text)
        {
            _chatDisplay.SelectionStart = _chatDisplay.TextLength;
            _chatDisplay.SelectionColor = AccentYellow;
            _chatDisplay.AppendText($"\n  [{DateTime.Now:HH:mm}]  {_memory.Name ?? "You"}\n");
            _chatDisplay.SelectionColor = TextWhite;
            _chatDisplay.SelectionBackColor = UserBg;
            _chatDisplay.AppendText($"  {text}\n");
            _chatDisplay.SelectionBackColor = BgDark;
            _chatDisplay.AppendText("\n");
            ScrollToBottom();
        }

        private void BotSay(string text)
        {
            _chatDisplay.SelectionStart = _chatDisplay.TextLength;
            _chatDisplay.SelectionColor = AccentCyan;
            _chatDisplay.AppendText($"\n  [{DateTime.Now:HH:mm}]  🤖 CyberBot\n");
            _chatDisplay.SelectionColor = TextWhite;
            _chatDisplay.SelectionBackColor = BotBg;
            foreach (string line in text.Split('\n'))
                _chatDisplay.AppendText($"  {line}\n");
            _chatDisplay.SelectionBackColor = BgDark;
            _chatDisplay.AppendText("\n");
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            _chatDisplay.SelectionStart = _chatDisplay.TextLength;
            _chatDisplay.ScrollToCaret();
        }

        private void UpdateStatus()
        {
            _statusLabel.Text =
                $"  🟢  {_memory.Name}  |  Tasks: {_taskManager.Tasks.Count}  ({_taskManager.PendingCount} pending)  |  Log entries: {_logger.TotalCount}";
        }


        private static string GetAsciiLogo() =>
            "  ██████╗██╗   ██╗██████╗ ███████╗██████╗      ██████╗  ██████╗ ████████╗\r\n" +
            " ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗     ██╔══██╗██╔═══██╗╚══██╔══╝\r\n" +
            " ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝     ██████╔╝██║   ██║   ██║   \r\n" +
            " ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗     ██╔══██╗██║   ██║   ██║   \r\n" +
            " ╚██████╗   ██║   ██████╔╝███████╗██║  ██║ ███╗██████╔╝╚██████╔╝   ██║   \r\n" +
            "  ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝ ╚══╝╚═════╝  ╚═════╝    ╚═╝  \r\n" +
            "     Cybersecurity Awareness Bot — POE  |  Keeping You Safe Online ";
    }
}
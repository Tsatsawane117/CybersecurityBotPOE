namespace CybersecurityBotPOE.Forms
{
    /// <summary>
    /// A clean dialog form that collects task title, description,
    /// and optional reminder when the user clicks the Add Task button.
    /// </summary>
    public class AddTaskDialog : Form
    {
        // ── Controls ──────────────────────────────────────────────────────────────
        private TextBox _titleBox = null!;
        private TextBox _descBox = null!;
        private TextBox _reminderBox = null!;
        private Button _saveBtn = null!;
        private Button _cancelBtn = null!;
        private Label _titleLbl = null!;
        private Label _descLbl = null!;
        private Label _reminderLbl = null!;

        // ── Colours ───────────────────────────────────────────────────────────────
        private static readonly Color BgDark = Color.FromArgb(22, 27, 34);
        private static readonly Color BgField = Color.FromArgb(13, 17, 23);
        private static readonly Color AccentCyan = Color.FromArgb(0, 210, 211);
        private static readonly Color TextWhite = Color.FromArgb(230, 237, 243);
        private static readonly Color TextGray = Color.FromArgb(139, 148, 158);

        // ── Output Properties ─────────────────────────────────────────────────────
        public string TaskTitle { get; private set; } = string.Empty;
        public string TaskDescription { get; private set; } = string.Empty;
        public string? TaskReminder { get; private set; }

        public AddTaskDialog()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            Text = "Add New Task";
            Size = new Size(480, 320);
            BackColor = BgDark;
            ForeColor = TextWhite;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Consolas", 10f);

            // Title
            _titleLbl = MakeLabel("Task Title:", 20, 20);
            _titleBox = MakeTextBox(20, 45, 420, false);
            _titleBox.PlaceholderText = "e.g. Enable two-factor authentication";

            // Description
            _descLbl = MakeLabel("Description (optional):", 20, 85);
            _descBox = MakeTextBox(20, 110, 420, true);
            _descBox.PlaceholderText = "Brief description of the task...";

            // Reminder
            _reminderLbl = MakeLabel("Reminder (optional):", 20, 175);
            _reminderBox = MakeTextBox(20, 200, 420, false);
            _reminderBox.PlaceholderText = "e.g. in 3 days, tomorrow, next week";

            // Buttons
            _saveBtn = new Button
            {
                Text = "Save Task",
                Location = new Point(235, 245),
                Size = new Size(105, 34),
                BackColor = AccentCyan,
                ForeColor = BgDark,
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
            };
            _saveBtn.FlatAppearance.BorderSize = 0;
            _saveBtn.Click += SaveBtn_Click;

            _cancelBtn = new Button
            {
                Text = "Cancel",
                Location = new Point(340, 245),
                Size = new Size(100, 34),
                BackColor = Color.FromArgb(40, 47, 56),
                ForeColor = TextGray,
                Font = new Font("Consolas", 10f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
            };
            _cancelBtn.FlatAppearance.BorderSize = 0;
            _cancelBtn.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[]
            {
                _titleLbl, _titleBox,
                _descLbl,  _descBox,
                _reminderLbl, _reminderBox,
                _saveBtn, _cancelBtn
            });

            AcceptButton = _saveBtn;
            CancelButton = _cancelBtn;
        }

        private void SaveBtn_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_titleBox.Text))
            {
                MessageBox.Show("Please enter a task title.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _titleBox.Focus();
                return;
            }

            TaskTitle = _titleBox.Text.Trim();
            TaskDescription = string.IsNullOrWhiteSpace(_descBox.Text)
                ? $"Complete the cybersecurity task: {TaskTitle}"
                : _descBox.Text.Trim();
            TaskReminder = string.IsNullOrWhiteSpace(_reminderBox.Text)
                ? null
                : _reminderBox.Text.Trim();

            DialogResult = DialogResult.OK;
            Close();
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private Label MakeLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = TextGray,
                Font = new Font("Consolas", 9f),
            };
        }

        private TextBox MakeTextBox(int x, int y, int width, bool multiline)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, multiline ? 55 : 28),
                BackColor = BgField,
                ForeColor = TextWhite,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 10f),
                Multiline = multiline,
            };
        }
    }
}
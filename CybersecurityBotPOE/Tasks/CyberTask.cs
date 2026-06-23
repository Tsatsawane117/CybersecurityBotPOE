namespace CybersecurityBotPOE.Tasks
{

    public class CyberTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Reminder { get; set; }
        public bool IsComplete { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string DisplayText =>
            $"[{(IsComplete ? "✅" : "⬜")}]  {Title}" +
            (Reminder != null ? $"  🔔 {Reminder}" : string.Empty);
    }
}
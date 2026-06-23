namespace CybersecurityBotPOE.Memory
{
    /// <summary>
    /// Stores information the user shares during the session so the chatbot
    /// can recall and personalise responses throughout the conversation.
    /// </summary>
    public class UserMemory
    {
        public string Name { get; private set; } = "there";
        public string? FavouriteTopic { get; private set; }
        public int MessageCount { get; private set; } = 0;

        private readonly List<string> _topicHistory = new();

        public void SetName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
                Name = char.ToUpper(name.Trim()[0]) + name.Trim().Substring(1).ToLower();
        }

        public void SetFavouriteTopic(string topic)
        {
            FavouriteTopic = topic;
            if (!_topicHistory.Contains(topic))
                _topicHistory.Add(topic);
        }

        public void IncrementMessageCount() => MessageCount++;

        public string GetMemoryRecall()
        {
            if (FavouriteTopic != null && MessageCount > 3)
                return $"As someone interested in {FavouriteTopic}, you might find this especially useful.";
            return string.Empty;
        }

        public IReadOnlyList<string> GetTopicHistory() => _topicHistory.AsReadOnly();
    }
}
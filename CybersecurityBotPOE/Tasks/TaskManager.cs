using CybersecurityBotPOE.Database;

namespace CybersecurityBotPOE.Tasks
{

    public class TaskManager
    {
        private readonly DatabaseManager _db;
        private readonly List<CyberTask> _tasks = new();

        public TaskManager(DatabaseManager db)
        {
            _db = db;
            Refresh();
        }

        //Public Properties 

        public IReadOnlyList<CyberTask> Tasks => _tasks.AsReadOnly();

        // Operations 

        public CyberTask Add(string title, string description, string? reminder = null)
        {
            var task = new CyberTask
            {
                Title = title,
                Description = description,
                Reminder = reminder
            };

            int newId = _db.AddTask(task);
            task.Id = newId;
            _tasks.Insert(0, task);
            return task;
        }

        public bool MarkComplete(int taskId)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return false;

            if (_db.MarkComplete(taskId))
            {
                task.IsComplete = true;
                return true;
            }
            return false;
        }

        public bool Delete(int taskId)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return false;

            if (_db.DeleteTask(taskId))
            {
                _tasks.Remove(task);
                return true;
            }
            return false;
        }

        public bool UpdateReminder(int taskId, string reminder)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return false;

            if (_db.UpdateReminder(taskId, reminder))
            {
                task.Reminder = reminder;
                return true;
            }
            return false;
        }

        public void Refresh()
        {
            _tasks.Clear();
            _tasks.AddRange(_db.LoadAllTasks());
        }

        public int PendingCount => _tasks.Count(t => !t.IsComplete);
    }
}
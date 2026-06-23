using MySql.Data.MySqlClient;
using CybersecurityBotPOE.Tasks;

namespace CybersecurityBotPOE.Database
{
    public class DatabaseManager
    {
        private const string ConnectionString = 
            "Server=localhost;Database=cybersecurity_bot;Uid=root;Pwd=Rialivhuwa@2;CharSet=utf8mb4;";

        public void Initialise()
        {
            try
            {
                // First connect without specifying DB to create it if missing
                string baseConn = "Server=localhost;Uid=root;Pwd=Rialivhuwa@2;CharSet=utf8mb4;";
                using var conn = new MySqlConnection(baseConn);
                conn.Open();

                using var createDb = new MySqlCommand(
                    "CREATE DATABASE IF NOT EXISTS cybersecurity_bot CHARACTER SET utf8mb4;", conn);
                createDb.ExecuteNonQuery();

                conn.ChangeDatabase("cybersecurity_bot");

                string createTable = @"
                    CREATE TABLE IF NOT EXISTS tasks (
                        id          INT AUTO_INCREMENT PRIMARY KEY,
                        title       VARCHAR(255)  NOT NULL,
                        description TEXT          NOT NULL,
                        reminder    VARCHAR(255)  NULL,
                        is_complete TINYINT(1)    NOT NULL DEFAULT 0,
                        created_at  DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP
                    );";

                using var cmd = new MySqlCommand(createTable, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Database initialisation failed:\n{ex.Message}\n\n" +
                    "The app will run without database support. " +
                    "Please check MySQL is running and update the connection string in DatabaseManager.cs.",
                    "Database Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // Add Task 

        public int AddTask(CyberTask task)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();

                string sql = @"INSERT INTO tasks (title, description, reminder)
                               VALUES (@title, @desc, @reminder);
                               SELECT LAST_INSERT_ID();";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@title", task.Title);
                cmd.Parameters.AddWithValue("@desc", task.Description);
                cmd.Parameters.AddWithValue("@reminder", task.Reminder ?? (object)DBNull.Value);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ShowDbError("adding task", ex);
                return -1;
            }
        }

        //Load All Tasks 

        public List<CyberTask> LoadAllTasks()
        {
            var list = new List<CyberTask>();
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();

                string sql = "SELECT id, title, description, reminder, is_complete, created_at FROM tasks ORDER BY created_at DESC;";
                using var cmd = new MySqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new CyberTask
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        Description = reader.GetString("description"),
                        Reminder = reader.IsDBNull(reader.GetOrdinal("reminder"))
                                        ? null
                                        : reader.GetString("reminder"),
                        IsComplete = reader.GetBoolean("is_complete"),
                        CreatedAt = reader.GetDateTime("created_at")
                    });
                }
            }
            catch (Exception ex)
            {
                ShowDbError("loading tasks", ex);
            }
            return list;
        }

        //Mark Complete 

        public bool MarkComplete(int taskId)
        {
            return ExecuteNonQuery(
                "UPDATE tasks SET is_complete = 1 WHERE id = @id;",
                ("@id", taskId));
        }

        //Delete Task 

        public bool DeleteTask(int taskId)
        {
            return ExecuteNonQuery(
                "DELETE FROM tasks WHERE id = @id;",
                ("@id", taskId));
        }

        //Update Reminder

        public bool UpdateReminder(int taskId, string reminder)
        {
            return ExecuteNonQuery(
                "UPDATE tasks SET reminder = @reminder WHERE id = @id;",
                ("@reminder", reminder), ("@id", taskId));
        }

        //Helpers

        private bool ExecuteNonQuery(string sql, params (string name, object value)[] parameters)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new MySqlCommand(sql, conn);
                foreach (var (name, value) in parameters)
                    cmd.Parameters.AddWithValue(name, value);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                ShowDbError("updating database", ex);
                return false;
            }
        }

        private static void ShowDbError(string action, Exception ex)
        {
            MessageBox.Show($"Error {action}:\n{ex.Message}",
                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
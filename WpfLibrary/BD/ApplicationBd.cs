using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CAT_COMPANY
{
    public class ApplicationBd
    {
        public class Application
        {
            public int ApplicationID { get; set; }
            public string DepartmentName { get; set; }
            public DateTime Date {  get; set; }
        }

        private readonly string connectionString;
        public List<Application> ApplicationList { get; private set; }

        public ApplicationBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            ApplicationList = GetApplicationData();
        }

        private List<Application> GetApplicationData()
        {
            List<Application> applicationList = new List<Application>();
            string query = "SELECT * FROM Application";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Application application = new Application
                            {
                                ApplicationID = Convert.ToInt32(reader["ApplicationID"]),
                                Date = Convert.ToDateTime(reader["Date"]),
                                DepartmentName = Convert.ToString(reader["DepartmentName"])
                            };

                            applicationList.Add(application);
                        }
                    }
                }
            }

            return applicationList;
        }

        public void AddApplication(Application newApplication)
        {
            string insertQuery = "INSERT INTO Application (DepartmentName, Date) VALUES ( @DepartmentName, @Date)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Date", newApplication.Date);
                        command.Parameters.AddWithValue("@DepartmentName", newApplication.DepartmentName);


                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newApplication.ApplicationID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                ApplicationList.Add(newApplication);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении заявки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateApplication(Application updatedApplication)
        {
            string updateQuery = "UPDATE Application SET Date = @Date, DepartmentName = @DepartmentName WHERE ApplicationID = @Id";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Date", updatedApplication.Date);
                        command.Parameters.AddWithValue("@DepartmentName", updatedApplication.DepartmentName);
                        command.Parameters.AddWithValue("@Id", updatedApplication.ApplicationID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Заявка с указанным Id не найдена.");
                    }
                    int index = ApplicationList.FindIndex(c => c.ApplicationID == updatedApplication.ApplicationID);
                    ApplicationList[index] = updatedApplication;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении заявки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteApplication(int ApplicationID)
        {
            string deleteQuery = "DELETE FROM Application WHERE ApplicationID = @Id";
            int index = ApplicationList.FindIndex(application => application.ApplicationID == ApplicationID);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", ApplicationID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        ApplicationList.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении заявки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CAT_COMPANY
{
    public class ApplicationIngBd
    {
        public class ApplicationIng
        {

            public int ApplicationIngID { get; set; }
            public int ApplicationID { get; set; }
            public int IngredientID { get; set; }
            public double Quantity {  get; set; }
        }

        private readonly string connectionString;
        public List<ApplicationIng> ApplicationIngList { get; private set; }

        public ApplicationIngBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            ApplicationIngList = GetApplicationIngData();
        }

        private List<ApplicationIng> GetApplicationIngData()
        {
            List<ApplicationIng> applicationList = new List<ApplicationIng>();
            string query = "SELECT * FROM ApplicationIngredient";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ApplicationIng application = new ApplicationIng
                            {
                                ApplicationIngID = Convert.ToInt32(reader["ApplicationIngID"]),
                                ApplicationID = Convert.ToInt32(reader["ApplicationID"]),
                                IngredientID = Convert.ToInt32(reader["IngredientID"]),
                                Quantity = Convert.ToDouble(reader["Quantity"])
                            };

                            applicationList.Add(application);
                        }
                    }
                }
            }

            return applicationList;
        }

        public void AddApplicationIng(ApplicationIng newApplicationIng)
        {
            string insertQuery = "INSERT INTO ApplicationIngredient (ApplicationID, IngredientID, Quantity) VALUES (@ApplicationID, @IngredientID, @Quantity)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationID", newApplicationIng.ApplicationID);
                        command.Parameters.AddWithValue("@IngredientID", newApplicationIng.IngredientID);
                        command.Parameters.AddWithValue("@Quantity", newApplicationIng.Quantity);


                        command.ExecuteNonQuery();
                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newApplicationIng.ApplicationIngID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                ApplicationIngList.Add(newApplicationIng);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении заявки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateApplicationIng(ApplicationIng updatedApplicationIng)
        {
            string updateQuery = "UPDATE ApplicationIngredient SET Quantity = @Quantity WHERE ApplicationIngID = @ApplicationIngID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Quantity", updatedApplicationIng.Quantity);
                        command.Parameters.AddWithValue("@ApplicationIngID", updatedApplicationIng.ApplicationIngID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Заявка с указанным Id не найдена.");
                    }
                    int index = ApplicationIngList.FindIndex(c => c.ApplicationIngID == updatedApplicationIng.ApplicationIngID);
                    ApplicationIngList[index] = updatedApplicationIng;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении заявки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteApplicationIng(int ApplicationID)
        {
            string deleteQuery = "DELETE FROM ApplicationIngredient WHERE ApplicationID = @ApplicationID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        ApplicationIngList.RemoveAll(applicationIng => applicationIng.ApplicationID == ApplicationID);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении поставки ингредиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}

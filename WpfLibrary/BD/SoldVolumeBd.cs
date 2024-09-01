using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CAT_COMPANY
{
    public class SoldVolumeBd
    {
        public class SoldVolume
        {
            public int SoldID { get; set; }
            public string FoodName { get; set; }
            public int Quantity { get; set; }
        }

        private readonly string connectionString;
        public List<SoldVolume> SoldVolumeList { get; private set; }

        public SoldVolumeBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            SoldVolumeList = GetSoldVolumeData();
        }

        private List<SoldVolume> GetSoldVolumeData()
        {
            List<SoldVolume> soldVolumeList = new List<SoldVolume>();
            string query = "SELECT SoldVolume.SoldID, Food.FoodName, SoldVolume.Quantity FROM SoldVolume INNER JOIN Food ON SoldVolume.FoodID = Food.FoodID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SoldVolume soldVolume = new SoldVolume
                            {
                                SoldID = Convert.ToInt32(reader["SoldID"]),
                                FoodName = Convert.ToString(reader["FoodName"]),
                                Quantity = Convert.ToInt32(reader["Quantity"])
                            };

                            soldVolumeList.Add(soldVolume);
                        }
                    }
                }
            }

            return soldVolumeList;
        }

        public void AddSoldVolume(SoldVolume newSoldVolume)
        {
            string insertQuery = "INSERT INTO SoldVolume (FoodID, Quantity) VALUES (@FoodID, @Quantity)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FoodID", GetFoodIDByName(newSoldVolume.FoodName));
                        command.Parameters.AddWithValue("@Quantity", newSoldVolume.Quantity);

                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newSoldVolume.SoldID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                SoldVolumeList.Add(newSoldVolume);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении продажи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateSoldVolume(SoldVolume updatedSoldVolume)
        {
            string updateQuery = "UPDATE SoldVolume SET FoodID = @FoodID, Quantity = @Quantity WHERE SoldID = @Id";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FoodID", GetFoodIDByName(updatedSoldVolume.FoodName));
                        command.Parameters.AddWithValue("@Quantity", updatedSoldVolume.Quantity);
                        command.Parameters.AddWithValue("@Id", updatedSoldVolume.SoldID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Продажа с указанным Id не найден.");
                    }
                    int index = SoldVolumeList.FindIndex(c => c.SoldID == updatedSoldVolume.SoldID);
                    SoldVolumeList[index] = updatedSoldVolume;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении продажи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteSoldVolume(int SoldID)
        {
            string deleteQuery = "DELETE FROM SoldVolume WHERE SoldID = @Id";
            int index = SoldVolumeList.FindIndex(soldVolume => soldVolume.SoldID == SoldID);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", SoldID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        SoldVolumeList.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении продажи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public int GetFoodIDByName(string FoodName)
        {
            string query = "SELECT FoodID FROM Food WHERE FoodName = @FoodName";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FoodName", FoodName);

                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        return Convert.ToInt32(result);
                    else
                        return -1;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CAT_COMPANY
{
    public class FoodGroupBd
    {
        public class FoodGroup
        {
            public int FoodGroupID { get; set; }
            public string FoodGroupName { get; set; }
        }

        private readonly string connectionString;
        public List<FoodGroup> FoodGroups { get; private set; }

        public FoodGroupBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            FoodGroups = GetFoodGroupData();
        }

        private List<FoodGroup> GetFoodGroupData()
        {
            List<FoodGroup> foodGroups = new List<FoodGroup>();
            string query = "SELECT * FROM FoodGroup";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FoodGroup foodGroup = new FoodGroup
                            {
                                FoodGroupID = Convert.ToInt32(reader["FoodGroupID"]),
                                FoodGroupName = Convert.ToString(reader["FoodGroupName"])
                            };

                            foodGroups.Add(foodGroup);
                        }
                    }
                }
            }

            return foodGroups;
        }

        public void AddFoodGroup(FoodGroup newFoodGroup)
        {
            string insertQuery = "INSERT INTO FoodGroup (FoodGroupName) VALUES (@Name)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", newFoodGroup.FoodGroupName);

                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newFoodGroup.FoodGroupID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                FoodGroups.Add(newFoodGroup);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении группы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateFoodGroup(FoodGroup updatedFoodGroup)
        {
            string updateQuery = "UPDATE FoodGroup SET FoodGroupName = @Name WHERE FoodGroupID = @Id";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", updatedFoodGroup.FoodGroupName);
                        command.Parameters.AddWithValue("@Id", updatedFoodGroup.FoodGroupID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Группа с указанным Id не найден.");
                    }
                    int index = FoodGroups.FindIndex(c => c.FoodGroupID == updatedFoodGroup.FoodGroupID);
                    FoodGroups[index] = updatedFoodGroup;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении группы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteFoodGroup(int FoodGroupID)
        {
            string deleteQuery = "DELETE FROM FoodGroup WHERE FoodGroupID = @Id";
            int index = FoodGroups.FindIndex(foodGroup => foodGroup.FoodGroupID == FoodGroupID);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", FoodGroupID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        FoodGroups.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении группы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}

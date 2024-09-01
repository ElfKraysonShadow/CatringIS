using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CAT_COMPANY
{
    public class FoodBd
    {
        public class Food
        {
            public int FoodID { get; set; }
            public string FoodName { get; set; }
            public string FoodGroupName { get; set; }
            public string Description { get; set; }
            public double Cost { get; set; }
            public int Yield { get; set; }
            public byte[] Photo { get; set; }
        }

        private readonly string connectionString;
        public List<Food> FoodList { get; private set; }

        public FoodBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            FoodList = GetFoodData();
        }

        private List<Food> GetFoodData()
        {
            List<Food> foodList = new List<Food>();
            string query = "SELECT Food.FoodID, Food.FoodName, Food.Description, Food.Cost, Food.Yield, FoodGroup.FoodGroupName, Food.Photo " +
                "FROM Food " +
                "INNER JOIN FoodGroup ON Food.FoodGroupID = FoodGroup.FoodGroupID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Food food = new Food
                            {
                                FoodID = Convert.ToInt32(reader["FoodID"]),
                                FoodName = Convert.ToString(reader["FoodName"]),
                                Description = Convert.ToString(reader["Description"]),
                                Cost = Convert.ToDouble(reader["Cost"]),
                                Yield = Convert.ToInt32(reader["Yield"]),
                                FoodGroupName = Convert.ToString(reader["FoodGroupName"]),
                                Photo = reader["Photo"] as byte[]
                            };

                            foodList.Add(food);
                        }
                    }
                }
            }

            return foodList;
        }

        public void AddFood(Food newFood)
        {
            string insertQuery = "INSERT INTO Food (FoodName, Description, Cost, Yield, FoodGroupID, Photo) VALUES " +
                "(@FoodName, @Description, @Cost, @Yield, @FoodGroupID, @Photo)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FoodName", newFood.FoodName);
                        command.Parameters.AddWithValue("@Description", newFood.Description);
                        command.Parameters.AddWithValue("@Cost", newFood.Cost);
                        command.Parameters.AddWithValue("@Yield", newFood.Yield);
                        command.Parameters.AddWithValue("@FoodGroupID", GetFoodGroupIDByName(newFood.FoodGroupName));
                        command.Parameters.AddWithValue("@Photo", newFood.Photo);

                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newFood.FoodID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                FoodList.Add(newFood);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении ингредиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateFood(Food updatedFood)
        {
            string updateQuery = "UPDATE Food " +
                     "SET FoodName = @FoodName, " +
                     "Description = @Description, " +
                     "Cost = @Cost, " +
                     "Yield = @Yield, " +
                     "FoodGroupID = @FoodGroupID, " +
                     "Photo = @Photo " +
                     "WHERE FoodID = @FoodID;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FoodName", updatedFood.FoodName);
                        command.Parameters.AddWithValue("@Description", updatedFood.Description);
                        command.Parameters.AddWithValue("@Cost", updatedFood.Cost);
                        command.Parameters.AddWithValue("@Yield", updatedFood.Yield);
                        command.Parameters.AddWithValue("@FoodGroupID", GetFoodGroupIDByName(updatedFood.FoodGroupName));
                        command.Parameters.AddWithValue("@Photo", updatedFood.Photo);
                        command.Parameters.AddWithValue("@FoodID", updatedFood.FoodID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Ингредиент с указанным Id не найден.");
                    }
                    int index = FoodList.FindIndex(c => c.FoodID == updatedFood.FoodID);
                    FoodList[index] = updatedFood;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении ингредиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteFood(int FoodID)
        {
            string deleteQuery = "DELETE FROM Food WHERE FoodID = @Id";
            int index = FoodList.FindIndex(food => food.FoodID == FoodID);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", FoodID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        FoodList.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении ингредиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private int GetFoodGroupIDByName(string FoodGroupName)
        {
            string query = "SELECT FoodGroupID FROM FoodGroup WHERE FoodGroupName = @FoodGroupName";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FoodGroupName", FoodGroupName);

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

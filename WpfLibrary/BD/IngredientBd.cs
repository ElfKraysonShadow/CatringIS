using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CAT_COMPANY
{
    public class IngredientBd
    {
        public class Ingredient
        {
            public int IngredientID { get; set; }
            public string IngredientName { get; set; }
            public string UnitOfMeasure { get; set; }
            public double Markup { get; set; }
            public double StockRemain { get; set; }
            public string ProviderName { get; set; }
        }

        private readonly string connectionString;
        public List<Ingredient> IngredientList { get; private set; }

        public IngredientBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            IngredientList = GetIngredientData();
        }

        private List<Ingredient> GetIngredientData()
        {
            List<Ingredient> ingredientList = new List<Ingredient>();
            string query = "SELECT Ingredient.IngredientID, Ingredient.IngredientName, Ingredient.UnitOfMeasure, Ingredient.Markup, Ingredient.StockRemain, Provider.ProviderName " +
                "FROM Ingredient " +
                "INNER JOIN Provider ON Ingredient.ProviderID = Provider.ProviderID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Ingredient ingredient = new Ingredient
                            {
                                IngredientID = Convert.ToInt32(reader["IngredientID"]),
                                IngredientName = Convert.ToString(reader["IngredientName"]),
                                UnitOfMeasure = Convert.ToString(reader["UnitOfMeasure"]),
                                Markup = Convert.ToDouble(reader["Markup"]),
                                StockRemain = Convert.ToDouble(reader["StockRemain"]),
                                ProviderName = Convert.ToString(reader["ProviderName"])
                            };

                            ingredientList.Add(ingredient);
                        }
                    }
                }
            }

            return ingredientList;
        }

        public void AddIngredient(Ingredient newIngredient)
        {
            string insertQuery = "INSERT INTO Ingredient (IngredientName, UnitOfMeasure, Markup, StockRemain, ProviderID) VALUES " +
                "(@IngredientName, @UnitOfMeasure, @Markup, @StockRemain, @ProviderID)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@IngredientName", newIngredient.IngredientName);
                        command.Parameters.AddWithValue("@UnitOfMeasure", newIngredient.UnitOfMeasure);
                        command.Parameters.AddWithValue("@Markup", newIngredient.Markup);
                        command.Parameters.AddWithValue("@StockRemain", newIngredient.StockRemain);
                        command.Parameters.AddWithValue("@ProviderID", GetProviderIDByName(newIngredient.ProviderName));

                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newIngredient.IngredientID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                IngredientList.Add(newIngredient);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении ингредиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateIngredient(Ingredient updatedIngredient)
        {
            string updateQuery = "UPDATE Ingredient " +
                     "SET IngredientName = @IngredientName, " +
                     "UnitOfMeasure = @UnitOfMeasure, " +
                     "Markup = @Markup, " +
                     "StockRemain = @StockRemain, " +
                     "ProviderID = @ProviderID " +
                     "WHERE IngredientID = @IngredientID;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@IngredientName", updatedIngredient.IngredientName);
                        command.Parameters.AddWithValue("@UnitOfMeasure", updatedIngredient.UnitOfMeasure);
                        command.Parameters.AddWithValue("@Markup", updatedIngredient.Markup);
                        command.Parameters.AddWithValue("@StockRemain", updatedIngredient.StockRemain);
                        command.Parameters.AddWithValue("@ProviderID", GetProviderIDByName(updatedIngredient.ProviderName));
                        command.Parameters.AddWithValue("@IngredientID", updatedIngredient.IngredientID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Ингредиент с указанным Id не найден.");
                    }
                    int index = IngredientList.FindIndex(c => c.IngredientID == updatedIngredient.IngredientID);
                    IngredientList[index] = updatedIngredient;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении ингредиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteIngredient(int IngredientID)
        {
            string deleteQuery = "DELETE FROM Ingredient WHERE IngredientID = @Id";
            int index = IngredientList.FindIndex(ingredient => ingredient.IngredientID == IngredientID);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", IngredientID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        IngredientList.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении ингредиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private int GetProviderIDByName(string ProviderName)
        {
            string query = "SELECT ProviderID FROM Provider WHERE ProviderName = @ProviderName";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProviderName", ProviderName);

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

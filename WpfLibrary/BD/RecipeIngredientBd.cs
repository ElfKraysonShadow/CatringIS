using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CAT_COMPANY
{
    public class RecipeIngredientBd
    {
        public class RecipeIngredient
        {
            public int RecipeIngID { get; set; }
            public int RecipeID { get; set; }
            public int IngredientID { get; set; }
            public double NetWeight { get; set; }
            public double GrossWeight { get; set; }
        }

        private readonly string connectionString;
        public List<RecipeIngredient> RecipeIngredientList { get; private set; }

        public RecipeIngredientBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            RecipeIngredientList = GetRecipeIngredientData();
        }

        private List<RecipeIngredient> GetRecipeIngredientData()
        {
            List<RecipeIngredient> recipeIngredientList = new List<RecipeIngredient>();
            string query = "SELECT * FROM RecipeIngredient";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RecipeIngredient recipeIngredient = new RecipeIngredient
                            {
                                RecipeIngID = Convert.ToInt32(reader["RecipeIngID"]),
                                RecipeID = Convert.ToInt32(reader["RecipeID"]),
                                IngredientID = Convert.ToInt32(reader["IngredientID"]),
                                NetWeight = Convert.ToDouble(reader["NetWeight"]),
                                GrossWeight = Convert.ToDouble(reader["GrossWeight"])
                            };

                            recipeIngredientList.Add(recipeIngredient);
                        }
                    }
                }
            }

            return recipeIngredientList;
        }

        public void AddRecipeIngredient(RecipeIngredient newRecipeIngredient)
        {
            string insertQuery = "INSERT INTO RecipeIngredient (RecipeID, IngredientID, NetWeight, GrossWeight) VALUES (@RecipeID, @IngredientID, @NetWeight, @GrossWeight)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@RecipeID", newRecipeIngredient.RecipeID);
                        command.Parameters.AddWithValue("@IngredientID", newRecipeIngredient.IngredientID);
                        command.Parameters.AddWithValue("@NetWeight", newRecipeIngredient.NetWeight);
                        command.Parameters.AddWithValue("@GrossWeight", newRecipeIngredient.GrossWeight);

                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newRecipeIngredient.RecipeIngID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                RecipeIngredientList.Add(newRecipeIngredient);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении поставки ингредиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateRecipeIngredient(RecipeIngredient updatedRecipeIngredient)
        {
            string updateQuery = "UPDATE RecipeIngredient SET NetWeight = @NetWeight, GrossWeight = @GrossWeight WHERE RecipeIngID = @RecipeIngID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@NetWeight", updatedRecipeIngredient.NetWeight);
                        command.Parameters.AddWithValue("@GrossWeight", updatedRecipeIngredient.GrossWeight);
                        command.Parameters.AddWithValue("@RecipeIngID", updatedRecipeIngredient.RecipeIngID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Поставка ингредиента с указанным Id не найдена.");
                    }
                    int index = RecipeIngredientList.FindIndex(c => c.RecipeIngID == updatedRecipeIngredient.RecipeIngID);
                    RecipeIngredientList[index] = updatedRecipeIngredient;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении поставки ингредиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteRecipeIngredient(int RecipeID)
        {
            string deleteQuery = "DELETE FROM RecipeIngredient WHERE RecipeID = @RecipeID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@RecipeID", RecipeID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        RecipeIngredientList.RemoveAll(recipeIngredient => recipeIngredient.RecipeID == RecipeID);
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

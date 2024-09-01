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
    public class RecipeBd
    {
        public class Recipe
        {
            public int RecipeID { get; set; }
            public string FoodName { get; set; }
            public string Description { get; set; }
        }

        private readonly string connectionString;
        public List<Recipe> RecipeList { get; private set; }

        public RecipeBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            RecipeList = GetRecipeData();
        }

        private List<Recipe> GetRecipeData()
        {
            List<Recipe> recipeList = new List<Recipe>();
            string query = "SELECT Recipe.RecipeID, Food.FoodName, Recipe.Description FROM Recipe INNER JOIN Food ON Recipe.FoodID = Food.FoodID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Recipe recipe = new Recipe
                            {
                                RecipeID = Convert.ToInt32(reader["RecipeID"]),
                                FoodName = Convert.ToString(reader["FoodName"]),
                                Description = Convert.ToString(reader["Description"])
                            };

                            recipeList.Add(recipe);
                        }
                    }
                }
            }

            return recipeList;
        }

        public void AddRecipe(Recipe newRecipe)
        {
            string insertQuery = "INSERT INTO Recipe (FoodID, Description) VALUES (@FoodID, @Description)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FoodID", GetFoodIDByName(newRecipe.FoodName));
                        command.Parameters.AddWithValue("@Description", newRecipe.Description);

                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newRecipe.RecipeID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                RecipeList.Add(newRecipe);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении рецепта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateRecipe(Recipe updatedRecipe)
        {
            string updateQuery = "UPDATE Recipe SET FoodID = @FoodID, Description = @Description WHERE RecipeID = @Id";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FoodID", GetFoodIDByName(updatedRecipe.FoodName));
                        command.Parameters.AddWithValue("@Description", updatedRecipe.Description);
                        command.Parameters.AddWithValue("@Id", updatedRecipe.RecipeID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Рецепт с указанным Id не найден.");
                    }
                    int index = RecipeList.FindIndex(c => c.RecipeID == updatedRecipe.RecipeID);
                    RecipeList[index] = updatedRecipe;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении рецепта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteRecipe(int RecipeID)
        {
            string deleteQuery = "DELETE FROM Recipe WHERE RecipeID = @Id";
            int index = RecipeList.FindIndex(recipe => recipe.RecipeID == RecipeID);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", RecipeID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        RecipeList.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении рецепта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CAT_COMPANY
{
    public class SupplyIngredientBd
    {
        public class SupplyIngredient
        {
            public int SupplyIngID { get; set; }
            public int SupplyID { get; set; }
            public int IngredientID { get; set; }
            public double PurnashePrice { get; set; }
            public double Quantity { get; set; }
        }

        private readonly string connectionString;
        public List<SupplyIngredient> SupplyIngredientList { get; private set; }

        public SupplyIngredientBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            SupplyIngredientList = GetSupplyIngredientData();
        }

        private List<SupplyIngredient> GetSupplyIngredientData()
        {
            List<SupplyIngredient> supplyIngredientList = new List<SupplyIngredient>();
            string query = "SELECT * FROM SupplyIngredient";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SupplyIngredient supplyIngredient = new SupplyIngredient
                            {
                                SupplyIngID = Convert.ToInt32(reader["SupplyIngID"]),
                                SupplyID = Convert.ToInt32(reader["SupplyID"]),
                                IngredientID = Convert.ToInt32(reader["IngredientID"]),
                                PurnashePrice = Convert.ToDouble(reader["PurnashePrice"]),
                                Quantity = Convert.ToDouble(reader["Quantity"])
                            };

                            supplyIngredientList.Add(supplyIngredient);
                        }
                    }
                }
            }

            return supplyIngredientList;
        }

        public void AddSupplyIngredient(SupplyIngredient newSupplyIngredient)
        {
            string insertQuery = "INSERT INTO SupplyIngredient (SupplyID, IngredientID, PurnashePrice, Quantity) VALUES (@SupplyID, @IngredientID, @PurnashePrice, @Quantity)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@SupplyID", newSupplyIngredient.SupplyID);
                        command.Parameters.AddWithValue("@IngredientID", newSupplyIngredient.IngredientID);
                        command.Parameters.AddWithValue("@PurnashePrice", newSupplyIngredient.PurnashePrice);
                        command.Parameters.AddWithValue("@Quantity", newSupplyIngredient.Quantity);

                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newSupplyIngredient.SupplyIngID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                SupplyIngredientList.Add(newSupplyIngredient);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении поставки ингредиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateSupplyIngredient(SupplyIngredient updatedSupplyIngredient)
        {
            string updateQuery = "UPDATE SupplyIngredient SET PurnashePrice = @PurnashePrice, Quantity = @Quantity WHERE SupplyIngID = @SupplyIngID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@PurnashePrice", updatedSupplyIngredient.PurnashePrice);
                        command.Parameters.AddWithValue("@Quantity", updatedSupplyIngredient.Quantity);
                        command.Parameters.AddWithValue("@SupplyIngID", updatedSupplyIngredient.SupplyIngID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Поставка ингредиента с указанным Id не найдена.");
                    }
                    int index = SupplyIngredientList.FindIndex(c => c.SupplyIngID == updatedSupplyIngredient.SupplyIngID);
                    SupplyIngredientList[index] = updatedSupplyIngredient;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении поставки ингредиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteSupplyIngredient(int SupplyID)
        {
            string deleteQuery = "DELETE FROM SupplyIngredient WHERE SupplyID = @SupplyID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@SupplyID", SupplyID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        SupplyIngredientList.RemoveAll(supplyIngredient => supplyIngredient.SupplyID == SupplyID);
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

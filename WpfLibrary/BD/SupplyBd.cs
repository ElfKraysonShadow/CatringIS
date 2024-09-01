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
    public class SupplyBd
    {
        public class Supply
        {
            public int SupplyID { get; set; }
            public string ProviderName { get; set; }
            public DateTime Date { get; set; }
        }

        private readonly string connectionString;
        public List<Supply> SupplyList { get; private set; }

        public SupplyBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            SupplyList = GetSupplyData();
        }

        private List<Supply> GetSupplyData()
        {
            List<Supply> supplyList = new List<Supply>();
            string query = "SELECT Supply.SupplyID, Provider.ProviderName, Supply.Date FROM Supply INNER JOIN Provider ON Supply.ProviderID = Provider.ProviderID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Supply supply = new Supply
                            {
                                SupplyID = Convert.ToInt32(reader["SupplyID"]),
                                ProviderName = Convert.ToString(reader["ProviderName"]),
                                Date = Convert.ToDateTime(reader["Date"])
                            };

                            supplyList.Add(supply);
                        }
                    }
                }
            }

            return supplyList;
        }

        public void AddSupply(Supply newSupply)
        {
            string insertQuery = "INSERT INTO Supply (ProviderID, Date) VALUES (@ProviderID, @Date)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ProviderID", GetProviderIDByName(newSupply.ProviderName));
                        command.Parameters.AddWithValue("@Date", newSupply.Date);

                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newSupply.SupplyID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                SupplyList.Add(newSupply);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении поставки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateSupply(Supply updatedSupply)
        {
            string updateQuery = "UPDATE Supply SET ProviderID = @ProviderID, Date = @Date WHERE SupplyID = @Id";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ProviderID", GetProviderIDByName(updatedSupply.ProviderName));
                        command.Parameters.AddWithValue("@Date", updatedSupply.Date);
                        command.Parameters.AddWithValue("@Id", updatedSupply.SupplyID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Поставка с указанным Id не найдена.");
                    }
                    int index = SupplyList.FindIndex(c => c.SupplyID == updatedSupply.SupplyID);
                    SupplyList[index] = updatedSupply;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении поставки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteSupply(int SupplyID)
        {
            string deleteQuery = "DELETE FROM Supply WHERE SupplyID = @Id";
            int index = SupplyList.FindIndex(supply => supply.SupplyID == SupplyID);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", SupplyID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        SupplyList.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении поставки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public int GetProviderIDByName(string ProviderName)
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

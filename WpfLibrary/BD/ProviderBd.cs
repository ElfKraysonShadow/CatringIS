using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CAT_COMPANY
{
    public class ProviderBd
    {
        public class Provider
        {
            public int ProviderID { get; set; }
            public string ProviderName { get; set; }
            public string ManagerName { get; set; }
            public string ManagerNumber { get; set; }
            public string ProviderINN { get; set; }
            public string BankName { get; set; }
            public string BankAccount { get; set; }
            public string FullAddress { get; set; }
        }

        private readonly string connectionString;
        public List<Provider> ProviderList { get; private set; }

        public ProviderBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            ProviderList = GetProviderData();
        }

        private List<Provider> GetProviderData()
        {
            List<Provider> providerList = new List<Provider>();
            string query = "SELECT Provider.ProviderID, Provider.ProviderName, Provider.ManagerName, Provider.ManagerNumber, Provider.ProviderINN, " +
                "Bank.BankName, Provider.BankAccount, Address.Street || ', ' || Address.HouseNumber AS FullAddress " +
                "FROM Provider " +
                "INNER JOIN Bank ON Provider.BankID = Bank.BankID " +
                "INNER JOIN Address ON Provider.AddressID = Address.AddressID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Provider provider = new Provider
                            {
                                ProviderID = Convert.ToInt32(reader["ProviderID"]),
                                ProviderName = Convert.ToString(reader["ProviderName"]),
                                ManagerName = Convert.ToString(reader["ManagerName"]),
                                ManagerNumber = Convert.ToString(reader["ManagerNumber"]),
                                ProviderINN = Convert.ToString(reader["ProviderINN"]),
                                BankName = Convert.ToString(reader["BankName"]),
                                BankAccount = Convert.ToString(reader["BankAccount"]),
                                FullAddress = Convert.ToString(reader["FullAddress"]),
                            };

                            providerList.Add(provider);
                        }
                    }
                }
            }

            return providerList;
        }

        public void AddProvider(Provider newProvider)
        {
            string insertQuery = "INSERT INTO Provider (ProviderName, ManagerName, ManagerNumber, ProviderINN, BankID, AddressID, BankAccount) VALUES " +
                "(@ProviderName, @ManagerName, @ManagerNumber, @ProviderINN, @BankID, @AddressID, @BankAccount)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ProviderName", newProvider.ProviderName);
                        command.Parameters.AddWithValue("@ManagerName", newProvider.ManagerName);
                        command.Parameters.AddWithValue("@ManagerNumber", newProvider.ManagerNumber);
                        command.Parameters.AddWithValue("@ProviderINN", newProvider.ProviderINN);
                        command.Parameters.AddWithValue("@BankID", GetBankIDByName(newProvider.BankName));
                        command.Parameters.AddWithValue("@BankAccount", newProvider.BankAccount);
                        command.Parameters.AddWithValue("@AddressID", GetAddressIDByName(newProvider.FullAddress));


                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newProvider.ProviderID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                ProviderList.Add(newProvider);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении поставщика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateProvider(Provider updatedProvider)
        {
            string updateQuery = "UPDATE Provider " +
                     "SET ProviderName = @ProviderName, " +
                     "ManagerName = @ManagerName, " +
                     "ManagerNumber = @ManagerNumber, " +
                     "ProviderINN = @ProviderINN, " +
                     "BankID = @BankID, " +
                     "AddressID = @AddressID, " +
                     "BankAccount = @BankAccount " +
                     "WHERE ProviderID = @ProviderID;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ProviderName", updatedProvider.ProviderName);
                        command.Parameters.AddWithValue("@ManagerName", updatedProvider.ManagerName);
                        command.Parameters.AddWithValue("@ManagerNumber", updatedProvider.ManagerNumber);
                        command.Parameters.AddWithValue("@ProviderINN", updatedProvider.ProviderINN);
                        command.Parameters.AddWithValue("@BankID", GetBankIDByName(updatedProvider.BankName));
                        command.Parameters.AddWithValue("@BankAccount", updatedProvider.BankAccount);
                        command.Parameters.AddWithValue("@AddressID", GetAddressIDByName(updatedProvider.FullAddress));
                        command.Parameters.AddWithValue("@ProviderID", updatedProvider.ProviderID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Поставщик с указанным Id не найден.");
                    }
                    int index = ProviderList.FindIndex(c => c.ProviderID == updatedProvider.ProviderID);
                    ProviderList[index] = updatedProvider;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении поставщика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteProvider(int ProviderID)
        {
            string deleteQuery = "DELETE FROM Provider WHERE ProviderID = @Id";
            int index = ProviderList.FindIndex(provider => provider.ProviderID == ProviderID);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", ProviderID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        ProviderList.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении поставщика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private int GetAddressIDByName(string FullAddress)
        {
            string query = "SELECT AddressID FROM Address WHERE Street = @Street AND HouseNumber = @HouseNumber";
            string[] fullAddressParts = FullAddress.Split(new[] { ", " }, StringSplitOptions.None);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Street", fullAddressParts[0]);
                    command.Parameters.AddWithValue("@HouseNumber", fullAddressParts[1]);

                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        return Convert.ToInt32(result);
                    else
                        return -1;
                }
            }
        }

        private int GetBankIDByName(string BankName)
        {
            string query = "SELECT BankID FROM Bank WHERE BankName = @BankName";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BankName", BankName);

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

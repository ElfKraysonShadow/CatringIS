using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CAT_COMPANY
{
    public class AddressBd
    {
        public class Address
        {
            public int AddressID { get; set; }
            public string HouseNumber { get; set; }
            public string Street {  get; set; }
        }

        private readonly string connectionString;
        public List<Address> AddressList { get; private set; }

        public AddressBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            AddressList = GetAddressData();
        }

        private List<Address> GetAddressData()
        {
            List<Address> adressList = new List<Address>();
            string query = "SELECT * FROM Address";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Address adress = new Address
                            {
                                AddressID = Convert.ToInt32(reader["AddressID"]),
                                Street = Convert.ToString(reader["Street"]),
                                HouseNumber = Convert.ToString(reader["HouseNumber"])
                            };

                            adressList.Add(adress);
                        }
                    }
                }
            }

            return adressList;
        }

        public void AddAddress(Address newAddress)
        {
            string insertQuery = "INSERT INTO Address (Street, HouseNumber) VALUES (@Street, @HouseNumber)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Street", newAddress.Street);
                        command.Parameters.AddWithValue("@HouseNumber", newAddress.HouseNumber);


                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newAddress.AddressID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                AddressList.Add(newAddress);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении адреса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateAddress(Address updatedAddress)
        {
            string updateQuery = "UPDATE Address SET Street = @Street, HouseNumber = @HouseNumber WHERE AddressID = @Id";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Street", updatedAddress.Street);
                        command.Parameters.AddWithValue("@HouseNumber", updatedAddress.HouseNumber);
                        command.Parameters.AddWithValue("@Id", updatedAddress.AddressID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Адрес с указанным Id не найден.");
                    }
                    int index = AddressList.FindIndex(c => c.AddressID == updatedAddress.AddressID);
                    AddressList[index] = updatedAddress;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении адреса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteAddress(int AddressID)
        {
            string deleteQuery = "DELETE FROM Address WHERE AddressID = @Id";
            int index = AddressList.FindIndex(adress => adress.AddressID == AddressID);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", AddressID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        AddressList.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении адреса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}

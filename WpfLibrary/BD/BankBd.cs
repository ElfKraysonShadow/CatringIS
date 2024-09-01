using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CAT_COMPANY
{
    public class BankBd
    {
        public class Bank
        {
            public int BankID { get; set; }
            public string BankName { get; set; }
        }

        private readonly string connectionString;
        public List<Bank> Banks { get; private set; }

        public BankBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            Banks = GetBankData();
        }

        private List<Bank> GetBankData()
        {
            List<Bank> banks = new List<Bank>();
            string query = "SELECT * FROM Bank";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Bank bank = new Bank
                            {
                                BankID = Convert.ToInt32(reader["BankID"]),
                                BankName = Convert.ToString(reader["BankName"])
                            };

                            banks.Add(bank);
                        }
                    }
                }
            }

            return banks;
        }

        public void AddBank(Bank newBank)
        {
            string insertQuery = "INSERT INTO Bank (BankName) VALUES (@Name)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", newBank.BankName);

                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newBank.BankID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                Banks.Add(newBank);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении банка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateBank(Bank updatedBank)
        {
            string updateQuery = "UPDATE Bank SET BankName = @Name WHERE BankID = @Id";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", updatedBank.BankName);
                        command.Parameters.AddWithValue("@Id", updatedBank.BankID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Банк с указанным Id не найден.");
                    }
                    int index = Banks.FindIndex(c => c.BankID == updatedBank.BankID);
                    Banks[index] = updatedBank;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении банка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteBank(int BankID)
        {
            string deleteQuery = "DELETE FROM Bank WHERE BankID = @Id";
            int index = Banks.FindIndex(bank => bank.BankID == BankID);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", BankID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        Banks.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении банка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}

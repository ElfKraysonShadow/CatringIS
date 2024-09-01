using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using static Tools.WorkWithWord;
using Xceed.Words.NET;
using System.Windows;

namespace Tools
{

    public class WorkToExcel
    {
        static string connectionString = "Data Source=CAT_COMPANY.db;Version=3;";
        string _filePath;
        private List<ClientInfo> GetClientsFromDatabase()
        {

            List<ClientInfo> clients = new List<ClientInfo>();

            string query = @"
            SELECT
                c.FirstName,
                c.LastName,
                c.MiddleName,
                c.BirthDate,
                c.PassportNumber,
                co.Name AS DestinationCountry,
                cr.LastName AS RepresentativeLastName
            FROM
                Clients c
            JOIN
                TouristGroups tg ON c.TouristGroupId = tg.Id
            JOIN
                Countries co ON tg.CountryCode = co.Id
            JOIN
                CompanyRepresentatives cr ON tg.RepresentativeCode = cr.Id;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {

                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClientInfo client = new ClientInfo
                            {
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                MiddleName = reader["MiddleName"].ToString(),
                                BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                                PassportNumber = reader["PassportNumber"].ToString(),
                                DestinationCountry = reader["DestinationCountry"].ToString(),
                                RepresentativeLastName = reader["RepresentativeLastName"].ToString()
                            };

                            clients.Add(client);
                        }
                    }
                }
            }

            return clients;
        }
        public WorkToExcel(string filePath)
        {
            _filePath = filePath;
            ExportClientsToExcel();
        }

        private void ExportClientsToExcel()
        {
            List<ClientInfo> clients = GetClientsFromDatabase();
            FileInfo file = new FileInfo(_filePath);

            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Clients");

                // Заголовки столбцов
                worksheet.Cells[1, 1].Value = "Имя";
                worksheet.Cells[1, 2].Value = "Фамилия";
                worksheet.Cells[1, 3].Value = "Отчество";
                worksheet.Cells[1, 4].Value = "Дата рождения";
                worksheet.Cells[1, 5].Value = "Номер паспорта";
                worksheet.Cells[1, 6].Value = "Страна назначения";
                worksheet.Cells[1, 7].Value = "Фамилия представителя";

                // Данные о клиентах
                int row = 2;
                foreach (var client in clients)
                {
                    worksheet.Cells[row, 1].Value = client.FirstName;
                    worksheet.Cells[row, 2].Value = client.LastName;
                    worksheet.Cells[row, 3].Value = client.MiddleName;
                    worksheet.Cells[row, 4].Value = client.BirthDate.ToShortDateString();
                    worksheet.Cells[row, 5].Value = client.PassportNumber;
                    worksheet.Cells[row, 6].Value = client.DestinationCountry;
                    worksheet.Cells[row, 7].Value = client.RepresentativeLastName;
                    row++;
                }

                package.Save();
            }
        }


        public class ClientInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public string PassportNumber { get; set; }
        public string DestinationCountry { get; set; }
        public string RepresentativeLastName { get; set; }
    }
}
}

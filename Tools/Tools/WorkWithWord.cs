using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Xceed.Words.NET;
using System.Windows;
namespace Tools
{
    public class WorkWithWord
    {
        static string connectionString = "Data Source=CAT_COMPANY.db;Version=3;";
       
        public void GenerateClientDocument(string filePath)
        {
            List<ClientInfo> clients = GetClientsFromDatabase();

            using (DocX document = DocX.Create(filePath))
            {
                document.InsertParagraph("Информация о клиентах")
                        .FontSize(20d)
                        .Bold();
                       

                foreach (var client in clients)
                {
                    document.InsertParagraph($"Имя: {client.FirstName}");
                    document.InsertParagraph($"Фамилия: {client.LastName}");
                    document.InsertParagraph($"Отчество: {client.MiddleName}");
                    document.InsertParagraph($"Дата рождения: {client.BirthDate.ToShortDateString()}");
                    document.InsertParagraph($"Номер паспорта: {client.PassportNumber}");
                    document.InsertParagraph($"Страна назначения: {client.DestinationCountry}");
                    document.InsertParagraph($"Фамилия представителя: {client.RepresentativeLastName}");

                    document.InsertParagraph("--------------------------");
                }
               
                document.Save();
              
            }
            
        }

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

            using(SQLiteConnection connection = new SQLiteConnection(connectionString))
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


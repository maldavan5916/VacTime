using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace VacuumCraft
{
    internal class Client
    {
        public int Id { get;  set; }
        public string Name { get;  set; }
        public string PhoneNomber { get;  set; }
        public string Email { get; set; }
        public string Unp { get; set; }
        public string UrAdress { get; set; }
        public string BankAccount { get; set; }

        public Client(int id)
        {
            string sqlQuery = $"SELECT id, name, phoneNomber, email, unp, urAdress, bankAccount FROM Clients WHERE id = {id}";

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    this.Id = Convert.ToInt32(reader["id"]);
                    Name = reader["name"].ToString();
                    PhoneNomber = reader["phoneNomber"].ToString();
                    Email = reader["email"].ToString();
                    Unp = reader["unp"].ToString();
                    UrAdress = reader["urAdress"].ToString();
                    BankAccount = reader["bankAccount"].ToString();
                }
                reader.Close();
            }
        }

        public Client() { }

        public static List<Client> GetAllClients()
        {
            List<Client> clients = new List<Client>();

            string sqlQuery = "SELECT id, name FROM Clients";

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    clients.Add(new Client
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString()
                    });
                }
                reader.Close();
            }

            return clients;
        }
    }
}

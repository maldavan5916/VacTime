using System;
using System.Data.SqlClient;

namespace VacuumCraft
{
    internal class Client
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string PhoneNomber { get; private set; }
        public string Email { get; private set; }
        public string Unp { get; private set; }
        public string UrAdress { get; private set; }
        public string BankAccount { get; private set; }

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
    }
}

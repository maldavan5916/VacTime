using System;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace VacuumCraft
{
    internal class VacuumInstallations
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string PhotoPath { get; private set; }
        public double Price { get; private set; }

        public VacuumInstallations(int id)
        {
            string sqlQuery = $"SELECT id, name, description, photoPath, price FROM VacuumInstallations WHERE id = {id}";

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    this.Id = Convert.ToInt32(reader["id"]);
                    Name = reader["name"].ToString();
                    Description = reader["description"].ToString();

                    if (Regex.IsMatch(reader["photoPath"].ToString(), @"^[a-zA-Z]:[\\/].*$"))
                        PhotoPath = reader["photoPath"].ToString();
                    else
                        PhotoPath = Directory.GetCurrentDirectory() + reader["photoPath"].ToString();

                    Price = Convert.ToDouble(reader["price"]);
                }
                reader.Close();
            }
        }
    }
}

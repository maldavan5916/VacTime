using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace VacuumCraft
{
    internal class VacuumInstallations
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhotoPath { get; set; }
        public double Price { get; set; }

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

        public VacuumInstallations() { }

        public static List<VacuumInstallations> GetAllInstallations()
        {
            List<VacuumInstallations> installations = new List<VacuumInstallations>();

            string sqlQuery = "SELECT id, name FROM VacuumInstallations WHERE visibility = 1";

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    installations.Add(new VacuumInstallations
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString()
                    });
                }
                reader.Close();
            }

            return installations;
        }
    }
}

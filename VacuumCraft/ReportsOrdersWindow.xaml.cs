using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace VacuumCraft
{
    /// <summary>
    /// Логика взаимодействия для ReportsOrdersWindow.xaml
    /// </summary>
    public partial class ReportsOrdersWindow : Window
    {
        public ReportsOrdersWindow()
        {
            InitializeComponent();

            LoadClients();
            LoadVacuumInstallations();

            EndDP.SelectedDate = DateTime.Now;
            StartDP.SelectedDate = DateTime.Now.AddDays(-10);
        }

        private void LoadClients()
        {
            var clients = Client.GetAllClients();

            clients.Add(new Client { Id = 0, Name = "Все" });

            ClientComboBox.ItemsSource = clients;
            ClientComboBox.DisplayMemberPath = "Name";
            ClientComboBox.SelectedValuePath = "Id";
            ClientComboBox.SelectedValue = 0;
        }

        private void LoadVacuumInstallations()
        {
            var installations = VacuumInstallations.GetAllInstallations();

            installations.Add(new VacuumInstallations { Id = 0, Name = "Все" });

            VacuumInstallationsComboBox.ItemsSource = installations;
            VacuumInstallationsComboBox.DisplayMemberPath = "Name";
            VacuumInstallationsComboBox.SelectedValuePath = "Id";
            VacuumInstallationsComboBox.SelectedValue = 0;
        }

        private void CreateReportButton_Click(object sender, RoutedEventArgs e)
        {
            CreateWordReport();
        }

        private void CreateWordReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Word Document|*.docx",
                Title = "Save the Word Document",
                FileName = "OrdersReport.docx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string connectionString = Properties.Settings.Default.VacTimeDBConnectionString;
                string query = @"
                    SELECT 
                        Id, Clients_id, VacuumInstallations_id, createDate, pathOrder
                    FROM 
                        Orders
                    WHERE 
                        (@ClientId = 0 OR Clients_id = @ClientId)
                        AND (@InstallationId = 0 OR VacuumInstallations_id = @InstallationId)
                        AND createDate BETWEEN @StartDate AND @EndDate;
                ";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ClientId", ClientComboBox.SelectedValue);
                    command.Parameters.AddWithValue("@InstallationId", VacuumInstallationsComboBox.SelectedValue);
                    command.Parameters.AddWithValue("@StartDate", StartDP.SelectedDate);
                    command.Parameters.AddWithValue("@EndDate", EndDP.SelectedDate);

                    SqlDataReader reader = command.ExecuteReader();

                    var doc = DocX.Create(saveFileDialog.FileName);
                    doc.InsertParagraph("Отчет о заявках").FontSize(20).Bold().Alignment = Alignment.center;
                    while (reader.Read())
                    {
                        Client client = new Client(Convert.ToInt32(reader["Clients_id"]));
                        VacuumInstallations vacuumInstallations = new VacuumInstallations(Convert.ToInt32(reader["VacuumInstallations_id"]));

                        doc.InsertParagraph()
                            .AppendLine($"Номер заказа: {reader["id"]}")
                            .AppendLine($"Клиент: {client.Name}")
                            .AppendLine($"Установка: {vacuumInstallations.Name}")
                            .AppendLine($"Дата заказа: {reader["createDate"]}")
                            .AppendLine($"Детали: {reader["pathOrder"]}")
                            .AppendLine(new string('-', 40));
                    }

                    reader.Close();
                    doc.Save();
                    MessageBox.Show("Отчёт создан успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}

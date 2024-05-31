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
        private string role;
        public ReportsOrdersWindow(int roleID)
        {
            InitializeComponent();
            Console.WriteLine("--------------------- " + roleID);
            
            switch(roleID)
            {
                case 2: role = "Менеджер                 "; break;
                case 1: role = "Администратор            "; break;
                default: role = "Неизвестный              "; break;
            }

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

                    doc.InsertParagraph("Отчёт по заявках")
                        .FontSize(20)
                        .Bold()
                        .Alignment = Alignment.center;
                    doc.InsertParagraph()
                        .AppendLine($"По клиенту: {ClientComboBox.Text}")
                        .AppendLine($"По установке: {VacuumInstallationsComboBox.Text}")
                        .AppendLine($"За период: с {StartDP.SelectedDate:dd.MM.yyyy} по {EndDP.SelectedDate:dd.MM.yyyy}");

                    var table = doc.AddTable(1, 5);

                    table.Rows[0].Cells[0].Paragraphs[0].Append("Номер заказа").Bold();
                    table.Rows[0].Cells[1].Paragraphs[0].Append("Клиент").Bold();
                    table.Rows[0].Cells[2].Paragraphs[0].Append("Установка").Bold();
                    table.Rows[0].Cells[3].Paragraphs[0].Append("Дата заказа").Bold();
                    table.Rows[0].Cells[4].Paragraphs[0].Append("Документ").Bold();

                    while (reader.Read())
                    {
                        var row = table.InsertRow();
                        Client client = new Client(Convert.ToInt32(reader["Clients_id"]));
                        VacuumInstallations vacuumInstallations = new VacuumInstallations(Convert.ToInt32(reader["VacuumInstallations_id"]));

                        row.Cells[0].Paragraphs[0].Append(reader["id"].ToString());
                        row.Cells[1].Paragraphs[0].Append(client.Name);
                        row.Cells[2].Paragraphs[0].Append(vacuumInstallations.Name);
                        row.Cells[3].Paragraphs[0].Append(Convert.ToDateTime(reader["createDate"]).ToString("dd.MM.yyyy"));
                        row.Cells[4].Paragraphs[0].Append(reader["pathOrder"].ToString());
                    }

                    reader.Close();
                    doc.InsertTable(table);

                    doc.InsertParagraph("");
                    //doc.InsertParagraph($"{role}\t\t__________\t_______________");
                    //doc.InsertParagraph("      (Должность)\t\t\t   (Подпись)\t  (Расшифровка)").FontSize(9);
                    doc.InsertParagraph()
                      .Append(role).UnderlineStyle(UnderlineStyle.singleLine).Append("\t\t\t\t\t")
                      .Append("____________\t__________________")
                      .AppendLine("       (Должность)\t\t\t\t\t\t     (Подпись)\t      (Расшифровка)").FontSize(9);
                    Console.WriteLine(role);
                    doc.Save();
                    MessageBox.Show("Отчёт создан успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}

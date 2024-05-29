using Microsoft.Win32;
using System.Data.SqlClient;
using System.Windows;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace VacuumCraft
{
    /// <summary>
    /// Логика взаимодействия для ReportsClientsWindow.xaml
    /// </summary>
    public partial class ReportsClientsWindow : Window
    {
        public ReportsClientsWindow()
        {
            InitializeComponent();
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
                FileName = "ClientsReport.docx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string connectionString = Properties.Settings.Default.VacTimeDBConnectionString;
                string query = "SELECT name, phoneNomber, email, unp, urAdress, bankAccount FROM Clients";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    var doc = DocX.Create(saveFileDialog.FileName);

                    doc.InsertParagraph("Отчёт о клиентах")
                        .FontSize(20)
                        .Bold()
                        .Alignment = Alignment.center;

                    while (reader.Read())
                    {
                        if (NameCB.IsChecked == true)
                        {
                            doc.InsertParagraph($"Имя клиента: {reader["name"]}")
                                .FontSize(12);
                        }
                        if (PhoneCB.IsChecked == true)
                        {
                            doc.InsertParagraph($"Номер телефона: {reader["phoneNomber"]}")
                                .FontSize(12);
                        }
                        if (EmailCB.IsChecked == true)
                        {
                            doc.InsertParagraph($"Email: {reader["email"]}")
                                .FontSize(12);
                        }
                        if (UnpCB.IsChecked == true)
                        {
                            doc.InsertParagraph($"УНП: {reader["unp"]}")
                                .FontSize(12);
                        }
                        if (UrAdressCB.IsChecked == true)
                        {
                            doc.InsertParagraph($"Юр. Адрес: {reader["urAdress"]}")
                                .FontSize(12);
                        }
                        if (BankAccountCB.IsChecked == true)
                        {
                            doc.InsertParagraph($"Банковский счёт: {reader["bankAccount"]}")
                                .FontSize(12);
                        }
                        doc.InsertParagraph();
                    }

                    reader.Close();
                    doc.Save();
                    MessageBox.Show("Отчёт создан успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}

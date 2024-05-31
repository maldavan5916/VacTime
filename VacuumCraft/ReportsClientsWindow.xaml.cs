using Microsoft.Win32;
using System;
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
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
                {
                    var doc = DocX.Create(saveFileDialog.FileName);

                    doc.InsertParagraph("Список клиентов")
                        .FontSize(20)
                        .Bold()
                        .Alignment = Alignment.center;

                    int columnCount = 0;
                    if (NameCB.IsChecked == true) columnCount++;
                    if (PhoneCB.IsChecked == true) columnCount++;
                    if (EmailCB.IsChecked == true) columnCount++;
                    if (UnpCB.IsChecked == true) columnCount++;
                    if (UrAdressCB.IsChecked == true) columnCount++;
                    if (BankAccountCB.IsChecked == true) columnCount++;

                    var table = doc.AddTable(1, columnCount);
                    int columnIndex = 0;

                    if (NameCB.IsChecked == true) table.Rows[0].Cells[columnIndex++].Paragraphs[0].Append("Имя клиента").Bold();
                    if (PhoneCB.IsChecked == true) table.Rows[0].Cells[columnIndex++].Paragraphs[0].Append("Номер телефона").Bold();
                    if (EmailCB.IsChecked == true) table.Rows[0].Cells[columnIndex++].Paragraphs[0].Append("Email").Bold();
                    if (UnpCB.IsChecked == true) table.Rows[0].Cells[columnIndex++].Paragraphs[0].Append("УНП").Bold();
                    if (UrAdressCB.IsChecked == true) table.Rows[0].Cells[columnIndex++].Paragraphs[0].Append("Юр. Адрес").Bold();
                    if (BankAccountCB.IsChecked == true) table.Rows[0].Cells[columnIndex++].Paragraphs[0].Append("Банковский счёт").Bold();

                    SqlCommand command = new SqlCommand("SELECT Clients_id FROM Users WHERE Roles_id = 3", connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var row = table.InsertRow();
                        columnIndex = 0;
                        Client client = new Client(Convert.ToInt32(reader["Clients_id"]));
                        if (NameCB.IsChecked == true) row.Cells[columnIndex++].Paragraphs[0].Append(client.Name);
                        if (PhoneCB.IsChecked == true) row.Cells[columnIndex++].Paragraphs[0].Append(client.PhoneNomber);
                        if (EmailCB.IsChecked == true) row.Cells[columnIndex++].Paragraphs[0].Append(client.Email);
                        if (UnpCB.IsChecked == true) row.Cells[columnIndex++].Paragraphs[0].Append(client.Unp);
                        if (UrAdressCB.IsChecked == true) row.Cells[columnIndex++].Paragraphs[0].Append(client.UrAdress);
                        if (BankAccountCB.IsChecked == true) row.Cells[columnIndex++].Paragraphs[0].Append(client.BankAccount);
                    }

                    reader.Close();
                    doc.InsertTable(table);
                    doc.Save();
                    MessageBox.Show("Отчёт создан успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}

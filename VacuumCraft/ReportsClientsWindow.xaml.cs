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

                    // Create the title
                    doc.InsertParagraph("Список клиентов")
                        .FontSize(20)
                        .Bold()
                        .Alignment = Alignment.center;

                    // Determine the number of columns based on the selected checkboxes
                    int columnCount = 0;
                    if (NameCB.IsChecked == true) columnCount++;
                    if (PhoneCB.IsChecked == true) columnCount++;
                    if (EmailCB.IsChecked == true) columnCount++;
                    if (UnpCB.IsChecked == true) columnCount++;
                    if (UrAdressCB.IsChecked == true) columnCount++;
                    if (BankAccountCB.IsChecked == true) columnCount++;

                    // Create the table with dynamic columns
                    var table = doc.AddTable(1, columnCount);
                    int columnIndex = 0;

                    // Add headers
                    if (NameCB.IsChecked == true) table.Rows[0].Cells[columnIndex++].Paragraphs[0].Append("Имя клиента").Bold();
                    if (PhoneCB.IsChecked == true) table.Rows[0].Cells[columnIndex++].Paragraphs[0].Append("Номер телефона").Bold();
                    if (EmailCB.IsChecked == true) table.Rows[0].Cells[columnIndex++].Paragraphs[0].Append("Email").Bold();
                    if (UnpCB.IsChecked == true) table.Rows[0].Cells[columnIndex++].Paragraphs[0].Append("УНП").Bold();
                    if (UrAdressCB.IsChecked == true) table.Rows[0].Cells[columnIndex++].Paragraphs[0].Append("Юр. Адрес").Bold();
                    if (BankAccountCB.IsChecked == true) table.Rows[0].Cells[columnIndex++].Paragraphs[0].Append("Банковский счёт").Bold();

                    // Add data rows
                    while (reader.Read())
                    {
                        var row = table.InsertRow();
                        columnIndex = 0;
                        if (NameCB.IsChecked == true) row.Cells[columnIndex++].Paragraphs[0].Append(reader["name"].ToString());
                        if (PhoneCB.IsChecked == true) row.Cells[columnIndex++].Paragraphs[0].Append(reader["phoneNomber"].ToString());
                        if (EmailCB.IsChecked == true) row.Cells[columnIndex++].Paragraphs[0].Append(reader["email"].ToString());
                        if (UnpCB.IsChecked == true) row.Cells[columnIndex++].Paragraphs[0].Append(reader["unp"].ToString());
                        if (UrAdressCB.IsChecked == true) row.Cells[columnIndex++].Paragraphs[0].Append(reader["urAdress"].ToString());
                        if (BankAccountCB.IsChecked == true) row.Cells[columnIndex++].Paragraphs[0].Append(reader["bankAccount"].ToString());
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

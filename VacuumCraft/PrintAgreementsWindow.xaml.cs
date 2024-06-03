using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace VacuumCraft
{
    /// <summary>
    /// Логика взаимодействия для PrintAgreementsWindow.xaml
    /// </summary>
    public partial class PrintAgreementsWindow : Window
    {
        private int currentSelectOrderID;

        public PrintAgreementsWindow()
        {
            InitializeComponent();
            dateConclusionDP.SelectedDate = DateTime.Now;
            UpdatelistViewData();
        }

        private void UpdatelistViewData()
        {
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                string sqlQuery = "SELECT id, Clients_id, VacuumInstallations_id, createDate, pathOrder FROM Orders";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, connection);
                DataSet dataSet = new DataSet();

                adapter.Fill(dataSet, "Orders");
                dataSet.Tables["Orders"].Columns.Add("ClientName");
                dataSet.Tables["Orders"].Columns.Add("VacuumInstallationName");
                dataSet.Tables["Orders"].Columns.Add("Date");

                foreach (DataRow row in dataSet.Tables["Orders"].Rows)
                {
                    Client client = new Client(Convert.ToInt32(row["Clients_id"]));
                    VacuumInstallations installation = new VacuumInstallations(Convert.ToInt32(row["VacuumInstallations_id"]));

                    row["ClientName"] = client.Name;
                    row["VacuumInstallationName"] = installation.Name;
                    row["Date"] = Convert.ToDateTime(row["createDate"]).ToString("dd.MM.yyyy");
                }

                listView2.ItemsSource = dataSet.Tables["Orders"].DefaultView;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                if (sender is TextBox textBox)
                {
                    int caretIndex = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Insert(caretIndex, Environment.NewLine);
                    textBox.CaretIndex = caretIndex + Environment.NewLine.Length;
                }
            }
        }

        private void SaveDocx()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Word Document|*.docx",
                Title = "Save the Word Document",
                FileName = "Agreement.docx"
            };

            if (saveFileDialog.ShowDialog() != true) return;

            string templatePath = $"{Directory.GetCurrentDirectory()}\\VacuumCraftData\\Template.docx";
            string newFilePath = saveFileDialog.FileName;

            using (var doc = DocX.Load(templatePath))
            {
                doc.Bookmarks["OrganizationName"].SetText(AssemblyCompany);
                doc.Bookmarks["ClientName"].SetText(clinetNameBox.Text);
                doc.Bookmarks["OrderDate"].SetText(orderCreateDateBox.Text);
                doc.Bookmarks["OrderID"].SetText(currentSelectOrderID.ToString());
                doc.Bookmarks["AgreementDate"].SetText($"{dateConclusionDP.SelectedDate:dd.MM.yyyy}");
                doc.Bookmarks["Сonditions"].SetText(ConditionsBox.Text);

                doc.InsertParagraph().InsertPageBreakAfterSelf();
                // Insert "Приложение 1" paragraph aligned to the right
                var appendixTitle = doc.InsertParagraph("Приложение 1").FontSize(14).Bold();
                appendixTitle.Alignment = Alignment.right;
                InsertDocument(doc, orderPathBox.Text);

                doc.SaveAs(newFilePath);
            }
        }

        private void InsertDocument(DocX mainDocument, string subDocumentPath)
        {
            using (var subDocument = DocX.Load(subDocumentPath))
            {
                // Append the content of the sub-document to the main document
                foreach (var element in subDocument.Paragraphs)
                {
                    mainDocument.InsertParagraph(element);
                }
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            if (listView2.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана заявка", "Ошибка");
                return;
            }
            else
                SaveDocx();

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                connection.Open();
                string insertquery = "INSERT INTO Agreements (Orders_id, conclusionDate, price, conditions) VALUES (@OrdersId, @ConclusionDate, @Price, @Conditions)";

                using (SqlCommand command = new SqlCommand(insertquery, connection))
                {
                    command.Parameters.AddWithValue("@OrdersId", currentSelectOrderID);
                    command.Parameters.AddWithValue("@ConclusionDate", dateConclusionDP.SelectedDate);
                    command.Parameters.AddWithValue("@Price", priceBox.Text);
                    command.Parameters.AddWithValue("@Conditions", ConditionsBox.Text);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Договор сохранён", "Успешно");
                    }
                }
            }
        }

        private void listView2_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DataRowView dataRowView = listView2.SelectedItem as DataRowView;
            currentSelectOrderID = Convert.ToInt32(dataRowView.Row["id"]);

            clinetNameBox.Text = dataRowView.Row["ClientName"].ToString();
            installationsNameBox.Text = dataRowView.Row["VacuumInstallationName"].ToString();
            orderCreateDateBox.Text = dataRowView.Row["Date"].ToString();
            orderPathBox.Text = Directory.GetCurrentDirectory() + dataRowView.Row["pathOrder"].ToString();

            VacuumInstallations vi = new VacuumInstallations(Convert.ToInt32(dataRowView.Row["VacuumInstallations_id"]));
            priceBox.Text = vi.Price.ToString();
        }

        private void priceBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PrintBtn.IsEnabled = Validation.Price(sender);
        }
    }
}

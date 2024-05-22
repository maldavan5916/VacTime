using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace VacuumCraft
{
    /// <summary>
    /// Логика взаимодействия для VacuumOrder.xaml
    /// </summary>
    public partial class VacuumOrder : Window
    {
        readonly VacuumInstallations VacuumInstallations;
        readonly Client Client;
        readonly bool[] ValidTextBoxes = new bool[6];

        public VacuumOrder(int VacuumInstallationID, int ClientId)
        {
            InitializeComponent();

            VacuumInstallations = new VacuumInstallations(VacuumInstallationID);
            Client = new Client(ClientId);

            try { VacuumPhoto.Source = new BitmapImage(new Uri(VacuumInstallations.PhotoPath)); } catch { }
            VacuumNameBox.Text = VacuumInstallations.Name;
            VacuumPriceBox.Text = VacuumInstallations.Price.ToString();
            VacuumDescription.Text = VacuumInstallations.Description;

            DataPicker.SelectedDate = DateTime.Now;

            ClientNameBox.Text = Client.Name;
            СlientPhoneBox.Text = Client.PhoneNomber;
            ClientEmailBox.Text = Client.Email;
            ClientUnpBox.Text = Client.Unp;
            ClientUrAdressBox.Text = Client.UrAdress;
            ClientBankAccountBox.Text = Client.BankAccount;
        }

        private void ToDocx(string path, int id)
        {
            // Создаем новый документ Word
            using (var document = DocX.Create(path))
            {
                // Добавляем заголовок
                document.InsertParagraph($"Заявка №{id}")
                    .FontSize(20)
                    .Bold()
                    .Alignment = Alignment.center;
                document.InsertParagraph($"Дата создания: {DataPicker.SelectedDate.Value:dd.MM.yyyy}")
                    .FontSize(14)
                    .SpacingAfter(20);

                document.InsertParagraph($"Установка: {VacuumNameBox.Text}")
                    .FontSize(12);
                document.InsertParagraph($"Цена: {VacuumPriceBox.Text}")
                    .FontSize(12);
                document.InsertParagraph($"Описание: {VacuumDescription.Text}")
                    .FontSize(12);

                document.InsertParagraph().SpacingAfter(20);

                document.InsertParagraph($"Клиент: {ClientNameBox.Text}")
                    .FontSize(12);
                document.InsertParagraph($"Номер телефона: {СlientPhoneBox.Text}")
                    .FontSize(12);
                document.InsertParagraph($"Email: {ClientEmailBox.Text}")
                    .FontSize(12);
                document.InsertParagraph($"УНП: {ClientUnpBox.Text}")
                    .FontSize(12);
                document.InsertParagraph($"юр. Адрес: {ClientUrAdressBox.Text}")
                    .FontSize(12);
                document.InsertParagraph($"Банковский счёт: {ClientBankAccountBox.Text}")
                    .FontSize(12);

                document.Save();
            }
        }

        private void AcceptClicl(object sender, EventArgs e)
        {
            if (Array.Exists(ValidTextBoxes, element => element == false))
            {
                MessageBox.Show("Не все поля заполнены правильно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string pathToDocx;
            int newOrderId;

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO Orders (Clients_id, VacuumInstallations_id, createDate) VALUES (@ClientsId, @VacuumInstallationsId, @CreateDate); SELECT SCOPE_IDENTITY();";
                string insertPath = "UPDATE Orders SET pathOrder = @PathOrder WHERE id = @OrderId";


                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ClientsId", Client.Id);
                    command.Parameters.AddWithValue("@VacuumInstallationsId", VacuumInstallations.Id);
                    command.Parameters.AddWithValue("@CreateDate", DataPicker.SelectedDate);

                    newOrderId = Convert.ToInt32(command.ExecuteScalar());
                }

                using (SqlCommand command = new SqlCommand(insertPath, connection))
                {
                    pathToDocx = $"\\VacuumCraftData\\Orders\\{newOrderId}_{DataPicker.SelectedDate.Value.Date:yyyy-MM-dd}.docx";

                    command.Parameters.AddWithValue("@PathOrder", pathToDocx);
                    command.Parameters.AddWithValue("@OrderId", newOrderId);

                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
            ToDocx(Directory.GetCurrentDirectory() + pathToDocx, newOrderId);

            MessageBoxResult result = MessageBox.Show($"Скачать заявку", "Успешно", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Microsoft Word (*.docx)|*.docx";
                saveFileDialog.Title = "Save order";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        ToDocx(filePath, newOrderId);
                        MessageBox.Show("Файл успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            Close();
        }

        private void ClientNameBox_TextChanged(object sender, RoutedEventArgs e)
        {
            ValidTextBoxes[0] = Validation.ClientName(sender);
        }

        private void СlientPhoneBox_TextChanged(object sender, RoutedEventArgs e)
        {
            ValidTextBoxes[1] = Validation.PhoneNomber(sender);
        }

        private void ClientEmailBox_TextChanged(object sender, RoutedEventArgs e)
        {
            ValidTextBoxes[2] = Validation.Email(sender);
        }

        private void ClientUnpBox_TextChanged(object sender, RoutedEventArgs e)
        {
            ValidTextBoxes[3] = Validation.Unp(sender);
        }

        private void ClientUrAdressBox_TextChanged(object sender, RoutedEventArgs e)
        {
            ValidTextBoxes[4] = Validation.UrAdress(sender);
        }

        private void ClientBankAccountBox_TextChanged(object sender, RoutedEventArgs e)
        {
            ValidTextBoxes[5] = Validation.BankAccount(sender);
        }

        private void Cancel(object sender, EventArgs e)
        {
            Close();
        }
    }
}

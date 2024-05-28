using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace VacuumCraft
{
    /// <summary>
    /// Логика взаимодействия для VacuumInstallationsWindow.xaml
    /// </summary>
    public partial class VacuumInstallationsWindow : Window
    {
        private bool[] ValidTextBoxes = new bool[4];
        private VacuumInstallations Installations;

        public VacuumInstallationsWindow()
        {
            InitializeComponent();
            ChangeBtn.Visibility = Visibility.Hidden;
        }

        public VacuumInstallationsWindow(int vacuumInstallationsid)
        {
            InitializeComponent();
            Installations = new VacuumInstallations(vacuumInstallationsid);

            NameBox.Text = Installations.Name;
            PhotoBox.Text = Installations.PhotoPath;
            PriceBox.Text = Installations.Price.ToString();
            DescriptionBox.Text = Installations.Description;

            AcceptBtn.Visibility = Visibility.Hidden;
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

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidTextBoxes[0] = Validation.Name(sender);
        }

        private void PriceBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidTextBoxes[1] = Validation.Price(sender);
        }

        private void DescriptionBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidTextBoxes[2] = Validation.Description(sender);
        }

        private void PhotoBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Validation.Path(sender, true))
            {
                try
                {
                    ImageVacuum.Source = LoadImage(PhotoBox.Text);
                    ValidTextBoxes[3] = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    ValidTextBoxes[3] = false;
                }
            }
            else
            {
                new BitmapImage(new Uri("/NoImage.png", UriKind.Relative));
                ValidTextBoxes[3] = false;
            }
        }

        private BitmapImage LoadImage(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage();
            using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }
            return bitmap;
        }


        private void OpenImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp, *.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                PhotoBox.Text = openFileDialog.FileName;
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (Array.Exists(ValidTextBoxes, element => element == false))
            {
                MessageBox.Show("Не все поля заполнены правильно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int newVacuumInstallationsId;
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO VacuumInstallations (name, description, price) VALUES (@Name, @Description, @Price); SELECT SCOPE_IDENTITY();";
                string insertPath = "UPDATE VacuumInstallations SET photoPath = @PhotoPath WHERE id = @VacuumInstallationsId";


                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", NameBox.Text);
                    command.Parameters.AddWithValue("@Description", DescriptionBox.Text);
                    command.Parameters.AddWithValue("@Price", Convert.ToDouble(PriceBox.Text));

                    newVacuumInstallationsId = Convert.ToInt32(command.ExecuteScalar());
                }

                using (SqlCommand command = new SqlCommand(insertPath, connection))
                {
                    command.Parameters.AddWithValue("@PhotoPath", $"\\VacuumCraftData\\VacuumPhoto\\{newVacuumInstallationsId}.jpg");
                    command.Parameters.AddWithValue("@VacuumInstallationsId", newVacuumInstallationsId);

                    int rowsAffected = command.ExecuteNonQuery();
                }
            }

            ConvertAndCopyImage(PhotoBox.Text, $"{Directory.GetCurrentDirectory()}\\VacuumCraftData\\VacuumPhoto\\{newVacuumInstallationsId}.jpg");
            MessageBox.Show("Успешно");
            Close();
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (Array.Exists(ValidTextBoxes, element => element == false))
            {
                MessageBox.Show("Не все поля заполнены правильно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                connection.Open();

                string Query = @"
                        UPDATE VacuumInstallations
                        SET name = @Name,
                            description = @Description,
                            photoPath = @PhotoPath,
                            price = @Price
                        WHERE id = @Id";


                using (SqlCommand command = new SqlCommand(Query, connection))
                {
                    command.Parameters.AddWithValue("@Id", Installations.Id);
                    command.Parameters.AddWithValue("@Name", NameBox.Text);
                    command.Parameters.AddWithValue("@PhotoPath", $"\\VacuumCraftData\\VacuumPhoto\\{Installations.Id}.jpg");
                    command.Parameters.AddWithValue("@Description", DescriptionBox.Text);
                    command.Parameters.AddWithValue("@Price", Convert.ToDouble(PriceBox.Text));

                    
                    ConvertAndCopyImage(PhotoBox.Text, $"{Directory.GetCurrentDirectory()}\\VacuumCraftData\\VacuumPhoto\\{Installations.Id}.jpg");
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Успешно");
            Close();
        }

        static void ConvertAndCopyImage(string sourceFilePath, string destinationFilePath)
        {
            try
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(sourceFilePath))
                {
                    image.Save(destinationFilePath, ImageFormat.Jpeg);
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Исходный файл не найден.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}

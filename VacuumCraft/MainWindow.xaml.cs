using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace VacuumCraft
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int UserID;
        private readonly int ClientId;
        private readonly int RoleID;

        public MainWindow(int UserID)
        {
            InitializeComponent();

            string sqlQuery = "SELECT Clients_id, Roles_id FROM Users WHERE id = @UserID";
            this.UserID = UserID;

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                UpdatelistViewData();
                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ClientId = reader.GetInt32(0);
                            RoleID = reader.GetInt32(1);
                        }
                    }
                }
            }

            switch (RoleID)
            {
                case 2:
                    {
                        AddUser.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 3:
                    {
                        ReportsMenu.Visibility = Visibility.Collapsed;
                        AddMenu.Visibility = Visibility.Collapsed;
                    }
                    break;
            }
        }

        private void UpdatelistViewData()
        {
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                string sqlQuery = "SELECT id, name, description, photoPath, price FROM VacuumInstallations WHERE Visibility = 1";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, connection);
                DataSet dataSet = new DataSet();

                adapter.Fill(dataSet, "VacuumInstallations");
                dataSet.Tables["VacuumInstallations"].Columns.Add("image");

                foreach (DataRow row in dataSet.Tables["VacuumInstallations"].Rows)
                {
                    string photoPath = row["photoPath"].ToString();
                    row["image"] = Regex.IsMatch(photoPath, @"^[a-zA-Z]:[\\/].*$")
                        ? photoPath
                        : Directory.GetCurrentDirectory() + photoPath;
                }

                listView1.ItemsSource = dataSet.Tables["VacuumInstallations"].DefaultView;
            }
        }

        private void AbotProgram(object sender, RoutedEventArgs e)
        {
            new AboutBox1().ShowDialog();
        }

        private void VacuumInstallationsClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                new VacuumOrder(Convert.ToInt32(button.Uid), ClientId).ShowDialog();
            }
        }

        private void OpenProfile(object sender, RoutedEventArgs e)
        {
            new Profile(UserID, ClientId).ShowDialog();
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }

        private void HideButton_loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && RoleID == 3)
            {
                btn.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateCatalog_Click(object sender, RoutedEventArgs e)
        {
            UpdatelistViewData();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            new Profile("Admin").ShowDialog();
        }

        private void InstallationsWindow_Closed(object sender, EventArgs e)
        {
            UpdatelistViewData();
        }

        private void VacuumInstallations_Click(object sender, RoutedEventArgs e)
        {
            VacuumInstallationsWindow installationsWindow = new VacuumInstallationsWindow();
            installationsWindow.Closed += InstallationsWindow_Closed;
            installationsWindow.ShowDialog();
        }

        private void VacuumInstallations_Change(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                VacuumInstallationsWindow installationsWindow = new VacuumInstallationsWindow(Convert.ToInt32(button.Uid));
                installationsWindow.Closed += InstallationsWindow_Closed;
                installationsWindow.ShowDialog();
            }
        }

        private void VacuumInstallations_Delete(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                int vacuumInstallationsid = Convert.ToInt32(button.Uid);

                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
                {
                    string deleteQuery = "UPDATE VacuumInstallations SET visibility = 0 WHERE id = @VacuumInstallationsId";
                    VacuumInstallations vacuumInstallations = new VacuumInstallations(vacuumInstallationsid);

                    MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить установку\n{vacuumInstallations.Name}", "Удаление", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }

                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@VacuumInstallationsId", vacuumInstallationsid);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                    }
                }
                UpdatelistViewData();
            }
        }

        private void ReportsOrders(object sender, RoutedEventArgs e)
        {
            new ReportsOrdersWindow().ShowDialog();
        }

        private void ReportsClients(object sender, RoutedEventArgs e)
        {
            new ReportsClientsWindow().ShowDialog();
        }

        private void PrintAgreements(object sender, RoutedEventArgs e)
        {
            new PrintAgreementsWindow().ShowDialog();
        }
    }

    public class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double width = (double)value;
            double adjustment = System.Convert.ToDouble(parameter);
            return width + adjustment;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imagePath = value as string;
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                return LoadImage(imagePath);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
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
    }

}

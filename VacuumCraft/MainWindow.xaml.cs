using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace VacuumCraft
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int UserID;
        private readonly int ClientId;

        public MainWindow(int UserID)
        {
            InitializeComponent();

            string sqlQuery = "SELECT id, name, description, photoPath, price FROM VacuumInstallations";
            string sqlQueryClient = $"SELECT Clients_id FROM Users WHERE id = {UserID}";
            this.UserID = UserID;

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, connection);
                DataSet dataSet = new DataSet();

                adapter.Fill(dataSet, "VacuumInstallations");
                dataSet.Tables["VacuumInstallations"].Columns.Add("image");

                foreach (DataRow row in dataSet.Tables["VacuumInstallations"].Rows)
                {
                    string photoPath = row["photoPath"].ToString();

                    if (Regex.IsMatch(photoPath, @"^[a-zA-Z]:[\\/].*$"))
                        row["image"] = photoPath;
                    else
                        row["image"] = Directory.GetCurrentDirectory() + photoPath;
                }

                listView1.ItemsSource = dataSet.Tables["VacuumInstallations"].DefaultView;

                connection.Open();
                SqlCommand command = new SqlCommand(sqlQueryClient, connection);
                object result = command.ExecuteScalar();

                if (result != null)
                    ClientId = Convert.ToInt32(result);
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

}

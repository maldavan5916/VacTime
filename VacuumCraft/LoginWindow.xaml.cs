using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace VacuumCraft
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            LoginBox.Focus();
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        }

        private void Auth(object sender, RoutedEventArgs e)
        {
            try
            {
                string sqlQuery = $"SELECT id, pass FROM Users WHERE name = '{LoginBox.Text}'";

                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, connection);
                    DataSet dataSet = new DataSet();

                    adapter.Fill(dataSet);

                    if (dataSet.Tables[0].Rows.Count == 1)
                    {
                        DataRow row = dataSet.Tables[0].Rows[0];
                        if (row["pass"].ToString() == PasswordBox.Password)
                        {
                            MainWindow mainWindow = new MainWindow(Convert.ToInt32(row["id"]));
                            mainWindow.Show();
                            Close();
                        }
                        else
                            throw new Exception();
                    }
                    else
                        throw new Exception();
                }
            }
            catch
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateAkk(object sender, RoutedEventArgs e)
        {
            new Profile().ShowDialog();
        }

        private void OpenReadme(object sender, RoutedEventArgs e)
        {
            Process.Start("readme.chm");
        }
    }
}

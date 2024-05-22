using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VacuumCraft
{
    /// <summary>
    /// Логика взаимодействия для Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        readonly bool[] ValidTextBoxes = new bool[7];

        private readonly int idUser;
        private readonly int idClient;

        public Profile(int idUser, int idClient)
        {
            InitializeComponent();
            this.idUser = idUser;
            this.idClient = idClient;

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT name FROM Users WHERE id = {idUser}", connection);
                object result = command.ExecuteScalar();
                if (result != null) { ProfileNameBox.Text = result.ToString(); }
            }

            Client client = new Client(idClient);

            ClientNameBox.Text = client.Name;
            PhoneNomberBox.Text = client.PhoneNomber;
            EmailBox.Text = client.Email;
            UnpBox.Text = client.Unp;
            UrAdressBox.Text = client.UrAdress;
            BankAccountBox.Text = client.BankAccount;
        }

        public Profile()
        {
            InitializeComponent();
            ValidTextBoxes[3] = true;
            ValidTextBoxes[4] = true;
            ValidTextBoxes[5] = true;

            btn1.Visibility = Visibility.Hidden;
            btn2.Visibility = Visibility.Hidden;
            lbl1.Visibility = Visibility.Hidden;
            lbl2.Content = "Пароль:";
            OldPass.Visibility = Visibility.Hidden;

            btn3.Visibility = Visibility.Visible;
            Title = "Создание аккаунта";
        }

        public Profile(string str)
        {
            InitializeComponent();
            ValidTextBoxes[3] = true;
            ValidTextBoxes[4] = true;
            ValidTextBoxes[5] = true;

            btn1.Visibility = Visibility.Hidden;
            btn2.Visibility = Visibility.Hidden;
            lbl1.Visibility = Visibility.Hidden;
            lbl2.Content = "Пароль:";
            OldPass.Visibility = Visibility.Hidden;

            btn3.Visibility = Visibility.Visible;
            btn3.Content = "Добавить";
            LblRole.Visibility = Visibility.Visible;
            RoleBox.Visibility = Visibility.Visible;
            Title = "Добавление пользователя";
        }

        private void NameBox_TextChanged(object sender, RoutedEventArgs e)
        {
            ValidTextBoxes[0] = Validation.ClientName(sender);
        }

        private void PhoneBox_TextChanged(object sender, RoutedEventArgs e)
        {
            ValidTextBoxes[1] = Validation.PhoneNomber(sender);
        }

        private void EmailBox_TextChanged(object sender, RoutedEventArgs e)
        {
            ValidTextBoxes[2] = Validation.Email(sender);
        }

        private void UnpBox_TextChanged(object sender, RoutedEventArgs e)
        {
            ValidTextBoxes[3] = Validation.Unp(sender);
        }

        private void UrAdressBox_TextChanged(object sender, RoutedEventArgs e)
        {
            ValidTextBoxes[4] = Validation.UrAdress(sender);
        }

        private void BankAccountBox_TextChanged(object sender, RoutedEventArgs e)
        {
            ValidTextBoxes[5] = Validation.BankAccount(sender);
        }

        private void ProfileNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidTextBoxes[6] = Validation.ProfileName(sender);
        }

        private void NewPass1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                if (passwordBox.Password.Length >= 5)
                {
                    passwordBox.ToolTip = null;
                    passwordBox.Background = null;
                }
                else
                {
                    passwordBox.Background = new SolidColorBrush(Properties.Settings.Default.NoValidColor);
                    passwordBox.ToolTip = "Длинна пароля должна быть больше 4 символов";
                }
            }
        }

        private void NewPass2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                if (passwordBox.Password != NewPass1.Password)
                {
                    passwordBox.Background = new SolidColorBrush(Properties.Settings.Default.NoValidColor);
                    passwordBox.ToolTip = "Пароли не одинаковы";
                }
                else
                {
                    passwordBox.ToolTip = null;
                    passwordBox.Background = null;
                }
            }
        }

        private void SaveClick(object sender, EventArgs e)
        {

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                string query = @"
                        UPDATE Clients
                        SET name = @Name,
                            phoneNomber = @PhoneNumber,
                            email = @Email,
                            unp = @UNP,
                            urAdress = @UrAdress,
                            bankAccount = @BankAccount
                        WHERE id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", idClient);
                    command.Parameters.AddWithValue("@Name", ClientNameBox.Text);
                    command.Parameters.AddWithValue("@PhoneNumber", PhoneNomberBox.Text);
                    command.Parameters.AddWithValue("@Email", EmailBox.Text);
                    command.Parameters.AddWithValue("@UNP", string.IsNullOrEmpty(UnpBox.Text) ? DBNull.Value : (object)UnpBox.Text);
                    command.Parameters.AddWithValue("@UrAdress", string.IsNullOrEmpty(UrAdressBox.Text) ? DBNull.Value : (object)UrAdressBox.Text);
                    command.Parameters.AddWithValue("@BankAccount", string.IsNullOrEmpty(BankAccountBox.Text) ? DBNull.Value : (object)BankAccountBox.Text);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        MessageBox.Show("Данные успешно обновлены");
                }
            }
        }

        private void СhangePass(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                string query = $"SELECT pass FROM Users WHERE id = {idUser}";
                string changeQuery = $"UPDATE Users SET pass = @pass WHERE id = {idUser}";
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                string oldpass = command.ExecuteScalar().ToString();

                if (oldpass == OldPass.Password)
                {
                    if (NewPass1.Password == NewPass2.Password)
                    {
                        if (NewPass1.Password.Length >= 5)
                        {
                            MessageBoxResult result = MessageBox.Show($"Желаете продолжить\n" +
                                $"Будет выполнен выход из программы для повторного входа", "Успешно", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                SqlCommand changeCommad = new SqlCommand(changeQuery, connection);
                                changeCommad.Parameters.AddWithValue("@pass", NewPass1.Password);
                                changeCommad.ExecuteNonQuery();

                                new LoginWindow().Show();

                                foreach (Window window in Application.Current.Windows)
                                    if (window.GetType() != typeof(LoginWindow))
                                        window.Close();
                            }
                        }
                        else
                            MessageBox.Show("Длинна пароля должна быть больше 4 символов");
                    }
                    else
                        MessageBox.Show("Введен разный пароль");
                }
                else
                    MessageBox.Show("Не верно введен старый пароль");
            }
        }

        private void CreateAccount(object sender, EventArgs e)
        {
            if (Array.Exists(ValidTextBoxes, element => element == false))
            {
                MessageBox.Show("Не все поля заполнены правильно\n" + String.Join("; ", ValidTextBoxes), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (NewPass1.Password != NewPass2.Password)
            {
                MessageBox.Show("Введен разный пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (NewPass1.Password.Length < 5)
            {
                MessageBox.Show("Длинна пароля должна быть больше 4 символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.VacTimeDBConnectionString))
            {
                int newClientId;
                string queryClient = @"
                            INSERT INTO Clients (name, phoneNomber, email, unp, urAdress, bankAccount)
                            VALUES (@Name, @PhoneNumber, @Email, @UNP, @UrAdress, @BankAccount); SELECT SCOPE_IDENTITY();";
                string queryUser = @"
                            INSERT INTO Users (name, Roles_id, pass, Clients_id)
                            VALUES (@Name, @Roles_id, @Pass, @Clients_id);";

                connection.Open();
                using (SqlCommand command = new SqlCommand(queryClient, connection))
                {
                    command.Parameters.AddWithValue("@Name", ClientNameBox.Text);
                    command.Parameters.AddWithValue("@PhoneNumber", PhoneNomberBox.Text);
                    command.Parameters.AddWithValue("@Email", EmailBox.Text);
                    command.Parameters.AddWithValue("@UNP", string.IsNullOrEmpty(UnpBox.Text) ? DBNull.Value : (object)UnpBox.Text);
                    command.Parameters.AddWithValue("@UrAdress", string.IsNullOrEmpty(UrAdressBox.Text) ? DBNull.Value : (object)UrAdressBox.Text);
                    command.Parameters.AddWithValue("@BankAccount", string.IsNullOrEmpty(BankAccountBox.Text) ? DBNull.Value : (object)BankAccountBox.Text);

                    newClientId = Convert.ToInt32(command.ExecuteScalar());
                }
                using (SqlCommand command = new SqlCommand(queryUser, connection))
                {
                    command.Parameters.AddWithValue("@Name", ProfileNameBox.Text);
                    command.Parameters.AddWithValue("@Roles_id", RoleBox.SelectedIndex + 1);
                    command.Parameters.AddWithValue("@Pass", NewPass1.Password);
                    command.Parameters.AddWithValue("@Clients_id", newClientId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        MessageBox.Show($"Успешно");
                    Close();
                }
            }
        }

        private void CloseClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;

namespace VacuumCraft
{
    internal class Validation
    {
        public static bool Name(object sender)
        {
            if (sender is TextBox textbox)
            {
                if (textbox.Text.Length > 128)
                {
                    textbox.Background = new SolidColorBrush(Properties.Settings.Default.NoValidColor);
                    textbox.ToolTip = "не более 128 символов";
                    return false;
                }
                else
                {
                    textbox.ToolTip = null;
                    textbox.Background = null;
                    return true;
                }
            }
            else
                throw new FormatException("is not textbox");
        }

        public static bool PhoneNomber(object sender)
        {
            if (sender is TextBox textbox)
            {
                if (!Regex.IsMatch(textbox.Text, @"^\+?[1-9]\d{1,14}$"))
                {
                    textbox.Background = new SolidColorBrush(Properties.Settings.Default.NoValidColor);
                    textbox.ToolTip = "не верный формат";
                    return false;
                }
                else
                {
                    textbox.ToolTip = null;
                    textbox.Background = null;
                    return true;
                }
            }
            else
                throw new FormatException("is not textbox");
        }

        public static bool Email(object sender)
        {
            if (sender is TextBox textbox)
            {
                if (!Regex.IsMatch(textbox.Text, @"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$"))
                {
                    textbox.Background = new SolidColorBrush(Properties.Settings.Default.NoValidColor);
                    textbox.ToolTip = "не верный формат";
                    return false;
                }
                else
                {
                    textbox.ToolTip = null;
                    textbox.Background = null;
                    return true;
                }
            }
            else
                throw new FormatException("is not textbox");
        }

        public static bool Unp(object sender)
        {
            if (sender is TextBox textbox)
            {
                if (!Regex.IsMatch(textbox.Text, @"^$|^\d{10}$"))
                {
                    textbox.Background = new SolidColorBrush(Properties.Settings.Default.NoValidColor);
                    textbox.ToolTip = "строго 10 цифр";
                    return false;
                }
                else
                {
                    textbox.ToolTip = null;
                    textbox.Background = null;
                    return true;
                }
            }
            else
                throw new FormatException("is not textbox");
        }

        public static bool UrAdress(object sender)
        {
            if (sender is TextBox textbox)
            {
                if (textbox.Text.Length > 45)
                {
                    textbox.Background = new SolidColorBrush(Properties.Settings.Default.NoValidColor);
                    textbox.ToolTip = "не более 45 символов";
                    return false;
                }
                else
                {
                    textbox.ToolTip = null;
                    textbox.Background = null;
                    return true;
                }
            }
            else
                throw new FormatException("is not textbox");
        }

        public static bool BankAccount(object sender)
        {
            if (sender is TextBox textbox)
            {
                if (textbox.Text.Length > 45)
                {
                    textbox.Background = new SolidColorBrush(Properties.Settings.Default.NoValidColor);
                    textbox.ToolTip = "не более 45 символов";
                    return false;
                }
                else
                {
                    textbox.ToolTip = null;
                    textbox.Background = null;
                    return true;
                }
            }
            else
                throw new FormatException("is not textbox");
        }

        public static bool ProfileName(object sender)
        {
            if (sender is TextBox textbox)
            {
                if (!Regex.IsMatch(textbox.Text, @"^[a-zA-Z0-9]{1,45}$"))
                {
                    textbox.Background = new SolidColorBrush(Properties.Settings.Default.NoValidColor);
                    textbox.ToolTip = "Только из букв латинского алфавита и цифр, не содержит пробелов и не превышает 45 символов";
                    return false;
                }
                else
                {
                    textbox.ToolTip = null;
                    textbox.Background = null;
                    return true;
                }
            }
            else
                throw new FormatException("is not textbox");
        }

        public static bool Path(object sender, bool IsFile)
        {
            if (sender is TextBox textbox)
            {
                if (IsFile)
                {
                    if (!File.Exists(textbox.Text)) 
                    {
                        textbox.Background = new SolidColorBrush(Properties.Settings.Default.NoValidColor);
                        textbox.ToolTip = "Файл не сушествует на данном ПК";
                        return false;
                    }
                    else
                    {
                        textbox.ToolTip = null;
                        textbox.Background = null;
                        return true;
                    }
                }
                else
                {
                    if (!Directory.Exists(textbox.Text)) 
                    {
                        textbox.Background = new SolidColorBrush(Properties.Settings.Default.NoValidColor);
                        textbox.ToolTip = "Директория не сушествует на данном ПК";
                        return false;
                    }
                    else
                    {
                        textbox.ToolTip = null;
                        textbox.Background = null;
                        return true;
                    }
                }

            }
            else
                throw new FormatException("is not textbox");
        }

        public static bool Price(object sender)
        {
            if (sender is TextBox textbox)
            {
                if (string.IsNullOrWhiteSpace(textbox.Text))
                {
                    textbox.Background = new SolidColorBrush(Properties.Settings.Default.NoValidColor);
                    textbox.ToolTip = "Не может быть пустым";
                    return false;
                }
                if (!Regex.IsMatch(textbox.Text, @"^\d{1,8}(\.\d{1,2})?$"))
                {
                    textbox.Background = new SolidColorBrush(Properties.Settings.Default.NoValidColor);
                    textbox.ToolTip = "Допустимы числа с не более чем двумя знаками после запятой.\nЦелая часть должна содержать от 1 до 8 цифр.";
                    return false;
                }
                else
                {
                    textbox.ToolTip = null;
                    textbox.Background = null;
                    return true;
                }
            }
            else
                throw new FormatException("is not textbox");
        }

        public static bool Description(object sender)
        {
            if (sender is TextBox textbox)
            {
                if (string.IsNullOrWhiteSpace(textbox.Text))
                {
                    textbox.Background = new SolidColorBrush(Properties.Settings.Default.NoValidColor);
                    textbox.ToolTip = "Строка пуста, null или состоит только из пробельных символов.";
                    return false;
                }
                else
                {
                    textbox.ToolTip = null;
                    textbox.Background = null;
                    return true;
                }
            }
            else
                throw new FormatException("is not textbox");
        }
    }
}

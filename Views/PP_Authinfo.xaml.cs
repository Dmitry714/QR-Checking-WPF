using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QR_Checking_winVersion
{
    /// <summary>
    /// Логика взаимодействия для PP_Authinfo.xaml
    /// </summary>
    public partial class PP_Authinfo : Page
    {
        private static DataClass DataClass;
        private static MainPage MainPage;
        private static Query Query;
        private static ProfilePage ProfilePage;
        CustomMessage message;
        public PP_Authinfo(DataClass dataClass, MainPage mainPage, Query query, ProfilePage profilePage)
        {
            InitializeComponent();
            DataClass = dataClass;
            MainPage = mainPage;
            Query = query;
            ProfilePage = profilePage;
        }

        private async void SaveData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainPage.OptionsBar.IsEnabled = false;
                MainGrid.IsEnabled = false;
                Loading.Visibility = Visibility.Visible;

                bool checkConn = await InternetConnectionChecker.InternetChecking();
                if (checkConn)
                {
                    if (LoginUser.Text != DataClass.Login)
                    {
                        bool checkLogin = await Query.SelectUsersLogin(LoginUser.Text);
                        if (checkLogin)
                        {
                            updateUserData();
                        }
                        else
                        {
                            message = new CustomMessage("Логин уже занят!", "Ошибка", false, 3);
                            message.ShowDialog();
                        }
                    }
                    else
                    {
                        updateUserData();
                    }
                }
                else
                {
                    message = new CustomMessage("Нет доступа в интернет", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                message = new CustomMessage("Что-то пошло не так!", "Упс!", false, 3);
                message.ShowDialog();
            }
            finally
            {
                MainPage.OptionsBar.IsEnabled = true;
                MainGrid.IsEnabled = true;
                Loading.Visibility = Visibility.Hidden;
            }
        }

        private async void updateUserData()
        {
            if (!string.IsNullOrWhiteSpace(LoginUser.Text) && !string.IsNullOrWhiteSpace(NewPasswordUser.Password) && !string.IsNullOrWhiteSpace(OldPasswordUser.Password))
            {
                if (LoginUser.Text.Length > 8 && NewPasswordUser.Password.Length > 8 && OldPasswordUser.Password.Length > 8)
                {
                    string oldPass = SHA256Converter.ConvertToSHA256(OldPasswordUser.Password);
                    if (oldPass == DataClass.Password)
                    {
                        string newPass = SHA256Converter.ConvertToSHA256(NewPasswordUser.Password);
                        bool result = await Query.UpdateUsersAuthData(DataClass.ID_User, LoginUser.Text, newPass);
                        if (result)
                        {
                            DataClass.Login = LoginUser.Text;
                            message = new CustomMessage("Данные успешно сохраненны", "Сохранение", false, 2);
                            message.ShowDialog();

                            OldPasswordUser.Password = null;
                            NewPasswordUser.Password = null;
                        }
                        else
                        {
                            message = new CustomMessage("Не удалось сохранить данные", "Ошибка", false, 3);
                            message.ShowDialog();
                        }
                    }
                    else
                    {
                        message = new CustomMessage("Пароль не верный", "Ошибка", false, 3);
                        message.ShowDialog();
                    }
                }
                else
                {
                    message = new CustomMessage("Логин и пароль должны быть больше 8 символов", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            else
            {
                message = new CustomMessage("Проверьте корректность введенных данных! Каждая строка должна быть заполнена!", "Ошибка", false, 3);
                message.ShowDialog();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoginUser.Text = DataClass.Login;
        }

        private void Back_Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.MainFrame.Content = ProfilePage;
        }

        private void OldPasswordUser_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void NewPasswordUser_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void LoginUser_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void LoginUser_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Z0-9_-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OldPasswordUser_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^a-zA-Z0-9_@#%+-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NewPasswordUser_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^a-zA-Z0-9_@#%+-]");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}

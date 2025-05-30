using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace QR_Checking_winVersion
{
    public partial class MainWindow : Window
    {
        private static CustomMessage message;
        private static Query query;
        private string emailCode;
        public MainWindow()
        {
            InitializeComponent();
            query = new Query();
        }

        private void Close_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Hide_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void logo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private async void enter_button_Click(object sender, RoutedEventArgs e)
        {           
            try
            {
                AuthControls.IsEnabled = false;
                enter_button.Visibility = Visibility.Hidden;
                Loading_Image.Visibility = Visibility.Visible;

                bool checkInternet = await InternetConnectionChecker.InternetChecking();
                if (checkInternet)
                {
                    bool checkconn = QueryConnection.IsConnectionOpen();
                    if (checkconn)
                    {
                        string password = SHA256Converter.ConvertToSHA256(_Password.Password);
                        DataClass dataClass = await query.SelectFromUsers(_Login.Text, password);
                        
                        if (dataClass != null)
                        {
                            MainPage mainPage = new MainPage(dataClass, query);
                            this.Close();
                            mainPage.Show();
                        }
                    }
                    else
                    {
                        ConnectFail();
                    }
                }
                else
                {
                    AuthControls.Visibility = Visibility.Hidden;
                    ConnError.Visibility = Visibility.Visible;
                    RefreshButtonInternetChecking.Visibility = Visibility.Visible;
                    Connection_Image.Visibility = Visibility.Hidden;
                    message = new CustomMessage("Нет доступа в интернет", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                message = new CustomMessage("Произошла неизвестная ошибка", "Ошибка", false, 3);
                message.ShowDialog();
            }
            finally
            {
                AuthControls.IsEnabled = true;
                enter_button.Visibility = Visibility.Visible;
                Loading_Image.Visibility = Visibility.Hidden;
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenConnect();
        }

        private void RefreshButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ConnError.Visibility = Visibility.Hidden;
            Connection_Image.Visibility = Visibility.Visible;
            RefreshButton.Visibility = Visibility.Hidden;
            OpenConnect();
        }

        private async void OpenConnect()
        {
            AuthControls.Visibility = Visibility.Hidden;
            ConnError.Visibility = Visibility.Hidden;
            RefreshButton.Visibility = Visibility.Hidden;
            Connection_Image.Visibility = Visibility.Visible;

            bool check = await QueryConnection.OpenConnection();

            if (check)
            {
                ConnectSuccess();
            }
            else
            {
                ConnectFail();
            }
        }

        private void ConnectSuccess()
        {
            AuthControls.Visibility = Visibility.Visible;
            Connection_Image.Visibility = Visibility.Hidden;
        }

        private void ConnectFail()
        {
            AuthControls.Visibility = Visibility.Hidden;
            ConnError.Visibility = Visibility.Visible;
            RefreshButton.Visibility = Visibility.Visible;
            Connection_Image.Visibility = Visibility.Hidden;
            message = new CustomMessage("Не удалось установить соединение с сервером", "Ошибка", false, 3);
            message.ShowDialog();
        }


        private async void RefreshButtonInternetChecking_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AuthControls.IsEnabled = true;
            ConnError.Visibility = Visibility.Hidden;
            RefreshButtonInternetChecking.Visibility = Visibility.Hidden;
            Connection_Image.Visibility = Visibility.Visible;

            bool checkInternet = await InternetConnectionChecker.InternetChecking();
            if (checkInternet)
            {
                AuthControls.Visibility = Visibility.Visible;
                Connection_Image.Visibility = Visibility.Hidden;
                enter_button.Visibility = Visibility.Visible;
                Loading_Image.Visibility = Visibility.Hidden;
            }
            else
            {
                AuthControls.Visibility = Visibility.Hidden;
                ConnError.Visibility = Visibility.Visible;
                RefreshButtonInternetChecking.Visibility = Visibility.Visible;
                Connection_Image.Visibility = Visibility.Hidden;
                message = new CustomMessage("Нет доступа в интернет", "Ошибка", false, 3);
                message.ShowDialog();
            }
        }

        private void hasAccountLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RegisterControls.Visibility = Visibility.Hidden;
            AuthControls.Visibility = Visibility.Visible;
            _Login.Text = "";
            _Password.Password = "";
        }



        private void hasNoAccount_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AuthControls.Visibility = Visibility.Hidden;
            RegisterControls.Visibility = Visibility.Visible;
            _EmailReg.Text = "";
            _LoginReg.Text = "";
            _PasswordReg.Password = "";
            _PasswordRegRepeat.Password = "";
        }

        private void forgetPass_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AuthControls.Visibility = Visibility.Hidden;
            ForgetPassControls.Visibility = Visibility.Visible;
            FillEmail.Text = "";
        }

        private void back_label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ForgetPassControls.Visibility = Visibility.Hidden;
            AuthControls.Visibility = Visibility.Visible;
        }



        private void Cancel_Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AcceptEmailControls.Visibility = Visibility.Hidden;
            AuthControls.Visibility = Visibility.Visible;
            _Login.Text = "";
            _Password.Password = "";
        }

        private async void reg_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                reg_button.Visibility = Visibility.Hidden;
                Loading_ImageReg.Visibility = Visibility.Visible;
                RegisterControls.IsEnabled = false;
                if (!string.IsNullOrWhiteSpace(_LoginReg.Text) && !string.IsNullOrWhiteSpace(_EmailReg.Text) && !string.IsNullOrWhiteSpace(_PasswordReg.Password) && !string.IsNullOrWhiteSpace(_PasswordRegRepeat.Password))
                {
                    if (_LoginReg.Text.Length > 8 && _PasswordReg.Password.Length > 8 && _PasswordRegRepeat.Password.Length > 8)
                    {
                        bool checkLogin = await query.SelectUsersLogin(_LoginReg.Text);
                        if (checkLogin)
                        {
                            if (_PasswordReg.Password == _PasswordRegRepeat.Password)
                            {
                                if (IsEmailValid(_EmailReg.Text))
                                {
                                    int checkEmail = await query.CheckEmail(_EmailReg.Text);
                                    if (checkEmail == 0)
                                    {
                                        emailCode = RandomText(8);
                                        string subject = "Код подтверждения";
                                        string body = $"Код подтверждения для регистрации в прилоложении QR CHECKING: {emailCode}";
                                        bool sendCode = await MailSender.SendEmail(subject, body, _EmailReg.Text);
                                        if (sendCode)
                                        {
                                            RegisterControls.Visibility = Visibility.Hidden;
                                            AcceptEmailControls.Visibility = Visibility.Visible;
                                        }
                                        else
                                        {
                                            RegisterControls.Visibility = Visibility.Hidden;
                                            AuthControls.Visibility = Visibility.Visible;
                                        }
                                    }
                                    else
                                    {
                                        message = new CustomMessage("Аккаунт с таким Email уже существует", "Ошибка", false, 3);
                                        message.ShowDialog();
                                    }
                                }
                                else
                                {
                                    message = new CustomMessage("Данный Email введен не корректно", "Ошибка", false, 3);
                                    message.ShowDialog();
                                }
                            }
                            else
                            {
                                message = new CustomMessage("Пароли не сходятся", "Ошибка", false, 3);
                                message.ShowDialog();
                            }
                        }
                        else
                        {
                            message = new CustomMessage("Логин уже занят!", "Ошибка", false, 3);
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
                    message = new CustomMessage("Проверьте корректность введенных данных", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                message = new CustomMessage("Что-то пошло не так", "Ошибка", false, 3);
                message.ShowDialog();
            }
            finally
            {
                reg_button.Visibility = Visibility.Visible;
                Loading_ImageReg.Visibility = Visibility.Hidden;
                RegisterControls.IsEnabled = true;
            }
        }

        public static string RandomText(int length)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                int index = rnd.Next(chars.Length);
                sb.Append(chars[index]);
            }
            return sb.ToString();
        }

        public static string RandomLogin(int length)
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                int index = rnd.Next(chars.Length);
                sb.Append(chars[index]);
            }
            return sb.ToString();
        }

        public static string RandomPassword(int length)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ%$#@!*";
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                int index = rnd.Next(chars.Length);
                sb.Append(chars[index]);
            }
            return sb.ToString();
        }

        private async void Accept_Code_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AcceptEmailControls.IsEnabled = false;
                Accept_Code.Visibility = Visibility.Hidden;
                Loading_ImageAE.Visibility = Visibility.Visible;

                bool checkInternet = await InternetConnectionChecker.InternetChecking();
                if (checkInternet)
                {
                    if (FillCode.Text == emailCode)
                    {
                        string password = SHA256Converter.ConvertToSHA256(_PasswordReg.Password);

                        Query query = new Query();
                        bool checkRegister = await query.RegisterUser(_LoginReg.Text, _EmailReg.Text, password);
                        if (checkRegister)
                        {
                            message = new CustomMessage("Аккаунт успешно зарегистрирован", "Регистрация", false, 2);
                            message.ShowDialog();
                            AcceptEmailControls.Visibility = Visibility.Hidden;
                            AuthControls.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            message = new CustomMessage("Не удалось зарегестрировать аккаунт", "Ошибка", false, 3);
                            message.ShowDialog();
                            AcceptEmailControls.Visibility = Visibility.Hidden;
                            AuthControls.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        message = new CustomMessage("Код не верный", "Ошибка", false, 3);
                        message.ShowDialog();
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
                message = new CustomMessage("Что-то пошло не так", "Ошибка", false, 3);
                message.ShowDialog();
            }
            finally
            {
                AcceptEmailControls.IsEnabled = true;
                Accept_Code.Visibility = Visibility.Visible;
                Loading_ImageAE.Visibility = Visibility.Hidden;
            }
        }

        private bool IsEmailValid(string email)
        {
            string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        private void _LoginReg_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Z0-9_-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void _EmailReg_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void _PasswordReg_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^a-zA-Z0-9_@#%+-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void _PasswordRegRepeat_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^a-zA-Z0-9_@#%+-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void _LoginReg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void _EmailReg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void _PasswordReg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void _PasswordRegRepeat_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void FillEmail_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void _Login_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Z0-9_-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void _Login_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void _Password_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^a-zA-Z0-9_@#%+-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void FillEmail_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void _Password_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private async void send_password_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ForgetPassControls.IsEnabled = false;
                send_password.Visibility = Visibility.Hidden;
                Loading_ImageFP.Visibility = Visibility.Visible;

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    Query query = new Query();
                    int ID_User = await query.CheckEmail(FillEmail.Text);
                    if (ID_User != 0)
                    {
                        string newlogin = RandomLogin(8);
                        string newpassword = RandomPassword(8);
                        string convertNewPassword = SHA256Converter.ConvertToSHA256(newpassword);
                        bool sendEmail = await MailSender.SendEmail("Восстановление аккаунта", $"Ваши новые данные от аккаунта: Логин: {newlogin}, Пароль: {newpassword}", FillEmail.Text);
                        if (sendEmail)
                        {
                            bool passUpdate = await query.UpdateUserData(ID_User, newlogin, convertNewPassword);
                            if (passUpdate)
                            {
                                message = new CustomMessage("Дальнейшие инструкции отправлены на почту", "Проверьте почту", false, 2);
                                message.ShowDialog();
                                ForgetPassControls.Visibility = Visibility.Hidden;
                                AuthControls.Visibility = Visibility;
                            }
                            else
                            {
                                message = new CustomMessage("Что-то пошло не так", "Ошибка", false, 3);
                                message.ShowDialog();
                            }
                        }
                    }
                    else
                    {
                        message = new CustomMessage("Пользователя с таким Email не существует", "Ошибка", false, 3);
                        message.ShowDialog();
                    }
                }
                else
                {
                    message = new CustomMessage("Нет доступа в интернет", "Ошибка", false, 3);
                    message.ShowDialog();
                    ForgetPassControls.Visibility = Visibility.Hidden;
                    AuthControls.Visibility = Visibility;
                }
            }
            catch (Exception)
            {
                message = new CustomMessage("Что-то пошло не так", "Ошибка", false, 3);
                message.ShowDialog();
            }
            finally
            {
                ForgetPassControls.IsEnabled = true;
                send_password.Visibility = Visibility.Visible;
                Loading_ImageFP.Visibility = Visibility.Hidden;
            }
        }
    }
}
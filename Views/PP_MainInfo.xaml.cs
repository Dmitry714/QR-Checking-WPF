using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QR_Checking_winVersion
{
    /// <summary>
    /// Логика взаимодействия для PP_MainInfo.xaml
    /// </summary>
    public partial class PP_MainInfo : Page
    {
        private static DataClass DataClass;
        private static MainPage MainPage;
        private static Query Query;
        private static ProfilePage ProfilePage;
        private static CustomMessage message;
        private string emailCode;
        public PP_MainInfo(DataClass dataClass, MainPage mainPage, Query query, ProfilePage profilePage)
        {
            InitializeComponent();
            DataClass = dataClass;
            MainPage = mainPage;
            Query = query;
            ProfilePage = profilePage;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            confirmCode.Visibility = Visibility.Collapsed;
            UserData.Visibility = Visibility.Visible;
            FirstNameUser.Text = DataClass.Name;
            SurnameUser.Text = DataClass.Surname;
            Patronymic.Text = DataClass.Patronymic;
            PhoneNumber.Text = DataClass.Phone_Number;
            Email.Text = DataClass.Email;
            Specialization.Text = DataClass.Specialization;
        }

        private void Back_Button_MouseDown(object sender, MouseButtonEventArgs e)
        {            
            MainPage.MainFrame.Content = ProfilePage;
        }

        private async void saveUserInfo()
        {
            try
            {
                bool answer = await Query.UpdateUsersMainData(DataClass.ID_User, FirstNameUser.Text, SurnameUser.Text, Patronymic.Text, Email.Text, PhoneNumber.Text, Specialization.Text);
                if (answer)
                {
                    DataClass.Name = FirstNameUser.Text;
                    DataClass.Surname = SurnameUser.Text;
                    DataClass.Patronymic = Patronymic.Text;
                    DataClass.Email = Email.Text;
                    DataClass.Phone_Number = PhoneNumber.Text;
                    DataClass.Specialization = Specialization.Text;
                    message = new CustomMessage("Данные успешно сохраненны", "Сохранено", false, 2);
                    message.ShowDialog();
                }
                else
                {
                    message = new CustomMessage("Не удалось сохранить данные", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                message = new CustomMessage("Что-то пошло не так!", "Упс!", false, 3);
                message.ShowDialog();
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

        private async void SaveData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainGrid.IsEnabled = false;
                MainPage.OptionsBar.IsEnabled = false;
                Loading.Visibility = Visibility.Visible;

                bool checkInternet = await InternetConnectionChecker.InternetChecking();
                if (checkInternet)
                {
                    if (!string.IsNullOrWhiteSpace(FirstNameUser.Text) && !string.IsNullOrWhiteSpace(SurnameUser.Text) && !string.IsNullOrWhiteSpace(Patronymic.Text) && !string.IsNullOrWhiteSpace(Email.Text) && !string.IsNullOrWhiteSpace(PhoneNumber.Text) && !string.IsNullOrWhiteSpace(Specialization.Text))
                    {
                        if (Email.Text != DataClass.Email && DataClass.Phone_Number != PhoneNumber.Text)
                        {
                            if (IsEmailValid(Email.Text) && IsPhoneNumberValid(PhoneNumber.Text))
                            {
                                int checkEmail = await Query.CheckEmail(Email.Text);
                                if (checkEmail == 0)
                                {
                                    emailCode = RandomText(8);
                                    string subject = "Код подтверждения";
                                    string body = $"Код подтверждения для смены Email в приложении QR CHECKING: {emailCode}";

                                    bool sendEmail = await MailSender.SendEmail(subject, body, Email.Text);
                                    if (sendEmail)
                                    {
                                        UserData.Visibility = Visibility.Collapsed;
                                        confirmCode.Visibility = Visibility.Visible;
                                        EmailCodeText.Text = "";
                                    }
                                }
                                else
                                {
                                    message = new CustomMessage("Данный Email уже занят", "Ошибка", false, 3);
                                    message.ShowDialog();
                                }
                            }
                            else
                            {
                                message = new CustomMessage("Данный Email или номер введены некорректно", "Ошибка", false, 3);
                                message.ShowDialog();
                            }
                        }
                        else if (Email.Text != DataClass.Email)
                        {
                            if (IsEmailValid(Email.Text))
                            {
                                int checkEmail = await Query.CheckEmail(Email.Text);
                                if (checkEmail == 0)
                                {
                                    emailCode = RandomText(8);
                                    string subject = "Код подтверждения";
                                    string body = $"Код подтверждения для смены Email в приложении QR CHECKING: {emailCode}";

                                    bool sendEmail = await MailSender.SendEmail(subject, body, Email.Text);
                                    if (sendEmail)
                                    {
                                        UserData.Visibility = Visibility.Collapsed;
                                        confirmCode.Visibility = Visibility.Visible;
                                        EmailCodeText.Text = "";
                                    }
                                }
                                else
                                {
                                    message = new CustomMessage("Данный Email уже занят", "Ошибка", false, 3);
                                    message.ShowDialog();
                                }
                            }
                            else
                            {
                                message = new CustomMessage("Данный Email введен некорректно", "Ошибка", false, 3);
                                message.ShowDialog();
                            }
                        }
                        else if (DataClass.Phone_Number != PhoneNumber.Text)
                        {
                            if (IsPhoneNumberValid(PhoneNumber.Text))
                            {
                                saveUserInfo();
                            }
                            else
                            {
                                message = new CustomMessage("Данный номер введен некорректно", "Ошибка", false, 3);
                                message.ShowDialog();
                            }
                        }
                        else
                        {
                            saveUserInfo();
                        }
                    }
                    else
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных. Строки не должны быть пустыми", "Ошибка", false, 3);
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
                message = new CustomMessage("Что-то пошло не так!", "Упс!", false, 3);
                message.ShowDialog();
            }
            finally
            {
                MainGrid.IsEnabled = true;
                MainPage.OptionsBar.IsEnabled = true;
                Loading.Visibility = Visibility.Hidden;
            }      
        }

        private bool IsPhoneNumberValid(string phoneNumber)
        {
            string pattern = @"^\+375\d{9}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(phoneNumber);
        }

        private void FirstNameUser_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Za-яА-Я]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SurnameUser_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Za-яА-Я]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Patronymic_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Za-яА-Я]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^+0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Email_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void FirstNameUser_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void SurnameUser_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Patronymic_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void PhoneNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Email_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private bool IsEmailValid(string email)
        {
            string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";

            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        private async void ConfirmEmailButton_Click(object sender, RoutedEventArgs e)
        {           
            try
            {
                MainGrid.IsEnabled = false;
                MainPage.OptionsBar.IsEnabled = false;
                Loading.Visibility = Visibility.Visible;
                bool checkConnection = await InternetConnectionChecker.InternetChecking();
                if (checkConnection)
                {
                    if (emailCode == EmailCodeText.Text)
                    {
                        saveUserInfo();
                        UserData.Visibility = Visibility.Visible;
                        confirmCode.Visibility = Visibility.Collapsed;
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
                message = new CustomMessage("Что-то пошло не так!", "Упс!", false, 3);
                message.ShowDialog();
            }
            finally
            {
                MainGrid.IsEnabled = true;
                MainPage.OptionsBar.IsEnabled = true;
                Loading.Visibility = Visibility.Hidden;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UserData.Visibility = Visibility.Visible;
            confirmCode.Visibility = Visibility.Collapsed;
        }

        private void Specialization_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Za-яА-Я,-.]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Specialization_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QR_Checking_winVersion
{
    /// <summary>
    /// Логика взаимодействия для AP_UsersStud.xaml
    /// </summary>
    public partial class AP_UsersStud : Page
    {
        private static CustomMessage message;
        private static Query query;
        private static AdminPanelPage AdminPanelPage;
        public AP_UsersStud(AdminPanelPage adminPanelPage)
        {
            InitializeComponent();
            query = new Query();
            AdminPanelPage = adminPanelPage;
        }

        public async Task FillDataGrid()
        {
            try
            {
                StateLabel.Visibility = Visibility.Collapsed;

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    Query query = new Query();
                    DataTable dataTable = null;

                    dataTable = await query.SelectUsersStudFullData();

                    if (dataTable != null)
                    {
                        dataGrid.ItemsSource = dataTable.DefaultView;
                        dataGrid.Columns[0].Header = "ID учащегося";
                        dataGrid.Columns[1].Header = "Имя учащегося";
                        dataGrid.Columns[2].Header = "Модель телефона";
                        dataGrid.Columns[3].Header = "Номер телефона";
                        dataGrid.Columns[4].Header = "Идентификатор приложения";
                        dataGrid.Columns[5].Header = "Email";
                        dataGrid.Columns[6].Header = "Логин";
                        dataGrid.Columns[7].Header = "Пароль";
                        dataGrid.Columns[8].Header = "Номер группы";
                    }
                    else
                    {
                        StateLabel.Visibility = Visibility.Visible;
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
                message = new CustomMessage("Что-то пошло не так", "Упс!", false, 3);
                message.ShowDialog();
            }
        }

        private void ClearTextBoxesButton_Click(object sender, RoutedEventArgs e)
        {
            IdGroupUsersStud.SelectedIndex = -1;
            ActiveUsersStud.SelectedIndex = -1;
            CleanTextBoxes.Clear(this);
        }

        private void ApplyFiltersUserStud()
        {
            DataView dataView = (DataView)dataGrid.ItemsSource;
            StringBuilder filterBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(UserFullNameUsersStudFilter.Text))
            {
                filterBuilder.Append($"UserFullName LIKE '%{UserFullNameUsersStudFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(PhoneModelUsersStudFilter.Text))
            {
                filterBuilder.Append($"Phone_Model LIKE '%{PhoneModelUsersStudFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(PhoneNumberUsersStudFilter.Text))
            {
                filterBuilder.Append($"Phone_Number LIKE '%{PhoneNumberUsersStudFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(AppIdUsersStudFilter.Text))
            {
                filterBuilder.Append($"App_ID LIKE '%{AppIdUsersStudFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(EmailUsersStudFilter.Text))
            {
                filterBuilder.Append($"Email LIKE '%{EmailUsersStudFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(LoginUsersStudFilter.Text))
            {
                filterBuilder.Append($"Login LIKE '%{LoginUsersStudFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(GroupNumberUsersStudFilter.Text))
            {
                filterBuilder.Append($"Group_Number LIKE '%{GroupNumberUsersStudFilter.Text}%' AND ");
            }

            if (filterBuilder.Length >= 5)
            {
                filterBuilder.Remove(filterBuilder.Length - 5, 5);
            }

            string filterExpression = filterBuilder.ToString();
            dataView.RowFilter = filterExpression;
            dataGrid.ItemsSource = dataView;
        }

        private void PageFilters(object sender, TextChangedEventArgs e)
        {
            ApplyFiltersUserStud();
        }

        private void SpaceInputBlock(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void IdUsersStud_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NameUsersStud_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^а-яА-Яa-zA-Z]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SurnameUsersStud_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^а-яА-Яa-zA-Z]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PatronymicUsersStud_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^а-яА-Яa-zA-Z]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PhoneModelUsersStud_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9а-яА-Яa-zA-Z:.+-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PhoneNumberUsersStud_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^+0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void AppIdUsersStud_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9а-яА-Яa-zA-Z:.-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void EmailUsersStud_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void LoginUsersStud_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9a-zA-Z:.-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PasswordUsersStud_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^a-zA-Z0-9_@#%+-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void IdGroupUsersStud_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        public async Task<bool> FillingGroups()
        {
            IdGroupUsersStud.Items.Clear();
            IdGroupUsersStud.SelectedIndex = -1;
            List<string> groups = await query.SelectFromGroups();
            if (groups != null)
            {
                foreach (string group in groups)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = group
                    };
                    IdGroupUsersStud.Items.Add(item);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private int IdGroup()
        {

            Regex regex = new Regex(@"^(\d+)");
            MatchCollection matches = regex.Matches(IdGroupUsersStud.Text);
            int numValue = 0;
            foreach (Match match in matches)
            {
                int.TryParse(match.Value, out int Result);
                numValue = Result;
            }
            return numValue;
        }

        private async void InsertUsersStud_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (!string.IsNullOrWhiteSpace(NameUsersStud.Text) || !string.IsNullOrWhiteSpace(SurnameUsersStud.Text) || !string.IsNullOrWhiteSpace(PhoneModelUsersStud.Text) || !string.IsNullOrWhiteSpace(PhoneNumberUsersStud.Text) || !string.IsNullOrWhiteSpace(EmailUsersStud.Text) || !string.IsNullOrWhiteSpace(LoginUsersStud.Text) || !string.IsNullOrWhiteSpace(PasswordUsersStud.Text) || !string.IsNullOrWhiteSpace(IdGroupUsersStud.Text) || !string.IsNullOrWhiteSpace(ActiveUsersStud.Text))
                    {
                        if (IsEmailValid(EmailUsersStud.Text))
                        {
                            if (IsPhoneNumberValid(PhoneNumberUsersStud.Text))
                            {
                                if (LoginUsersStud.Text.Length > 8 || PasswordUsersStud.Text.Length > 8)
                                {
                                    bool checkLogin = await query.SelectUsersStudLogin(LoginUsersStud.Text);
                                    if (checkLogin)
                                    {
                                        int checkEmail = await query.CheckEmailStud(EmailUsersStud.Text);
                                        if (checkEmail == 0)
                                        {
                                            string pass = SHA256Converter.ConvertToSHA256(PasswordUsersStud.Text);
                                            bool result = await query.InsertUserStud(NameUsersStud.Text, SurnameUsersStud.Text, PatronymicUsersStud.Text, PhoneModelUsersStud.Text, PhoneNumberUsersStud.Text, EmailUsersStud.Text, LoginUsersStud.Text, pass, IdGroup(), ActiveUsersStud.Text);
                                            if (result)
                                            {
                                                await FillDataGrid();
                                                CleanTextBoxes.Clear(this);
                                                IdGroupUsersStud.SelectedIndex = -1;
                                                ActiveUsersStud.SelectedIndex = -1;
                                                message = new CustomMessage("Данные добавлены", "Выполнено", false, 2);
                                                message.ShowDialog();
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
                                message = new CustomMessage("Данный номер введен не корректно", "Ошибка", false, 3);
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
                        message = new CustomMessage("Проверьте корректность введенных данных", "Ошибка", false, 2);
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
                message = new CustomMessage("Что-то пошло не так", "Упс!", false, 3);
                message.ShowDialog();
            }
            finally
            {
                AdminPanelPage.LoadingPageDone();
            }
        }

        private async void UpdateUsersStud_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (!string.IsNullOrWhiteSpace(IdUsersStud.Text))
                    {
                        if (IsEmailValid(EmailUsersStud.Text) || string.IsNullOrEmpty(EmailUsersStud.Text))
                        {
                            if (IsPhoneNumberValid(PhoneNumberUsersStud.Text) || string.IsNullOrEmpty(PhoneNumberUsersStud.Text))
                            {
                                if ((LoginUsersStud.Text.Length > 8 || string.IsNullOrEmpty(LoginUsersStud.Text)) && (PasswordUsersStud.Text.Length > 8 || string.IsNullOrEmpty(PasswordUsersStud.Text)))
                                {
                                    bool checkLogin = await query.SelectUsersStudLogin(LoginUsersStud.Text);
                                    if (checkLogin || string.IsNullOrEmpty(LoginUsersStud.Text))
                                    {
                                        int checkEmail = await query.CheckEmailStud(EmailUsersStud.Text);
                                        if (checkEmail == 0 || string.IsNullOrEmpty(EmailUsersStud.Text))
                                        {
                                            string pass = SHA256Converter.ConvertToSHA256(PasswordUsersStud.Text);
                                            bool result = await query.UpdateUserStud(int.Parse(IdUsersStud.Text), NameUsersStud.Text, SurnameUsersStud.Text, PatronymicUsersStud.Text, PhoneModelUsersStud.Text, PhoneNumberUsersStud.Text, EmailUsersStud.Text, LoginUsersStud.Text, pass, IdGroup(), ActiveUsersStud.Text);
                                            if (result)
                                            {
                                                await FillDataGrid();
                                                CleanTextBoxes.Clear(this);
                                                IdGroupUsersStud.SelectedIndex = -1;
                                                ActiveUsersStud.SelectedIndex = -1;
                                                message = new CustomMessage("Данные обновлены", "Выполнено", false, 2);
                                                message.ShowDialog();
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
                                message = new CustomMessage("Данный номер введен не корректно", "Ошибка", false, 3);
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
                        message = new CustomMessage("Проверьте корректность введенных данных. Заполните ID учащегося", "Ошибка", false, 2);
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
                message = new CustomMessage("Что-то пошло не так", "Упс!", false, 3);
                message.ShowDialog();
            }
            finally
            {
                AdminPanelPage.LoadingPageDone();
            }
        }

        private async void DeleteUsersStud_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();
                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (string.IsNullOrWhiteSpace(IdUsersStud.Text))
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных", "Ошибка", false, 2);
                        message.ShowDialog();
                    }
                    else
                    {
                        bool result = await query.DeleteUserStud(int.Parse(IdUsersStud.Text));
                        if (result)
                        {
                            await FillDataGrid();
                            CleanTextBoxes.Clear(this);
                            IdGroupUsersStud.SelectedIndex = -1;
                            ActiveUsersStud.SelectedIndex = -1;
                            message = new CustomMessage("Данные успешно удалены", "Выполнено", false, 2);
                            message.ShowDialog();
                        }
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
                message = new CustomMessage("Что-то пошло не так", "Упс!", false, 3);
                message.ShowDialog();
            }
            finally
            {
                AdminPanelPage.LoadingPageDone();
            }
        }

        private bool IsEmailValid(string email)
        {
            string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        private bool IsPhoneNumberValid(string phoneNumber)
        {
            string pattern = @"^\+375\d{9}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(phoneNumber);
        }


        private async void DeleteAppID_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();
                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (string.IsNullOrWhiteSpace(IdUsersStud.Text))
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных", "Ошибка", false, 2);
                        message.ShowDialog();
                    }
                    else
                    {
                        bool result = await query.SetAppIDToNull(int.Parse(IdUsersStud.Text));
                        if (result)
                        {
                            await FillDataGrid();
                            CleanTextBoxes.Clear(this);
                            IdGroupUsersStud.SelectedIndex = -1;
                            ActiveUsersStud.SelectedIndex = -1;
                            message = new CustomMessage("Идентификатор приложения был удален", "Выполнено", false, 2);
                            message.ShowDialog();
                        }
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
                message = new CustomMessage("Что-то пошло не так", "Упс!", false, 3);
                message.ShowDialog();
            }
            finally
            {
                AdminPanelPage.LoadingPageDone();
            }
        }
    }
}

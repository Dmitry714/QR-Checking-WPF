using System;
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
    /// Логика взаимодействия для AP_UsersTeach.xaml
    /// </summary>
    public partial class AP_UsersTeach : Page
    {
        private static CustomMessage message;
        private static Query query;
        private static AdminPanelPage AdminPanelPage;
        public AP_UsersTeach(AdminPanelPage adminPanelPage)
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

                    dataTable = await query.SelectUsersTeachFullData();

                    if (dataTable != null)
                    {
                        dataGrid.ItemsSource = dataTable.DefaultView;
                        dataGrid.Columns[0].Header = "ID преподователя";
                        dataGrid.Columns[1].Header = "Имя преподователя";
                        dataGrid.Columns[2].Header = "Email";
                        dataGrid.Columns[3].Header = "Номер телефона";
                        dataGrid.Columns[4].Header = "Специализация";
                        dataGrid.Columns[5].Header = "Роль";
                        dataGrid.Columns[6].Header = "Логин";
                        dataGrid.Columns[7].Header = "Пароль";
                        dataGrid.Columns[8].Header = "Активно";
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
            ActiveUsersTeach.SelectedItem = -1;
            RoleUsersTeach.SelectedIndex = -1;
            CleanTextBoxes.Clear(this);
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

        private void ApplyFiltersUserTeach()
        {
            DataView dataView = (DataView)dataGrid.ItemsSource;
            StringBuilder filterBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(FullNameUsersTeachFilter.Text))
            {
                filterBuilder.Append($"FullName LIKE '%{FullNameUsersTeachFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(EmailUsersTeachFilter.Text))
            {
                filterBuilder.Append($"Email LIKE '%{EmailUsersTeachFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(PhoneNumberUsersTeachFilter.Text))
            {
                filterBuilder.Append($"Phone_Number LIKE '%{PhoneNumberUsersTeachFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(SpecializationUsersTeachFilter.Text))
            {
                filterBuilder.Append($"Specialization LIKE '%{SpecializationUsersTeachFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(RoleUsersTeachFilter.Text))
            {
                filterBuilder.Append($"Role LIKE '%{RoleUsersTeachFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(LoginUsersTeachFilter.Text))
            {
                filterBuilder.Append($"Login LIKE '%{LoginUsersTeachFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(ActiveUsersTeachFilter.Text))
            {
                filterBuilder.Append($"Enable LIKE '%{ActiveUsersTeachFilter.Text}%' AND ");
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
            ApplyFiltersUserTeach();
        }

        private void SpaceInputBlock(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void IdUsersTeach_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NameUsersTeach_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^а-яА-Яa-zA-Z]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SurnameUsersTeach_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^а-яА-Яa-zA-Z]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PatronymicUsersTeach_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^а-яА-Яa-zA-Z]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void EmailUsersTeach_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PhoneNumberUsersTeach_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^+0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SpecializationUsersTeach_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^а-яА-Яa-zA-Z-,.]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void LoginUsersTeach_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9a-zA-Z:.-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PasswordUsersTeach_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^a-zA-Z0-9_@#%+-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void InsertUsersTeach_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (!string.IsNullOrWhiteSpace(NameUsersTeach.Text) || !string.IsNullOrWhiteSpace(SurnameUsersTeach.Text) || !string.IsNullOrWhiteSpace(EmailUsersTeach.Text) || !string.IsNullOrWhiteSpace(PhoneNumberUsersTeach.Text) || !string.IsNullOrWhiteSpace(SpecializationUsersTeach.Text) || !string.IsNullOrWhiteSpace(RoleUsersTeach.Text) || !string.IsNullOrWhiteSpace(LoginUsersTeach.Text) || !string.IsNullOrWhiteSpace(PasswordUsersTeach.Text) || !string.IsNullOrWhiteSpace(ActiveUsersTeach.Text))
                    {
                        if (IsEmailValid(EmailUsersTeach.Text))
                        {
                            if (IsPhoneNumberValid(PhoneNumberUsersTeach.Text))
                            {
                                if (LoginUsersTeach.Text.Length > 8 || PasswordUsersTeach.Text.Length > 8)
                                {
                                    bool checkLogin = await query.SelectUsersStudLogin(LoginUsersTeach.Text);
                                    if (checkLogin)
                                    {
                                        int checkEmail = await query.CheckEmailStud(EmailUsersTeach.Text);
                                        if (checkEmail == 0)
                                        {
                                            string pass = SHA256Converter.ConvertToSHA256(PasswordUsersTeach.Text);
                                            bool result = await query.InsertUserTeach(NameUsersTeach.Text, SurnameUsersTeach.Text, PatronymicUsersTeach.Text, EmailUsersTeach.Text, PhoneNumberUsersTeach.Text, SpecializationUsersTeach.Text, RoleUsersTeach.Text, LoginUsersTeach.Text, pass, ActiveUsersTeach.Text);
                                            if (result)
                                            {
                                                await FillDataGrid();
                                                CleanTextBoxes.Clear(this);
                                                ActiveUsersTeach.SelectedItem = -1;
                                                RoleUsersTeach.SelectedIndex = -1;
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

        private async void UpdateUsersTeach_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (!string.IsNullOrWhiteSpace(IdUsersTeach.Text))
                    {
                        if (IsEmailValid(EmailUsersTeach.Text) || string.IsNullOrEmpty(EmailUsersTeach.Text))
                        {
                            if (IsPhoneNumberValid(PhoneNumberUsersTeach.Text) || string.IsNullOrEmpty(PhoneNumberUsersTeach.Text))
                            {
                                if ((string.IsNullOrEmpty(LoginUsersTeach.Text) || LoginUsersTeach.Text.Length > 8) && (string.IsNullOrEmpty(PasswordUsersTeach.Text) || PasswordUsersTeach.Text.Length > 8))
                                {
                                    bool checkLogin = await query.SelectUsersLogin(LoginUsersTeach.Text);
                                    if (checkLogin || string.IsNullOrEmpty(LoginUsersTeach.Text))
                                    {
                                        int checkEmail = await query.CheckEmail(EmailUsersTeach.Text);
                                        if (checkEmail == 0 || string.IsNullOrEmpty(EmailUsersTeach.Text))
                                        {
                                            string pass = SHA256Converter.ConvertToSHA256(PasswordUsersTeach.Text);
                                            bool result = await query.UpdateUserTeach(int.Parse(IdUsersTeach.Text), NameUsersTeach.Text, SurnameUsersTeach.Text, PatronymicUsersTeach.Text, EmailUsersTeach.Text, PhoneNumberUsersTeach.Text, SpecializationUsersTeach.Text, RoleUsersTeach.Text, LoginUsersTeach.Text, pass, ActiveUsersTeach.Text);
                                            if (result)
                                            {
                                                await FillDataGrid();
                                                CleanTextBoxes.Clear(this);
                                                ActiveUsersTeach.SelectedItem = -1;
                                                RoleUsersTeach.SelectedIndex = -1;
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
                        message = new CustomMessage("Проверьте корректность введенных данных. Заполните ID преподавателя", "Ошибка", false, 2);
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

        private async void DisableUsersTeach_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (!string.IsNullOrWhiteSpace(IdUsersTeach.Text))
                    {

                        bool result = await query.DisableUser(int.Parse(IdUsersTeach.Text));
                        if (result)
                        {
                            await FillDataGrid();
                            CleanTextBoxes.Clear(this);
                            ActiveUsersTeach.SelectedItem = -1;
                            RoleUsersTeach.SelectedIndex = -1;
                            message = new CustomMessage("Преподаватель деактивирован", "Выполнено", false, 2);
                            message.ShowDialog();
                        }
                    }
                    else
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных. Заполните ID преподавателя", "Ошибка", false, 2);
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
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QR_Checking_winVersion
{
    /// <summary>
    /// Логика взаимодействия для AP_Attendance.xaml
    /// </summary>
    public partial class AP_Attendance : Page
    {
        private static CustomMessage message;
        private static Query query;
        private static AdminPanelPage AdminPanelPage;
        public AP_Attendance(AdminPanelPage adminPanelPage)
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
                    DataTable dataTable = null;

                    AttendanceFilter.Visibility = Visibility.Visible;

                    dataTable = await query.SelectAttendanceFullData();

                    if (dataTable != null)
                    {
                        dataGrid.ItemsSource = dataTable.DefaultView;
                        dataGrid.Columns[0].Header = "ID Посещения";
                        dataGrid.Columns[1].Header = "Имя учащегося";
                        dataGrid.Columns[2].Header = "Номер группы";
                        dataGrid.Columns[3].Header = "Название события";
                        dataGrid.Columns[4].Header = "Дата посещения";
                        dataGrid.Columns[5].Header = "Статус посещения";
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

        private void ApplyFiltersAttendance()
        {
            DataView dataView = (DataView)dataGrid.ItemsSource;
            StringBuilder filterBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(UserFullNameAttendanceFilter.Text))
            {
                filterBuilder.Append($"UserFullName LIKE '%{UserFullNameAttendanceFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(GroupNumberAttendanceFilter.Text))
            {
                filterBuilder.Append($"Group_Number LIKE '%{GroupNumberAttendanceFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(EventNameAttendanceFilter.Text))
            {
                filterBuilder.Append($"Event_Name LIKE '%{EventNameAttendanceFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(AttendanceDateAttendanceFilter.Text))
            {
                filterBuilder.Append($"Attendance_date LIKE '%{AttendanceDateAttendanceFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(AttendanceStatucAttendanceFilter.Text))
            {
                filterBuilder.Append($"Attendance_Status LIKE '%{AttendanceStatucAttendanceFilter.Text}%' AND ");
            }

            if (filterBuilder.Length >= 5)
            {
                filterBuilder.Remove(filterBuilder.Length - 5, 5);
            }

            string filterExpression = filterBuilder.ToString();
            dataView.RowFilter = filterExpression;
            dataGrid.ItemsSource = dataView;
        }

        private void ClearTextBoxesButton_Click(object sender, RoutedEventArgs e)
        {
            IdUserStudAttendance.SelectedIndex = -1;
            IdEventAttendance.SelectedIndex = -1;
            CleanTextBoxes.Clear(this);
        }


        private void ApplyFilters(object sender, TextChangedEventArgs e)
        {
            ApplyFiltersAttendance();
        }

        private void SpaceBlockInput(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void IdAttendance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }


        private void AttendanceDateAttendance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9.:-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void AttendanceStatucAttendance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9а-яА-Яa-zA-Z:.-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private int IdUserStud()
        {

            Regex regex = new Regex(@"^(\d+)");
            MatchCollection matches = regex.Matches(IdUserStudAttendance.Text);
            int numValue = 0;
            foreach (Match match in matches)
            {
                int.TryParse(match.Value, out int Result);
                numValue = Result;
            }
            return numValue;
        }

        private int IdEvent()
        {
            Regex regex = new Regex(@"^(\d+)");
            MatchCollection matches = regex.Matches(IdEventAttendance.Text);
            int numValue = 0;
            foreach (Match match in matches)
            {
                int.TryParse(match.Value, out int Result);
                numValue = Result;
            }
            return numValue;
        }

        private async void InsertAttendance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (string.IsNullOrWhiteSpace(IdUserStudAttendance.Text) || string.IsNullOrWhiteSpace(IdEventAttendance.Text) || string.IsNullOrWhiteSpace(AttendanceDateAttendance.Text) || string.IsNullOrWhiteSpace(AttendanceStatucAttendance.Text))
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных", "Ошибка", false, 2);
                        message.ShowDialog();
                    }
                    else
                    {
                        string dateString = AttendanceDateAttendance.Text;
                        string format = "dd.MM.yyyy HH:mm:ss";
                        string formattedDate = "";

                        DateTime resultDateTime;
                        if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out resultDateTime))
                        {
                            formattedDate = resultDateTime.ToString("yyyy-MM-dd HH:mm:ss");

                            DateTime resultDate = DateTime.ParseExact(formattedDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                            bool result = await query.insertAttendance(IdUserStud(), IdEvent(), resultDate, AttendanceStatucAttendance.Text);
                            if (result)
                            {
                                await FillDataGrid();
                                CleanTextBoxes.Clear(this);
                                IdUserStudAttendance.SelectedIndex = -1;
                                IdEventAttendance.SelectedIndex = -1;
                                message = new CustomMessage("Данные успешно добавлены", "Выполнено", false, 2);
                                message.ShowDialog();
                            }
                        }
                        else
                        {
                            message = new CustomMessage("Неверный формат даты и времени", "Ошибка", false, 3);
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

        public async Task<bool> FillingUsers()
        {
            IdUserStudAttendance.Items.Clear();

            List<string> groups = await query.SelectUsersForAttendace();
            if (groups != null)
            {
                foreach (string group in groups)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = group
                    };
                    IdUserStudAttendance.Items.Add(item);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> FillingEvents()
        {
            IdEventAttendance.Items.Clear();

            List<string> groups = await query.SelectEventsForAttendace();
            if (groups != null)
            {
                foreach (string group in groups)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = group
                    };
                    IdEventAttendance.Items.Add(item);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AttendanceDateAttendance_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string input = textBox.Text;

                string digitsOnly = new string(input.Where(char.IsDigit).ToArray());

                if (digitsOnly.Length > 2)
                {
                    digitsOnly = digitsOnly.Insert(2, ".");
                }
                if (digitsOnly.Length > 5)
                {
                    digitsOnly = digitsOnly.Insert(5, ".");
                }
                if (digitsOnly.Length > 10)
                {
                    digitsOnly = digitsOnly.Insert(10, " ");
                }
                if (digitsOnly.Length > 13)
                {
                    digitsOnly = digitsOnly.Insert(13, ":");
                }
                if (digitsOnly.Length > 16)
                {
                    digitsOnly = digitsOnly.Insert(16, ":");
                }

                textBox.Text = digitsOnly;
                textBox.CaretIndex = digitsOnly.Length;
            }
        }

        private async void UpdateAttendance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (string.IsNullOrWhiteSpace(IdUserStudAttendance.Text) || string.IsNullOrWhiteSpace(IdEventAttendance.Text) || string.IsNullOrWhiteSpace(AttendanceDateAttendance.Text) || string.IsNullOrWhiteSpace(AttendanceStatucAttendance.Text))
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных", "Ошибка", false, 2);
                        message.ShowDialog();
                    }
                    else
                    {
                        string dateString = AttendanceDateAttendance.Text;
                        string format = "dd.MM.yyyy HH:mm:ss";
                        string formattedDate = "";

                        DateTime resultDateTime;
                        if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out resultDateTime))
                        {
                            formattedDate = resultDateTime.ToString("yyyy-MM-dd HH:mm:ss");

                            DateTime resultDate = DateTime.ParseExact(formattedDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                            bool result = await query.updateAttendance(int.Parse(IdAttendance.Text), IdUserStud(), IdEvent(), resultDate, AttendanceStatucAttendance.Text);
                            if (result)
                            {
                                await FillDataGrid();
                                CleanTextBoxes.Clear(this);
                                IdUserStudAttendance.SelectedIndex = -1;
                                IdEventAttendance.SelectedIndex = -1;
                                message = new CustomMessage("Данные успешно изменены", "Выполнено", false, 2);
                                message.ShowDialog();
                            }
                        }
                        else
                        {
                            message = new CustomMessage("Неверный формат даты и времени", "Ошибка", false, 3);
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

        private async void DeleteAttendance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();
                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (string.IsNullOrWhiteSpace(IdAttendance.Text))
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных", "Ошибка", false, 2);
                        message.ShowDialog();
                    }
                    else
                    {
                        bool result = await query.deleteAttendance(int.Parse(IdAttendance.Text));
                        if (result)
                        {
                            await FillDataGrid();
                            CleanTextBoxes.Clear(this);
                            IdUserStudAttendance.SelectedIndex = -1;
                            IdEventAttendance.SelectedIndex = -1;
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
    }
}

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
    /// Логика взаимодействия для AP_Events.xaml
    /// </summary>
    public partial class AP_Events : Page
    {
        private static CustomMessage message;
        private static Query query;
        private static AdminPanelPage AdminPanelPage;
        public AP_Events(AdminPanelPage adminPanelPage)
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

                    dataTable = await query.SelectFromEventsFullData();

                    if (dataTable != null)
                    {
                        dataGrid.ItemsSource = dataTable.DefaultView;
                        dataGrid.Columns[0].Header = "ID события";
                        dataGrid.Columns[1].Header = "Имя преподавателя";
                        dataGrid.Columns[2].Header = "Номер группы";
                        dataGrid.Columns[3].Header = "Название события";
                        dataGrid.Columns[4].Header = "Место события";
                        dataGrid.Columns[5].Header = "Начало события";
                        dataGrid.Columns[6].Header = "Конец события";
                        dataGrid.Columns[7].Header = "Активно";
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

        private void ApplyFiltersEvents()
        {
            DataView dataView = (DataView)dataGrid.ItemsSource;
            StringBuilder filterBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(TeachNameEventsFilter.Text))
            {
                filterBuilder.Append($"Teach_Name LIKE '%{TeachNameEventsFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(GroupNumberEventsFilter.Text))
            {
                filterBuilder.Append($"Group_Number LIKE '%{GroupNumberEventsFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(EventNameEventsFilter.Text))
            {
                filterBuilder.Append($"Event_Name LIKE '%{EventNameEventsFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(EventLocationEventsFilter.Text))
            {
                filterBuilder.Append($"Event_Location LIKE '%{EventLocationEventsFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(EventBeginEventsFilter.Text))
            {
                filterBuilder.Append($"Event_Begin LIKE '%{EventBeginEventsFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(EventEndEventsFilter.Text))
            {
                filterBuilder.Append($"Event_End LIKE '%{EventEndEventsFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(IsActiveEventsFilter.Text))
            {
                filterBuilder.Append($"IsActive LIKE '%{IsActiveEventsFilter.Text}%' AND ");
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
            CleanTextBoxes.Clear(this);
        }

        private void ApplyFilters(object sender, TextChangedEventArgs e)
        {
            ApplyFiltersEvents();
        }

        private void IdEventEvents_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9.]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SpaceBlockInput(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void EventNameEvents_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9а-яА-Яa-zA-Z.-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void EventLocationEvents_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Za-яА-Я0-9,. ]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void DisableEvents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (string.IsNullOrWhiteSpace(IdEventEvents.Text))
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных. Заполните ID события", "Ошибка", false, 2);
                        message.ShowDialog();
                    }
                    else
                    {
                        bool result = await query.UpdateEventState_IdEvent(int.Parse(IdEventEvents.Text));
                        if (result)
                        {
                            await FillDataGrid();
                            CleanTextBoxes.Clear(this);
                            message = new CustomMessage("Событие деактивировано", "Выполнено", false, 2);
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

        private async void UpdateEvents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (string.IsNullOrWhiteSpace(IdEventEvents.Text))
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных", "Ошибка", false, 2);
                        message.ShowDialog();
                    }
                    else
                    {
                        bool result = await query.UpdateEventData(int.Parse(IdEventEvents.Text), EventNameEvents.Text, EventLocationEvents.Text);
                        if (result)
                        {
                            await FillDataGrid();
                            CleanTextBoxes.Clear(this);
                            message = new CustomMessage("Данные успешно изменены", "Выполнено", false, 2);
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

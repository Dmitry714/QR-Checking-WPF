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
    /// Логика взаимодействия для AP_Groups.xaml
    /// </summary>
    public partial class AP_Groups : Page
    {
        private static CustomMessage message;
        private static Query query;
        private static AdminPanelPage AdminPanelPage;
        public AP_Groups(AdminPanelPage adminPanelPage)
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

                    dataTable = await query.SelectGroupsFullData();

                    if (dataTable != null)
                    {
                        dataGrid.ItemsSource = dataTable.DefaultView;
                        dataGrid.Columns[0].Header = "ID группы";
                        dataGrid.Columns[1].Header = "Номер группы";
                        dataGrid.Columns[2].Header = "Специальность";
                        dataGrid.Columns[3].Header = "Куратор";
                        dataGrid.Columns[4].Header = "Аудитория";
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
            CleanTextBoxes.Clear(this);
        }

        private void ApplyFiltersGroups()
        {
            DataView dataView = (DataView)dataGrid.ItemsSource;
            StringBuilder filterBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(GroupNumberGroupsFilter.Text))
            {
                filterBuilder.Append($"Group_Number LIKE '%{GroupNumberGroupsFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(SpecialitiesGroupsFilter.Text))
            {
                filterBuilder.Append($"Specialities LIKE '%{SpecialitiesGroupsFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(CuratorFullNameGroupsFilter.Text))
            {
                filterBuilder.Append($"CuratorFullName LIKE '%{CuratorFullNameGroupsFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(ClassroomGroupsFilter.Text))
            {
                filterBuilder.Append($"Classroom LIKE '%{ClassroomGroupsFilter.Text}%' AND ");
            }
            
            if (filterBuilder.Length >= 5)
            {
                filterBuilder.Remove(filterBuilder.Length - 5, 5);
            }

            string filterExpression = filterBuilder.ToString();
            dataView.RowFilter = filterExpression;
            dataGrid.ItemsSource = dataView;
        }

        private void ApplyFilters(object sender, TextChangedEventArgs e)
        {
            ApplyFiltersGroups();
        }

        private void SpaceBlockInput(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void IdGroupGroups_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void GroupNumberGroups_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void SpecialitiesGroups_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9а-яА-Яa-zA-Z.-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CuratorGroups_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ClassRoomGroups_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }
       
        public async Task<bool> FillingCurators()
        {
            CuratorGroups.Items.Clear();

            List<string> curators = await query.SelectCurators();
            if (curators != null)
            {
                foreach (string curator in curators)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = curator
                    };
                    CuratorGroups.Items.Add(item);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        private int IdCurator()
        {
            Regex regex = new Regex(@"^(\d+)");
            MatchCollection matches = regex.Matches(CuratorGroups.Text);
            int numValue = 0;
            foreach (Match match in matches)
            {
                int.TryParse(match.Value, out int Result);
                numValue = Result;
            }
            return numValue;
        }

        private async void InsertGroups_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (!string.IsNullOrWhiteSpace(GroupNumberGroups.Text) || !string.IsNullOrWhiteSpace(SpecialitiesGroups.Text) || !string.IsNullOrWhiteSpace(CuratorGroups.Text) || !string.IsNullOrWhiteSpace(ClassRoomGroups.Text))
                    {
                        bool result = await query.insertGroups(GroupNumberGroups.Text, SpecialitiesGroups.Text, IdCurator(), ClassRoomGroups.Text);
                        if (result)
                        {
                            await FillDataGrid();
                            CleanTextBoxes.Clear(this);
                            message = new CustomMessage("Данные добавлены", "Выполнено", false, 2);
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

        private async void UpdateGroups_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (!string.IsNullOrWhiteSpace(IdGroupGroups.Text))
                    {
                        bool result = await query.UpdateGroups(int.Parse(IdGroupGroups.Text), GroupNumberGroups.Text, SpecialitiesGroups.Text, IdCurator(), ClassRoomGroups.Text);
                        if (result)
                        {
                            await FillDataGrid();
                            CleanTextBoxes.Clear(this);
                            message = new CustomMessage("Данные изменены", "Выполнено", false, 2);
                            message.ShowDialog();
                        }
                    }
                    else
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных. Заполните ID группы", "Ошибка", false, 2);
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

        private async void DeleteGroups_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (!string.IsNullOrWhiteSpace(IdGroupGroups.Text))
                    {
                        bool result = await query.DeleteGroups(int.Parse(IdGroupGroups.Text));
                        if (result)
                        {
                            await FillDataGrid();
                            CleanTextBoxes.Clear(this);
                            message = new CustomMessage("Данные удалены", "Выполнено", false, 2);
                            message.ShowDialog();
                        }
                    }
                    else
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных. Заполните ID группы", "Ошибка", false, 2);
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

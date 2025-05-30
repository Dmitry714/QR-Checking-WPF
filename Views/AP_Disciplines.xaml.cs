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
    /// Логика взаимодействия для AP_Disciplines.xaml
    /// </summary>
    public partial class AP_Disciplines : Page
    {
        private static CustomMessage message;
        private static Query query;
        private static AdminPanelPage AdminPanelPage;
        public AP_Disciplines(AdminPanelPage adminPanelPage)
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

                    dataTable = await query.SelectFromDisciplineFullData();

                    if (dataTable != null)
                    {
                        dataGrid.ItemsSource = dataTable.DefaultView;
                        dataGrid.Columns[0].Header = "ID предмета";
                        dataGrid.Columns[1].Header = "Название предмета";
                        dataGrid.Columns[2].Header = "Номер группы";
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


        private void ApplyFiltersDisciplines()
        {
            DataView dataView = (DataView)dataGrid.ItemsSource;
            StringBuilder filterBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(DisciplineNameDisciplinesFilter.Text))
            {
                filterBuilder.Append($"Discipline_Name LIKE '%{DisciplineNameDisciplinesFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(GroupNumberDisciplinesFilter.Text))
            {
                filterBuilder.Append($"Group_number LIKE '%{GroupNumberDisciplinesFilter.Text}%' AND ");
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
            ApplyFiltersDisciplines();
        }

        private void IdDiscipline_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9.]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SpaceInputBlock(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void DisciplineNameDisciplines_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9а-яА-Яa-zA-Z.-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private int IdGroup()
        {
            Regex regex = new Regex(@"^(\d+)");
            MatchCollection matches = regex.Matches(IdGroupDisciplines.Text);
            int numValue = 0;
            foreach (Match match in matches)
            {
                int.TryParse(match.Value, out int Result);
                numValue = Result;
            }
            return numValue;
        }


        private async void InsertDisciplines_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (string.IsNullOrWhiteSpace(DisciplineNameDisciplines.Text) || string.IsNullOrWhiteSpace(IdGroupDisciplines.Text))
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных", "Ошибка", false, 2);
                        message.ShowDialog();
                    }
                    else
                    {
                        bool result = await query.insertDiscipline(DisciplineNameDisciplines.Text, IdGroup());
                        if (result)
                        {
                            await FillDataGrid();
                            CleanTextBoxes.Clear(this);
                            IdGroupDisciplines.SelectedIndex = -1;
                            message = new CustomMessage("Данные успешно добавлены", "Выполнено", false, 2);
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

        private async void UpdateDisciplines_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (string.IsNullOrWhiteSpace(IdDiscipline.Text) || string.IsNullOrWhiteSpace(DisciplineNameDisciplines.Text) || string.IsNullOrWhiteSpace(IdGroupDisciplines.Text))
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных", "Ошибка", false, 2);
                        message.ShowDialog();
                    }
                    else
                    {
                        bool result = await query.updateDiscipline(int.Parse(IdDiscipline.Text), DisciplineNameDisciplines.Text, IdGroup());
                        if (result)
                        {
                            await FillDataGrid();
                            CleanTextBoxes.Clear(this);
                            IdGroupDisciplines.SelectedIndex = -1;
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

        private async void DeleteDisciplines_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();
                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (string.IsNullOrWhiteSpace(IdDiscipline.Text))
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных", "Ошибка", false, 2);
                        message.ShowDialog();
                    }
                    else
                    {
                        bool result = await query.deleteDiscipline(int.Parse(IdDiscipline.Text));
                        if (result)
                        {
                            await FillDataGrid();
                            CleanTextBoxes.Clear(this);
                            IdGroupDisciplines.SelectedIndex = -1;
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

        private void ClearTextBoxesButton_Click(object sender, RoutedEventArgs e)
        {
            IdGroupDisciplines.SelectedIndex = -1;
            CleanTextBoxes.Clear(this);
        }

        public async Task<bool> FillingGroups()
        {
            IdGroupDisciplines.Items.Clear();

            List<string> groups = await query.SelectFromGroups();
            if (groups != null)
            {
                foreach (string group in groups)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = group
                    };
                    IdGroupDisciplines.Items.Add(item);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}


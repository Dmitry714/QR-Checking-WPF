using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QR_Checking_winVersion
{
    /// <summary>
    /// Логика взаимодействия для SP_Events.xaml
    /// </summary>
    public partial class SP_Events : Page
    {
        private static CustomMessage message;
        private static Query query;
        public SP_Events()
        {
            InitializeComponent();
            query = new Query();
        }

        public async Task FillDataGrid()
        {
            try
            {
                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    StateLabel.Visibility = Visibility.Collapsed;
                    DataTable dataTable = null;                    
                    dataTable = await query.SelectFromEventsFullData();

                    if (dataTable != null)
                    {
                        dataGrid.ItemsSource = dataTable.DefaultView;
                        dataGrid.Columns[0].Header = "ID";
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

        private void ApplyFilters(object sender, TextChangedEventArgs e)
        {
            ApplyFiltersEvents();
        }

        private void ApplyFiltersEvents()
        {
            DataView dataView = (DataView)dataGrid.ItemsSource;
            StringBuilder filterBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(NameFilterEvents.Text))
            {
                filterBuilder.Append($"Teach_Name LIKE '%{NameFilterEvents.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(GroupFilterEvents.Text))
            {
                filterBuilder.Append($"Group_Number LIKE '%{GroupFilterEvents.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(DisciplineNameFilterEvents.Text))
            {
                filterBuilder.Append($"Event_Name LIKE '%{DisciplineNameFilterEvents.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(EventPlaceFilter.Text))
            {
                filterBuilder.Append($"Event_Location LIKE '%{EventPlaceFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(EventBeginFilter.Text))
            {
                filterBuilder.Append($"Event_Begin LIKE '%{EventBeginFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(EventEndFilter.Text))
            {
                filterBuilder.Append($"Event_End LIKE '%{EventEndFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(EventStateFilter.Text))
            {
                filterBuilder.Append($"IsActive LIKE '%{EventStateFilter.Text}%' AND ");
            }

            if (filterBuilder.Length >= 5)
            {
                filterBuilder.Remove(filterBuilder.Length - 5, 5);
            }

            string filterExpression = filterBuilder.ToString();
            dataView.RowFilter = filterExpression;
            dataGrid.ItemsSource = dataView;
        }

        private void ClearTextBoxesButtonEvents_Click(object sender, RoutedEventArgs e)
        {
            CleanTextBoxes.Clear(this);
        }
    }
}

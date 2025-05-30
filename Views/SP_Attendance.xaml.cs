using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace QR_Checking_winVersion
{
    /// <summary>
    /// Логика взаимодействия для SP_Attendance.xaml
    /// </summary>
    public partial class SP_Attendance : Page
    {
        private static CustomMessage message;
        private static Query query;

        public SP_Attendance()
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
                    dataTable = await query.SelectAttendanceFullData();

                    if (dataTable != null)
                    {
                        dataGrid.ItemsSource = dataTable.DefaultView;
                        dataGrid.Columns[0].Header = "ID";
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

        private void ApplyFilters(object sender, TextChangedEventArgs e)
        {
            ApplyFiltersAttendance();
        }

        private void ApplyFiltersAttendance()
        {
            DataView dataView = (DataView)dataGrid.ItemsSource;
            StringBuilder filterBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(DisciplineNameFilter.Text))
            {
                filterBuilder.Append($"Event_Name LIKE '%{DisciplineNameFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(GroupFilter.Text))
            {
                filterBuilder.Append($"Group_Number LIKE '%{GroupFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(NameFilter.Text))
            {
                filterBuilder.Append($"UserFullName LIKE '%{NameFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(DateFilter.Text))
            {
                filterBuilder.Append($"Attendance_Date LIKE '%{DateFilter.Text}%' AND ");
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
    }
}

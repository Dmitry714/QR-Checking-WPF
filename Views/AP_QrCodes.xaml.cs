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
    /// Логика взаимодействия для AP_QrCodes.xaml
    /// </summary>
    public partial class AP_QrCodes : Page
    {
        private static CustomMessage message;
        private static Query query;
        private static AdminPanelPage AdminPanelPage;
        public AP_QrCodes(AdminPanelPage adminPanelPage)
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

                    dataTable = await query.SelectFromQrCodesFullData();

                    if (dataTable != null)
                    {
                        dataGrid.ItemsSource = dataTable.DefaultView;
                        dataGrid.Columns[0].Header = "ID кода";
                        dataGrid.Columns[1].Header = "Название события";
                        dataGrid.Columns[2].Header = "Преподаватель";
                        dataGrid.Columns[3].Header = "Информация в коде";
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

        private void ApplyFiltersQrCodes()
        {
            DataView dataView = (DataView)dataGrid.ItemsSource;
            StringBuilder filterBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(EventNameQrCodesFilter.Text))
            {
                filterBuilder.Append($"Event_Name LIKE '%{EventNameQrCodesFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(TeacherNameQrCodesFilter.Text))
            {
                filterBuilder.Append($"TeachFullName LIKE '%{TeacherNameQrCodesFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(QrCodeInfoQrCodesFilter.Text))
            {
                filterBuilder.Append($"QR_Code_info LIKE '%{QrCodeInfoQrCodesFilter.Text}%' AND ");
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
            ApplyFiltersQrCodes();
        }

        private void SpaceBlockInput(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void IdQrCodeQrCodes_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void DeleteQrCodes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (string.IsNullOrWhiteSpace(IdQrCodeQrCodes.Text))
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных. Заполните ID QR-кода", "Ошибка", false, 2);
                        message.ShowDialog();
                    }
                    else
                    {
                        bool result = await query.QrCodeDeleteQrID(int.Parse(IdQrCodeQrCodes.Text));
                        if (result)
                        {
                            await FillDataGrid();
                            CleanTextBoxes.Clear(this);
                            message = new CustomMessage("QR-код удален", "Выполнено", false, 2);
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

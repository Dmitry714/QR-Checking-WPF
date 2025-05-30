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
    /// Логика взаимодействия для AP_AppConfig.xaml
    /// </summary>
    public partial class AP_AppConfig : Page
    {
        private static CustomMessage message;
        private static Query query;
        private static AdminPanelPage AdminPanelPage;
        public AP_AppConfig(AdminPanelPage adminPanelPage)
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
                    using (DataTable dataTable = await query.SelectFromAppConfigFullData())
                    {                        
                        if (dataTable != null)
                        {
                            dataGrid.ItemsSource = dataTable.DefaultView;
                            dataGrid.Columns[0].Header = "ID параметра";
                            dataGrid.Columns[1].Header = "IP-адрес";
                            dataGrid.Columns[2].Header = "Telegram ID";
                        }
                        else
                        {
                            StateLabel.Visibility = Visibility.Visible;
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
        }

        private void ApplyFiltersAppConfig()
        {
            DataView dataView = (DataView)dataGrid.ItemsSource;
            StringBuilder filterBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(IpAppConfigFilter.Text))
            {
                filterBuilder.Append($"IP LIKE '%{IpAppConfigFilter.Text}%' AND ");
            }

            if (!string.IsNullOrEmpty(TgSupportAppConfigFilter.Text))
            {
                filterBuilder.Append($"Telegram_ID LIKE '%{TgSupportAppConfigFilter.Text}%' AND ");
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
            ApplyFiltersAppConfig();
        }

        private async void InsertAppConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (string.IsNullOrWhiteSpace(IpAppConfig.Text) || string.IsNullOrWhiteSpace(TgSupportAppConfig.Text))
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных", "Выполнено", false, 2);
                        message.ShowDialog();
                    }
                    else
                    {
                        if (IsIpValid(IpAppConfig.Text))
                        {
                            bool result = await query.insertAppConfig(IpAppConfig.Text, TgSupportAppConfig.Text);
                            if (result)
                            {
                                await FillDataGrid();
                                CleanTextBoxes.Clear(this);
                                message = new CustomMessage("Данные успешно добавлены", "Выполнено", false, 2);
                                message.ShowDialog();
                            }
                        }
                        else
                        {
                            message = new CustomMessage("Неверный формат IP-адреса", "Ошибка", false, 3);
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

        private async void UpdateAppConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();
                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (string.IsNullOrWhiteSpace(IdAppConfig.Text))
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных", "Выполнено", false, 2);
                        message.ShowDialog();
                    }
                    else
                    {
                        if (IsIpValid(IpAppConfig.Text) || string.IsNullOrEmpty(IpAppConfig.Text))
                        {
                            bool result = await query.updateAppConfig(int.Parse(IdAppConfig.Text), IpAppConfig.Text, TgSupportAppConfig.Text);
                            if (result)
                            {
                                await FillDataGrid();
                                CleanTextBoxes.Clear(this);
                                message = new CustomMessage("Данные успешно изменены", "Выполнено", false, 2);
                                message.ShowDialog();
                            }
                        }
                        else
                        {
                            message = new CustomMessage("Неверный формат IP-адреса", "Ошибка", false, 3);
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

        private async void DeleteAppConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminPanelPage.LoadingPageStart();
                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    if (string.IsNullOrWhiteSpace(IdAppConfig.Text))
                    {
                        message = new CustomMessage("Проверьте корректность введенных данных", "Выполнено", false, 2);
                        message.ShowDialog();
                    }
                    else
                    {                        
                        bool result = await query.deleteAppConfig(int.Parse(IdAppConfig.Text));
                        if (result)
                        {
                            await FillDataGrid();
                            CleanTextBoxes.Clear(this);
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

        private void IdAppConfig_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SpaceInputBlock(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void IpAppConfig_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9.]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TgSupportAppConfig_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9./@?=_+%&a-zA-Z-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private bool IsIpValid(string ip)
        {
            string pattern = @"^(\d{1,3}\.){3}\d{1,3}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(ip);
        }
    }
}

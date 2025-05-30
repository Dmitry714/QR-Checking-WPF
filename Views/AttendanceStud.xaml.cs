using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QR_Checking_winVersion
{
    /// <summary>
    /// Логика взаимодействия для AttendanceStud.xaml
    /// </summary>
    public partial class AttendanceStud : Page
    {
        private static DataClass DataClass;
        private static CustomMessage message;
        private static Query Query;
        private static MainPage MainPage;
        private int markedCount = 0;
        public AttendanceStud(DataClass dataClass, Query query, MainPage mainPage)
        {
            InitializeComponent();
            DataClass = dataClass;
            Query = query;
            MainPage = mainPage;
        }

        private async void FillDataTable()
        {            
            try
            {
                while (dataGrid.Columns.Count > 0)
                {
                    dataGrid.Columns.RemoveAt(0);
                }
                StateLabel.Visibility = Visibility.Hidden;
                ShowLoading();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {
                    int idEvent = DataClass.EventID;
                    if (idEvent != 0)
                    {
                        DataTable dataTable = await Query.SelectAttendance(idEvent);
                        if (dataTable != null)
                        {
                            marked.IsEnabled = true;
                            dataGrid.ItemsSource = dataTable.DefaultView;
                            dataGrid.Columns[0].Header = "Название предмета";
                            dataGrid.Columns[1].Header = "Дата посещения";
                            dataGrid.Columns[2].Header = "Статус посещения";
                            dataGrid.Columns[3].Header = "Имя учащегося";

                            for (int i = 0; i < 4; i++)
                            {
                                dataGrid.Columns[i].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                            }                            
                        }

                        studCount.Content = $"Всего отмеченых учащихся: {dataGrid.Items.Count}";
                    }
                    else
                    {
                        StateLabel.Visibility = Visibility.Visible;
                        markedCount = 0;
                        studCount.Content = $"Всего отмеченых учащихся: 0";
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
                message = new CustomMessage("Что-то пошло не так!", "Упс!", false, 3);
                message.ShowDialog();
            }
            finally
            {
                HideLoading();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataTable();
        }

        private void RefreshButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FillDataTable();
        }

        private void ShowLoading()
        {
            MainPage.OptionsBar.IsEnabled = false;
            Loading.Visibility = Visibility.Visible;
            MainGrid.IsEnabled = false;
            RefreshButton.Visibility = Visibility.Hidden;
        }
        private void HideLoading()
        {
            MainPage.OptionsBar.IsEnabled = true;
            Loading.Visibility = Visibility.Hidden;
            RefreshButton.Visibility = Visibility.Visible;
            MainGrid.IsEnabled = true;
        }

        private async void marked_Click(object sender, RoutedEventArgs e)
        {           
            ShowLoading();
            try
            {
                if (DataClass.Group_ID != 0 || DataClass.EventID != 0)
                {
                    if (markedCount == 0)
                    {
                        bool internetCheck = await InternetConnectionChecker.InternetChecking();
                        if (internetCheck)
                        {
                            bool result = await Query.SelectUsersUnmarked(DataClass.Group_ID, DataClass.EventID, DataClass.Event_End);
                            if (result)
                            {
                                FillDataTable();
                                markedCount = 1;
                                message = new CustomMessage("Отсутствующие отмечены", "Выполнено", false, 2);
                                message.ShowDialog();
                            }
                            else
                            {
                                message = new CustomMessage("Все на месте", "Выполнено", false, 2);
                                message.ShowDialog();
                            }
                        }
                        else
                        {
                            message = new CustomMessage("Нет доступа в интернет", "Ошибка", false, 3);
                            message.ShowDialog();
                        }
                    }
                    else
                    {
                        message = new CustomMessage("Отсутствующие уже были отмечены", "Ошибка", false, 3);
                        message.ShowDialog();
                    }                    
                }
                else
                {
                    message = new CustomMessage("Нет события", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                message = new CustomMessage("Что-то пошло не так!", "Упс!", false, 3);
                message.ShowDialog();
            }
            finally
            {                
                HideLoading();
            }
            
        }
    }
}

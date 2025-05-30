using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace QR_Checking_winVersion
{
    /// <summary>
    /// Логика взаимодействия для StatPage.xaml
    /// </summary>
    public partial class StatPage : Page
    {
        private static CustomMessage message;
        private static MainPage MainPage;
        private static HomePage HomePage;
        private static SP_Attendance attendance;
        private static SP_Events events;
        public StatPage(MainPage mainPage, HomePage homePage)
        {
            InitializeComponent();
            MainPage = mainPage;
            HomePage = homePage;
            attendance = new SP_Attendance();
            events = new SP_Events();
        }

        private async void FillMainGrid()
        {
            try
            {
                LoadingStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {

                    if (StatComboBox.SelectedIndex == 0)
                    {
                        MainFrame.Content = attendance;
                        await attendance.FillDataGrid();
                    }
                    else if (StatComboBox.SelectedIndex == 1)
                    {
                        MainFrame.Content = events;
                        await events.FillDataGrid();
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
                LoadingEnd();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StatComboBox.SelectedIndex = 0;
        }

        public void LoadingStart()
        {
            MainPage.OptionsBar.IsEnabled = false;
            Loading.Visibility = Visibility.Visible;
            MainGrid.IsEnabled = false;
            RefreshButton.Visibility = Visibility.Hidden;
        }

        public void LoadingEnd()
        {
            MainPage.OptionsBar.IsEnabled = true;
            Loading.Visibility = Visibility.Hidden;
            RefreshButton.Visibility = Visibility.Visible;
            MainGrid.IsEnabled = true;
        }

        private void RefreshButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FillMainGrid();
        }

        private void Back_Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.MainFrame.Content = HomePage;
        }

        private void StatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillMainGrid();
        }
    }
}

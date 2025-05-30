using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QR_Checking_winVersion
{
    /// <summary>
    /// Логика взаимодействия для AdminPanelPage.xaml
    /// </summary>
    public partial class AdminPanelPage : Page
    {
        private static CustomMessage message;
        private static MainPage MainPage;
        private static HomePage HomePage;
        private static AP_AppConfig appConfig;
        private static AP_Attendance attendance;
        private static AP_Disciplines disciplines;
        private static AP_Events events;
        private static AP_Groups groups;
        private static AP_QrCodes qrCodes;
        private static AP_UsersStud usersStud;
        private static AP_UsersTeach usersTeach;


        public AdminPanelPage(MainPage mainPage, HomePage homePage)
        {
            InitializeComponent();
            MainPage = mainPage;
            HomePage = homePage;
            appConfig = new AP_AppConfig(this);
            attendance = new AP_Attendance(this);
            disciplines = new AP_Disciplines(this);
            events = new AP_Events(this);
            groups = new AP_Groups(this);
            qrCodes = new AP_QrCodes(this);
            usersStud = new AP_UsersStud(this);
            usersTeach = new AP_UsersTeach(this);

        }

        private void RefreshButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FillMainGrid();
        }

        private void Back_Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.MainFrame.Content = HomePage;
        }

        private void AdminPanelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillMainGrid();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AdminPanelComboBox.SelectedIndex = 0;
        }
        
        public void LoadingPageStart()
        {
            MainPage.OptionsBar.IsEnabled = false;
            Loading.Visibility = Visibility.Visible;
            MainGrid.IsEnabled = false;
            RefreshButton.Visibility = Visibility.Hidden;
        }

        public void LoadingPageDone()
        {
            MainPage.OptionsBar.IsEnabled = true;
            Loading.Visibility = Visibility.Hidden;
            RefreshButton.Visibility = Visibility.Visible;
            MainGrid.IsEnabled = true;
        }

        private async void FillMainGrid()
        {
            try
            {
                LoadingPageStart();

                bool internetCheck = await InternetConnectionChecker.InternetChecking();
                if (internetCheck)
                {

                    if (AdminPanelComboBox.SelectedIndex == 0)
                    {
                        AdminPanelFrame.Content = appConfig;
                        await appConfig.FillDataGrid();                                               
                    }
                    else if (AdminPanelComboBox.SelectedIndex == 1)
                    {
                        AdminPanelFrame.Content = attendance;
                        await attendance.FillDataGrid();
                        bool fillUsers = await attendance.FillingUsers();
                        bool fillEvents = await attendance.FillingEvents();
                        if (!fillUsers)
                        {
                            message = new CustomMessage("Не удалось загрузить данные", "Ошибка", false, 3);
                            message.ShowDialog();
                        }
                        if (!fillEvents)
                        {
                            message = new CustomMessage("Не удалось загрузить данные", "Ошибка", false, 3);
                            message.ShowDialog();
                        }
                    }
                    else if (AdminPanelComboBox.SelectedIndex == 2)
                    {
                        AdminPanelFrame.Content = disciplines;
                        await disciplines.FillDataGrid();
                        bool fillGroups = await disciplines.FillingGroups();
                        if (!fillGroups)
                        {
                            message = new CustomMessage("Не удалось загрузить данные", "Ошибка", false, 3);
                            message.ShowDialog();
                        }
                    }
                    else if (AdminPanelComboBox.SelectedIndex == 3)
                    {
                        AdminPanelFrame.Content = events;
                        await events.FillDataGrid();                       
                    }
                    else if (AdminPanelComboBox.SelectedIndex == 4)
                    {
                        AdminPanelFrame.Content = qrCodes;
                        await qrCodes.FillDataGrid();
                    }
                    else if (AdminPanelComboBox.SelectedIndex == 5)
                    {
                        AdminPanelFrame.Content = groups;
                        await groups.FillDataGrid();
                        bool fillCurators = await groups.FillingCurators();
                        if (!fillCurators)
                        {
                            message = new CustomMessage("Не удалось загрузить данные", "Ошибка", false, 3);
                            message.ShowDialog();
                        }
                    }
                    else if (AdminPanelComboBox.SelectedIndex == 6)
                    {
                        AdminPanelFrame.Content = usersStud;
                        await usersStud.FillDataGrid();
                        bool fillGroups = await usersStud.FillingGroups();
                        if (!fillGroups)
                        {
                            message = new CustomMessage("Не удалось загрузить данные", "Ошибка", false, 3);
                            message.ShowDialog();
                        }
                    }
                    else if (AdminPanelComboBox.SelectedIndex == 7)
                    {
                        AdminPanelFrame.Content = usersTeach;
                        await usersTeach.FillDataGrid();
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
                LoadingPageDone();
            }
        }
    }
}

using System.Windows;
using System.Windows.Input;

namespace QR_Checking_winVersion
{
    public partial class MainPage : Window
    {
        private static DataClass DataClass;
        private static ProfilePage profilePage = null;
        private static QR_GEN_Page qR_GEN_Page = null;
        private static HomePage homePage = null;
        private static AttendanceStud attendanceStud = null;

        private static Query Query;

        public MainPage(DataClass dataClass, Query query)
        {
            InitializeComponent();
            DataClass = dataClass;
            Query = query;
            homePage = new HomePage(this, DataClass, Query);            
            MainFrame.Content = homePage;            
            profilePage = new ProfilePage(this, DataClass, Query);
            qR_GEN_Page = new QR_GEN_Page(DataClass, this, Query);
            attendanceStud = new AttendanceStud(DataClass, Query, this);
        }

        private void Close_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CustomMessageBox customMessage = new CustomMessageBox(this, DataClass, Query);
            customMessage.ShowDialog();
        }

        private void Hide_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void MyProfile_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Content = profilePage;
        }

        private void Home_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Content = homePage;
        }

        private void QR_Generate_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Content = qR_GEN_Page;
        }

        private void Event_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
           MainFrame.Content = attendanceStud;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
    }
}

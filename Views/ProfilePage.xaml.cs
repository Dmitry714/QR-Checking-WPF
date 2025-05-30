using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QR_Checking_winVersion
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    /// 
    
    public partial class ProfilePage : Page
    {
        private static MainPage MainPage;
        private static DataClass DataClass;
        private static Query Query;
        private static PP_MainInfo pp_info = null;
        private static PP_Authinfo pp_authInfo = null;
        public ProfilePage(MainPage mainPage, DataClass dataClass, Query query)
        {
            InitializeComponent();
            MainPage = mainPage;
            DataClass = dataClass;
            Query = query;
            pp_info = new PP_MainInfo(DataClass, MainPage, Query, this);
            pp_authInfo = new PP_Authinfo(DataClass, MainPage, Query, this);
        }

        private void mData_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.MainFrame.Content = pp_info;
        }

        private void aData_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.MainFrame.Content = pp_authInfo;
        }

        private async void dAccount_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CustomMessage deleteAccount = new CustomMessage("После удаления аккаунта нужно будет заново войти. Удалить аккаунт ?", "Удаление аккаунта", true, 1);
            bool? resultmessage = deleteAccount.ShowDialog();
            if (resultmessage == true)
            {
                MainGrid.IsEnabled = false;
                Loading.Visibility = Visibility.Visible;
                bool result = await Query.DisableUser(DataClass.ID_User);
                if (result)
                {
                    MainGrid.IsEnabled = true;
                    Loading.Visibility = Visibility.Hidden;
                    await QueryConnection.CloseConnection();
                    MainWindow authPage = new MainWindow();
                    authPage.Show();                    
                    MainPage.Close();
                }
                else
                {
                    MainGrid.IsEnabled = true;
                    Loading.Visibility = Visibility.Hidden;
                    CustomMessage error = new CustomMessage("Не удалось удалить аккаунт. Ошибка подключения", "Ошибка", false, 3);
                    error.ShowDialog();
                }
            }            
        }
    }
}

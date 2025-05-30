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
using System.Windows.Shapes;

namespace QR_Checking_winVersion
{
    /// <summary>
    /// Логика взаимодействия для CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        private static MainPage MainPage;
        private static DataClass DataClass;
        private static Query Query;

        public CustomMessageBox(MainPage mainPage, DataClass dataClass, Query query)
        {
            InitializeComponent();
            MainPage = mainPage;
            DataClass = dataClass;
            Query = query;
        }

        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void Close_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private async void Close_App_Click(object sender, RoutedEventArgs e)
        {
            MesIcon.Visibility = Visibility.Hidden;
            Loading.Visibility = Visibility.Visible;
            this.IsEnabled = false;
            try
            {                
                await Query.UpdateEventState(DataClass.ID_User);
                await Query.QrCodeDelete(DataClass.ID_User);
                await QueryConnection.CloseConnection();
                Application.Current.Shutdown();
            }
            catch (Exception) { }
            finally
            {
                MesIcon.Visibility = Visibility.Visible;
                Loading.Visibility = Visibility.Hidden;
                this.IsEnabled = true;
            }                        
        }

        private async void Change_acc_Click(object sender, RoutedEventArgs e)
        {
            MesIcon.Visibility = Visibility.Hidden;
            Loading.Visibility = Visibility.Visible;
            this.IsEnabled = false;
            try
            {
                await Query.UpdateEventState(DataClass.ID_User);
                await Query.QrCodeDelete(DataClass.ID_User);
                await QueryConnection.CloseConnection();
                MainWindow authPage = new MainWindow();
                authPage.Show();
                this.Close();
                MainPage.Close();
            }
            catch (Exception) { }
            finally
            {
                MesIcon.Visibility = Visibility.Visible;
                Loading.Visibility = Visibility.Hidden;
                this.IsEnabled = true;
            }
            
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

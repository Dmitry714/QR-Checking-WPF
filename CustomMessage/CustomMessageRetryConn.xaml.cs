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
    /// Логика взаимодействия для CustomMessageRetryConn.xaml
    /// </summary>
    public partial class CustomMessageRetryConn : Window
    {

        public CustomMessageRetryConn()
        {
            InitializeComponent();
        }

        private void RetryConnButton_Click(object sender, RoutedEventArgs e)
        {
            RetryConnect();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private async void RetryConnect()
        {
            Info.Content = "Подключение";
            MainMessage.Text = "Повторное подключение к серверу...";
            Loading.Visibility = Visibility.Visible;
            MesIcon.Visibility = Visibility.Hidden;            
            RetryButton.Visibility = Visibility.Hidden;
            CancelButton.Visibility = Visibility.Hidden;

            bool tryConnect = await QueryConnection.OpenConnection();
            if (tryConnect)
            {
                this.Close();
            }
            else
            {
                Info.Content = "Ошибка";
                MainMessage.Text = "Не удалось подключиться к серверу!";
                Loading.Visibility = Visibility.Hidden;
                MesIcon.Visibility = Visibility.Visible;
                RetryButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Visible;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RetryConnect();
        }
    }
}

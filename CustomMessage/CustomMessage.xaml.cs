using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace QR_Checking_winVersion
{
    public partial class CustomMessage : Window
    {
        public string MessageText { get; set; }
        public string TitleText { get; set; }
        public bool ShowCancelButton { get; set; }
        public int MessageIcon { get; set; }


        public CustomMessage(string messageText, string titleText, bool showCancelButton, int messageIcon)
        {
            InitializeComponent();
            DataContext = true;
            MessageText = messageText;
            TitleText = titleText;
            MessageIcon = messageIcon;
            ShowCancelButton = showCancelButton;
            Info.Content = TitleText;
            MainMessage.Text = MessageText;
            Title = TitleText;
            SelectIcon();
            SelectOkCancel();                        
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void SelectOkCancel()
        {
            if (!ShowCancelButton)
            {
                CancelButton.Visibility = Visibility.Collapsed;
            }
        }

        private void SelectIcon()
        {
            switch (MessageIcon)
            {
                //Вопрос
                case 1:
                    MesIcon.Source = new BitmapImage(new Uri("/QR_Checking_winVersion;component/Resources/question.png", UriKind.RelativeOrAbsolute));
                    break;
                //Восклицательный знак
                case 2:
                    MesIcon.Source = new BitmapImage(new Uri("/QR_Checking_winVersion;component/Resources/information.png", UriKind.RelativeOrAbsolute));
                    break;
                case 3:
                //Ошибка
                    MesIcon.Source = new BitmapImage(new Uri("/QR_Checking_winVersion;component/Resources/error.png", UriKind.RelativeOrAbsolute));
                    break;
            }
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
    }
}

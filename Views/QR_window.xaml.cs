using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ZXing;

namespace QR_Checking_winVersion
{
    /// <summary>
    /// Логика взаимодействия для QR_window.xaml
    /// </summary>
    public partial class QR_window : Window
    {
        private Timer timer;
        private int timerSeconds;

        private static DataClass DataClass;
        private static QR_GEN_Page QR_GEN;
        private static MainPage MainPage;
        private static CustomMessage message;
        private static Query Query;        
        public QR_window(DataClass dataClass, QR_GEN_Page qr_Gen, MainPage mainPage, Query query)
        {
            InitializeComponent();
            DataClass = dataClass;
            QR_GEN = qr_Gen;
            MainPage = mainPage;
            Query = query;
            StartTimer();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerSeconds--;
            Dispatcher.Invoke(() =>
            {
                timerTextBlock.Text = TimeSpan.FromSeconds(timerSeconds).ToString("mm':'ss");
                if (timerSeconds == 0)
                {
                    RestartTimer();                    
                }
            });
        }

        private async void RestartTimer()
        {
            timer.Stop();            
            timerSeconds = GetNumbersFromComboBox();
            timerTextBlock.Text = TimeSpan.FromSeconds(timerSeconds).ToString("mm':'ss");
            bool updateQr = await UpdateQrCode();
            if (updateQr)
            {
                timer.Start();
            }
            else
            {
                RetryUpdateQrCode.Visibility = Visibility.Visible;
            }
        }

        public BitmapImage GenerateQRCode(string text)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE                
            };
            writer.Options.Height = 600;
            writer.Options.Width = 600;
            var result = writer.Write(text);

            Bitmap qrCodeImage = new Bitmap(result);
            BitmapImage qrCodeBitmap = new BitmapImage();

            using (var memoryStream = new MemoryStream())
            {
                qrCodeImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Position = 0;

                qrCodeBitmap.BeginInit();
                qrCodeBitmap.CacheOption = BitmapCacheOption.OnLoad;
                qrCodeBitmap.StreamSource = memoryStream;
                qrCodeBitmap.EndInit();
            }
            return qrCodeBitmap;
        }

        public static string GetQrText(int length)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                int index = rnd.Next(chars.Length);
                sb.Append(chars[index]);
            }
            return sb.ToString();
        }

        public async Task<bool> CreateNewQrCode()
        {
            logo.Visibility = Visibility.Hidden;
            Loading.Visibility = Visibility.Visible;
            string qr_text = GetQrText(20);

            bool result = await Query.QrCodeGenerate(DataClass.EventID, DataClass.ID_User, qr_text);
            if (result)
            {
                logo.Visibility = Visibility.Visible;
                Loading.Visibility = Visibility.Hidden;
                BitmapImage qrCodeImage = GenerateQRCode(qr_text);
                qrpicture.Source = qrCodeImage;
                return true;
            }
            else
            {
                logo.Visibility = Visibility.Visible;
                Loading.Visibility = Visibility.Hidden;
                message = new CustomMessage("Не удалось создать QR код","Ошибка",false,3);
                message.ShowDialog();             
                return false;
            }            
        }

        public async Task<bool> UpdateQrCode()
        {
            logo.Visibility = Visibility.Hidden;
            Loading.Visibility = Visibility.Visible;
            qrpicture.Effect = new BlurEffect() { Radius = 15 };
            string qr_text = GetQrText(20);
            bool result = await Query.QrCodeUpdate(DataClass.ID_User, qr_text);
            if (result)
            {
                logo.Visibility = Visibility.Visible;
                Loading.Visibility = Visibility.Hidden;
                BitmapImage qrCodeImage = GenerateQRCode(qr_text);
                qrpicture.Source = qrCodeImage;
                qrpicture.Effect = null;
                return true;                
            }
            else
            {
                logo.Visibility = Visibility.Visible;
                Loading.Visibility = Visibility.Hidden;
                message = new CustomMessage("Не удалось обновить QR код", "Ошибка", false, 3);
                message.ShowDialog();
                timer.Stop();
                qrpicture.Effect = null;
                return false;                
            }                        
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Stop();

            bool unmarkedUsers = await Query.SelectUsersUnmarked(DataClass.Group_ID, DataClass.EventID, DataClass.Event_End);
            if (unmarkedUsers)
            {
                message = new CustomMessage("Отсутствующие отмечены", "Выполнено", false, 2);
                message.ShowDialog();
            }

            DataClass.EventID = 0;
            DataClass.Group_ID = 0;
            

            bool eventUpdateState = await Query.UpdateEventState(DataClass.ID_User);
            if (eventUpdateState)
            {
                message = new CustomMessage("Событие деактивированно", "Выполнено", false, 2);
                message.ShowDialog();
            }
            else
            {
                message = new CustomMessage("Не удалось деактивировать событие", "Ошибка", false, 3);
                message.ShowDialog();
            }            

            bool qrdelete = await Query.QrCodeDelete(DataClass.ID_User);
            if (qrdelete)
            {
                MainPage.QR_Generate_button.Style = (Style)FindResource("QR_Generate_button_change");
                message = new CustomMessage("QR код был удален", "Выполнено", false, 2);
                message.ShowDialog();
                MainPage.QR_Generate_button.IsEnabled = true;                
            }
            else
            {
                MainPage.QR_Generate_button.Style = (Style)FindResource("QR_Generate_button_change");
                message = new CustomMessage("Не удалось удалить QR код", "Ошибка", false, 3);
                message.ShowDialog();
                MainPage.QR_Generate_button.IsEnabled = true;                
            }
        }

        private void Close_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();            
        }

        private void Hide_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Expand_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }            
        }

        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private int GetNumbersFromComboBox()
        {
            string text = QR_GEN.UpdateTime.Text;

            Regex regex = new Regex(@"\d+");

            MatchCollection matches = regex.Matches(text);
            int numValue = 0;
            foreach (Match match in matches)
            {
                int.TryParse(match.Value, out int Result);
                numValue = Result;                
            }
            return numValue;
        }

        private async void StartTimer()
        {
            timerSeconds = GetNumbersFromComboBox();
            timerTextBlock.Text = TimeSpan.FromSeconds(timerSeconds).ToString("mm':'ss");
            timer = new Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            bool createQr = await CreateNewQrCode();
            if (createQr)
            {
                timer.Start();
            }
            else
            {
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Match _match = Regex.Match(QR_GEN.DisciplineComboBox.Text, @"-\s*(.*)");
            string disciplineName = string.Empty;
            if (_match.Success)
            {
                disciplineName = _match.Groups[1].Value;
            }

            Match _match2 = Regex.Match(QR_GEN.Groups.Text, @"-\s*(.*)");
            string groupName = string.Empty;
            if (_match2.Success)
            {
                groupName = _match2.Groups[1].Value;
            }

            if (QR_GEN.Groups.Text == "Нет")
            {                
                EventName.Text = QR_GEN.EventTextBox.Text;
                EventLabel.Content = "Событие:";
                GroupNumber.Text = "Для всех";
            }
            else
            {
                EventLabel.Content = "Предмет:";
                EventName.Text = disciplineName;
                GroupNumber.Text = groupName;
            }            
        }

        private void RetryUpdateQrCode_Click(object sender, RoutedEventArgs e)
        {
            RestartTimer();
            RetryUpdateQrCode.Visibility = Visibility.Hidden;
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OfficeOpenXml;
using Microsoft.Win32;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace QR_Checking_winVersion
{
    public partial class HomePage : Page
    {
        private static MainPage MainPage;
        private static StatPage StatPage = null;
        private static DataClass DataClass;
        private static CustomMessage message;
        private static AdminPanelPage AdminPanelPage = null;
        private static Query Query;
        public HomePage(MainPage mainpage, DataClass dataClass, Query query)
        {
            InitializeComponent();
            MainPage = mainpage;
            DataClass = dataClass;
            Query = query;
            StatPage = new StatPage(MainPage, this);
            AdminPanelPage = new AdminPanelPage(MainPage, this);
        }

        private void stat_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.MainFrame.Content = StatPage;
        }

        private void admin_panel_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataClass.Role == "Admin")
            {
                MainPage.MainFrame.Content = AdminPanelPage;
            }
            else
            {
                message = new CustomMessage("У вас нет прав администратора", "Ошибка", false, 3);
                message.ShowDialog();
            }
        }

        private async void report_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            bool checkInternet = await InternetConnectionChecker.InternetChecking();
            if (checkInternet)
            {
                ExportToExcel();
            }
            else
            {
                message = new CustomMessage("Нет доступа в интернет", "Ошибка", false, 3);
                message.ShowDialog();
            }            
        }

        private async void ExportToExcel()
        {
            miniLoading.Visibility = Visibility.Visible;
            MainGrid.IsEnabled = false;
            MainPage.OptionsBar.IsEnabled = false;

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                DateTime time = DateTime.Now;
                string timeToday = time.ToString("dd.MM.yyyy");
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveFileDialog.FileName = $"Отчет посещений {timeToday}";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (ExcelPackage package = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet1 = package.Workbook.Worksheets.Add("Посещения");
                        ExcelWorksheet worksheet2 = package.Workbook.Worksheets.Add("События");       

                        DataTable dataTableAttendance = await Query.SelectAttendanceFullData();
                        
                        dataTableAttendance.Columns[0].ColumnName = "ID посещения";
                        dataTableAttendance.Columns[1].ColumnName = "Имя учащегося";
                        dataTableAttendance.Columns[2].ColumnName = "Номер группы";
                        dataTableAttendance.Columns[3].ColumnName = "Название события";
                        dataTableAttendance.Columns[4].ColumnName = "Дата посещения";
                        dataTableAttendance.Columns[5].ColumnName = "Статус посещения";

                        if (dataTableAttendance != null)
                        {
                            for (int i = 0; i < dataTableAttendance.Columns.Count; i++)
                            {
                                worksheet1.Cells[1, i + 1].Value = dataTableAttendance.Columns[i].ColumnName;

                                ExcelRange headerCell = worksheet1.Cells[1, i + 1];
                                headerCell.Style.Font.Bold = true;
                                headerCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                            }

                            for (int row = 0; row < dataTableAttendance.Rows.Count; row++)
                            {
                                for (int col = 0; col < dataTableAttendance.Columns.Count; col++)
                                {
                                    worksheet1.Cells[row + 2, col + 1].Value = dataTableAttendance.Rows[row][col];
                                }
                            }
                        }
                        
                        DataTable dataTableEvents = await Query.SelectFromEventsFullData();

                        dataTableEvents.Columns[0].ColumnName = "ID события";
                        dataTableEvents.Columns[1].ColumnName = "Имя преподавателя";
                        dataTableEvents.Columns[2].ColumnName = "Номер группы";
                        dataTableEvents.Columns[3].ColumnName = "Название события/предмета";
                        dataTableEvents.Columns[4].ColumnName = "Место события";
                        dataTableEvents.Columns[5].ColumnName = "Дата начала события";
                        dataTableEvents.Columns[6].ColumnName = "Дата конца события";
                        dataTableEvents.Columns[7].ColumnName = "Активно";

                        if (dataTableEvents != null)
                        {
                            for (int i = 0; i < dataTableEvents.Columns.Count; i++)
                            {
                                worksheet2.Cells[1, i + 1].Value = dataTableEvents.Columns[i].ColumnName;
                            }

                            for (int row = 0; row < dataTableEvents.Rows.Count; row++)
                            {
                                for (int col = 0; col < dataTableEvents.Columns.Count; col++)
                                {
                                    worksheet2.Cells[row + 2, col + 1].Value = dataTableEvents.Rows[row][col];
                                }
                            }
                        }                        

                        worksheet1.Cells.AutoFitColumns();
                        worksheet2.Cells.AutoFitColumns();
                        package.SaveAs(new FileInfo(filePath));

                        message = new CustomMessage("Отчет был успешно экcпортирован", "Выполнено", false, 2);
                        message.ShowDialog();
                    }
                }
            }
            catch (Exception)
            {
                message = new CustomMessage("Не удалось экспортировать отчет в Excel", "Ошибка", false, 3);
                message.ShowDialog();
            }
            finally
            {
                miniLoading.Visibility = Visibility.Hidden;
                MainGrid.IsEnabled = true;
                MainPage.OptionsBar.IsEnabled = true;                
            }
        }

        private void checkIpLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string url = "https://2ip.ru";

            try
            {
                Process.Start(new ProcessStartInfo(url));
            }
            catch (Exception)
            {
                message = new CustomMessage("Не удалось открыть ссылку", "Ошибка", false, 3);
                message.ShowDialog();
            }
        }

        private void dbAdminPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string url = "https://www.db4free.net/about.php";

            try
            {
                Process.Start(new ProcessStartInfo(url));
            }
            catch (Exception)
            {
                message = new CustomMessage("Не удалось открыть ссылку", "Ошибка", false, 3);
                message.ShowDialog();
            }
        }

        private void vk_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string url = "https://vk.com/public40400418";

            try
            {
                Process.Start(new ProcessStartInfo(url));
            }
            catch (Exception)
            {
                message = new CustomMessage("Не удалось открыть ссылку", "Ошибка", false, 3);
                message.ShowDialog();
            }
        }

        private void tg_button_MouseDown(object sender, MouseButtonEventArgs e)
        {            
            string url = "https://t.me/ngaek";

            try
            {
                Process.Start(new ProcessStartInfo(url));
            }
            catch (Exception)
            {
                message = new CustomMessage("Не удалось открыть ссылку", "Ошибка", false, 3);
                message.ShowDialog();
            }
        }

        private void insta_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string url = "https://www.instagram.com/uo_ngaek/";

            try
            {
                Process.Start(new ProcessStartInfo(url));
            }
            catch (Exception)
            {
                message = new CustomMessage("Не удалось открыть ссылку", "Ошибка", false, 3);
                message.ShowDialog();
            }
        }

        private void info_button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var exePath = AppDomain.CurrentDomain.BaseDirectory;
                var path = Path.Combine(exePath, "..\\..\\Resources\\readme.txt");

                Process.Start(new ProcessStartInfo()
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
            catch (Exception)
            {
                message = new CustomMessage("Не удалось открыть файл", "Ошибка", false, 3);
                message.ShowDialog();
            }
        }

    }
}

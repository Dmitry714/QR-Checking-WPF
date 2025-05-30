using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QR_Checking_winVersion
{
    /// <summary>
    /// Логика взаимодействия для QR_GEN_Page.xaml
    /// </summary>
    public partial class QR_GEN_Page : Page
    {
        private static DataClass DataClass;
        private static MainPage MainPage;
        private static AttendanceStud AttendanceStud = null;
        private static CustomMessage message;
        private static Query Query;
        private bool GroupSelectionIsNo = false;
        public QR_GEN_Page(DataClass dataClass, MainPage mainPage, Query query)
        {
            InitializeComponent();
            FillingHours();
            FillingMinutes();            
            DataClass = dataClass;
            MainPage = mainPage;
            AttendanceStud = new AttendanceStud(DataClass, Query, MainPage);
            Query = query;
            HoursComboBoxBegin.SelectedIndex = 3;
            MinutesComboBoxBegin.SelectedIndex = 0;
            HoursComboBoxEnd.SelectedIndex = 4;
            MinutesComboBoxEnd.SelectedIndex = 7;
            DateBegin.SelectedDate = DateTime.Now;
            DateEnd.SelectedDate = DateTime.Now;
        }

        private string BeginDateTimeCreator()
        {
            string timeBegin = $"{HoursComboBoxBegin.Text}:{MinutesComboBoxBegin.Text}:00";
            DateTime? dateTimeBeginSelected = DateBegin.SelectedDate;
            if (dateTimeBeginSelected.HasValue && HoursComboBoxBegin.Text != "" && MinutesComboBoxBegin.Text != "")
            {
                DateTime dateBegin = dateTimeBeginSelected.Value;
                string dateTimeBegin = dateBegin.ToString("yyyy-MM-dd") + " " + timeBegin;
                return dateTimeBegin;
            }
            else
            {
                return null;
            }
        }

        private string EndDateTimeCreator()
        {
            string timeEnd = $"{HoursComboBoxEnd.Text}:{MinutesComboBoxEnd.Text}:00";
            DateTime? dateTimeEndSelected = DateEnd.SelectedDate;
            if (dateTimeEndSelected.HasValue && HoursComboBoxEnd.Text != "" && MinutesComboBoxEnd.Text != "")
            {
                DateTime dateEnd = dateTimeEndSelected.Value;
                string dateTimeEnd = dateEnd.ToString("yyyy-MM-dd") + " " + timeEnd;
                return dateTimeEnd;
            }
            else
            {
                return null;
            }
        }

        private bool DateChecking()
        {
            string beginDateTimeString = BeginDateTimeCreator();
            string endDateTimeString = EndDateTimeCreator();
            if (beginDateTimeString != null && endDateTimeString != null)
            {
                DateTime beginDateTime = DateTime.ParseExact(beginDateTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime endDateTime = DateTime.ParseExact(endDateTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                if (beginDateTime < endDateTime)
                {
                    return true;
                }
                else if (beginDateTime > endDateTime || beginDateTime == endDateTime)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        private async void create_qr_code_Click(object sender, RoutedEventArgs e)
        {
            MainPage.OptionsBar.IsEnabled = false;
            miniLoading.Visibility = Visibility.Visible;
            RefreshButton.Visibility = Visibility.Hidden;
            GridControls.IsEnabled = false;

            try
            {
                bool isConnected = await InternetConnectionChecker.InternetChecking();
                if (isConnected)
                {
                    if (!string.IsNullOrWhiteSpace(DataClass.Name) || !string.IsNullOrWhiteSpace(DataClass.Surname) || !string.IsNullOrWhiteSpace(DataClass.Patronymic) || !string.IsNullOrWhiteSpace(DataClass.Specialization) || !string.IsNullOrWhiteSpace(DataClass.Specialization))
                    {
                        bool result = await CreateEvent();

                        if (result)
                        {
                            int selectedID = 0;
                            List<int> eventIds = await Query.SelectActiveEventsForGenerateCode(DataClass.ID_User);
                            if (eventIds != null)
                            {
                                foreach (int eventId in eventIds)
                                {
                                    selectedID = eventId;
                                }

                                DataClass.EventID = selectedID;

                                MainPage.QR_Generate_button.IsEnabled = false;
                                MainPage.QR_Generate_button.Style = (Style)FindResource("QR_Generate_button_Done_State");
                                MainPage.MainFrame.Content = AttendanceStud;
                                QR_window QR_Window = new QR_window(DataClass, this, MainPage, Query);
                                QR_Window.Show();
                            }
                            else
                            {
                                message = new CustomMessage("У вас больше 1 активного события. Возможно это возникло в результате сбоя БД. Обратитесь к администратору", "Ошибка", false, 3);
                                message.ShowDialog();
                            }
                        }
                    }
                    else
                    {
                        message = new CustomMessage("Заполнте профиль, прежде чем создать QR-код!", "Ошибка", false, 3);
                        message.ShowDialog();
                    }
                }
                else
                {
                    GridControls.Visibility = Visibility.Hidden;
                    ConnError.Visibility = Visibility.Visible;
                }
            }
            catch (Exception)
            {
                message = new CustomMessage("Что-то пошло не так!", "Упс!", false, 3);
                message.ShowDialog();
            }
            finally
            {
                miniLoading.Visibility = Visibility.Hidden;
                RefreshButton.Visibility = Visibility.Visible;
                GridControls.IsEnabled = true;
            }            
        }

        private async Task<bool> FillingGroups()
        {
            Groups.Items.Clear();
            Groups.SelectedIndex = 0;
            Groups.Items.Add("Нет");
            List<string> groups = await Query.SelectFromGroups();
            if (groups != null)
            {
                foreach (string group in groups)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = group
                    };
                    Groups.Items.Add(item);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void FillingHours()
        {
            for (int i = 6; i < 10; i++)
            {
                HoursComboBoxBegin.Items.Add("0" + i);
                HoursComboBoxEnd.Items.Add("0" + i);
            }

            for (int i = 10; i < 24; i++)
            {
                HoursComboBoxBegin.Items.Add(i);
                HoursComboBoxEnd.Items.Add(i);
            }
        }

        private void FillingMinutes()
        {
            for (int i = 0; i < 6; i += 5)
            {
                MinutesComboBoxBegin.Items.Add("0" + i);
                MinutesComboBoxEnd.Items.Add("0" + i);
            }

            for (int i = 10; i < 60; i += 5)
            {
                MinutesComboBoxBegin.Items.Add(i);
                MinutesComboBoxEnd.Items.Add(i);
            }
        }

        private async Task<bool> FillingDisciplines(string groupID)
        {
            Regex regex = new Regex(@"^(\d+)");

            MatchCollection matches = regex.Matches(groupID);
            int numValue = 0;
            foreach (Match match in matches)
            {
                int.TryParse(match.Value, out int Result);
                numValue = Result;
            }

            List<string> disciplines = await Query.SelectFromDisciplines(numValue);
            if (disciplines != null)
            {
                foreach (string discipline in disciplines)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = discipline
                    };
                    DisciplineComboBox.Items.Add(item);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task PageLoading()
        {
            MainPage.OptionsBar.IsEnabled = false;
            try
            {
                Loading.Visibility = Visibility.Visible;
                miniLoading.Visibility = Visibility.Hidden;
                GridControls.Visibility = Visibility.Hidden;
                RefreshButton.Visibility = Visibility.Hidden;
                GridControls.Visibility = Visibility.Hidden;
                ConnError.Visibility = Visibility.Hidden;

                bool isConnected = await InternetConnectionChecker.InternetChecking();
                if (isConnected)
                {
                    bool fillgroups = await FillingGroups();
                    if (fillgroups)
                    {
                        Loading.Visibility = Visibility.Hidden;
                        GridControls.Visibility = Visibility.Visible;
                        RefreshButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        message = new CustomMessage("Не удалось загрузить данные", "Ошибка", false, 3);
                        message.ShowDialog();

                        Groups.Items.Clear();
                        Groups.SelectedIndex = 0;
                        Groups.Items.Add("<Нет данных>");
                        Loading.Visibility = Visibility.Hidden;
                        GridControls.Visibility = Visibility.Visible;
                        RefreshButton.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    miniLoading.Visibility = Visibility.Hidden;
                    Loading.Visibility = Visibility.Hidden;
                    GridControls.Visibility = Visibility.Hidden;
                    ConnError.Visibility = Visibility.Visible;
                    RefreshButton.Visibility = Visibility.Visible;
                }
            }
            catch (Exception) 
            {
                message = new CustomMessage("Что-то пошло не так!", "Упс!", false, 3);
                message.ShowDialog();
            }
            finally
            {
                MainPage.OptionsBar.IsEnabled = true;
            }
            
        }

        private async void Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisciplineComboBox.Items.Clear();
            DisciplineComboBox.SelectedIndex = 0;

            if (Groups.SelectedIndex == 0)
            {
                GroupSelectionIsNo = true;
                DisciplineLabel.Visibility = Visibility.Collapsed;
                DisciplineComboBox.Visibility = Visibility.Collapsed;
                EventLabel.Visibility = Visibility.Visible;
                EventTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                GroupSelectionIsNo = false;
                EventLabel.Visibility = Visibility.Collapsed;
                EventTextBox.Visibility = Visibility.Collapsed;
                DisciplineLabel.Visibility = Visibility.Visible;
                DisciplineComboBox.Visibility = Visibility.Visible;
                if (Groups.SelectedItem is ListBoxItem selectedItem)
                {
                    DisciplineComboBox.IsEnabled = false;
                    create_qr_code.IsEnabled = false;
                    miniLoading.Visibility = Visibility.Visible;
                    string selectedText = selectedItem.Content.ToString();


                    bool isConnected = await InternetConnectionChecker.InternetChecking();
                    if (isConnected)
                    {
                        bool fillevents = await FillingDisciplines(selectedText);
                        if (fillevents)
                        {
                            DisciplineComboBox.IsEnabled = true;
                            create_qr_code.IsEnabled = true;
                            miniLoading.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            DisciplineComboBox.IsEnabled = true;
                            create_qr_code.IsEnabled = true;
                            miniLoading.Visibility = Visibility.Hidden;

                            message = new CustomMessage("Не удалось загрузить данные", "Ошибка", false, 3);
                            message.ShowDialog();

                            DisciplineComboBox.Items.Clear();
                            DisciplineComboBox.SelectedIndex = 0;
                            DisciplineComboBox.Items.Add("<Нет данных>");
                        }
                    }
                    else
                    {
                        DisciplineComboBox.IsEnabled = true;
                        create_qr_code.IsEnabled = true;
                        miniLoading.Visibility = Visibility.Hidden;
                        GridControls.Visibility = Visibility.Hidden;
                        ConnError.Visibility = Visibility.Visible;
                    }
                }
            }
        }
        private async Task<bool> CreateEvent()
        {
            bool correctDate = DateChecking();

            if (GroupSelectionIsNo)
            {
                if (Groups.SelectedItem == null || string.IsNullOrWhiteSpace(LocationTextBox.Text) || string.IsNullOrWhiteSpace(EventTextBox.Text) || string.IsNullOrWhiteSpace(DateBegin.Text) || string.IsNullOrWhiteSpace(DateEnd.Text) || Groups.Text == "<Нет данных>" || correctDate == false)
                {
                    message = new CustomMessage("Проверьте корректность введенных данных", "Ошибка", false, 3);
                    message.ShowDialog();
                    return false;
                }
                else
                {
                    List<int> eventIds1 = await Query.SelectActiveEvents(DataClass.ID_User);
                    if (eventIds1 != null)
                    {
                        bool result = await Query.EventCreate(DataClass.ID_User, null, EventTextBox.Text, LocationTextBox.Text, BeginDateTimeCreator(), EndDateTimeCreator(), "Да");
                        if (result)
                        {
                            message = new CustomMessage("Событие было добавлено", "Событие добавлено", false, 2);
                            message.ShowDialog();
                            return true;
                        }
                        else
                        {
                            message = new CustomMessage("Не удалось добавить событие", "Ошибка", false, 3);
                            message.ShowDialog();
                            return false;
                        }
                    }
                    else
                    {
                        message = new CustomMessage("У вас уже есть 1 активное событие. Возможно это возникло в результате сбоя БД. Обратитесь к администратору", "Ошибка", false, 3);
                        message.ShowDialog();
                        return false;
                    }
                }
            }
            else
            {
                if (Groups.SelectedItem == null || DisciplineComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(LocationTextBox.Text) || string.IsNullOrWhiteSpace(DateBegin.Text) || string.IsNullOrWhiteSpace(DateEnd.Text) || Groups.Text == "<Нет данных>" || DisciplineComboBox.Text == "<Нет данных>" || correctDate == false)
                {
                    message = new CustomMessage("Проверьте корректность введенных данных", "Ошибка", false, 3);
                    message.ShowDialog();
                    return false;
                }
                else
                {
                    List<int> eventIds2 = await Query.SelectActiveEvents(DataClass.ID_User);
                    if (eventIds2 != null)
                    {
                        Regex regex = new Regex(@"^(\d+)");
                        MatchCollection matches = regex.Matches(Groups.Text);
                        int GroupId = 0;
                        foreach (Match match in matches)
                        {
                            int.TryParse(match.Value, out int Result);
                            GroupId = Result;
                        }

                        Match _match = Regex.Match(DisciplineComboBox.Text, @"-\s*(.*)");
                        string disciplineName = string.Empty;
                        if (_match.Success)
                        {
                            disciplineName = _match.Groups[1].Value;

                            bool result = await Query.EventCreate(DataClass.ID_User, GroupId, disciplineName, LocationTextBox.Text, BeginDateTimeCreator(), EndDateTimeCreator(), "Да");
                            if (result)
                            {
                                message = new CustomMessage("Событие было добавлено", "Событие добавлено", false, 2);
                                message.ShowDialog();
                                DataClass.Event_End = EndDateTimeCreator();
                                DataClass.Group_ID = GroupId;
                                return true;
                            }
                            else
                            {
                                message = new CustomMessage("Не удалось добавить событие", "Ошибка", false, 3);
                                message.ShowDialog();
                                return false;
                            }
                        }
                        else
                        {
                            message = new CustomMessage("Не удалось добавить событие", "Ошибка", false, 3);
                            message.ShowDialog();
                            return false;
                        }
                    }
                    else
                    {
                        message = new CustomMessage("У вас уже есть 1 активное событие. Возможно это возникло в результате сбоя БД. Обратитесь к администратору", "Ошибка", false, 3);
                        message.ShowDialog();
                        return false;
                    }
                }
            }
        }

        private async void RefreshButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await PageLoading();
        }

        private void EventTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Za-яА-Я0-9 ]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void LocationTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Za-яА-Я0-9,. ]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {            
            await PageLoading();
        }
    }
}
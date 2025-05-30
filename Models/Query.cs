using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace QR_Checking_winVersion
{
    public class Query
    {
        private static CustomMessage message;
        public DataClass _dataClass;

        public Query()
        {
            _dataClass = new DataClass();
        }

        public async Task<bool> UpdateEventState(int ID_User_Teach)
        {
            try
            {
                string query = "Update Events Set IsActive = 'Нет' where ID_User_Teach = @ID_User_Teach";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@ID_User_Teach", ID_User_Teach);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> UpdateEventState_IdEvent(int ID_Event)
        {
            try
            {
                string query = "Update Events Set IsActive = 'Нет' where ID_Event = @ID_Event";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@ID_Event", ID_Event);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }


        public async Task<List<int>> SelectActiveEventsForGenerateCode(int ID_User_Teach)
        {
            try
            {
                string query = $"select ID_Event from Events where ID_User_Teach = @ID_User_Teach and IsActive = 'Да'";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@ID_User_Teach", ID_User_Teach);
                MySqlDataReader reader = await command.ExecuteReaderAsync();
                List<int> eventIDs = new List<int>();

                while (await reader.ReadAsync())
                {
                    int eventID = reader.GetInt32(0);
                    eventIDs.Add(eventID);
                }

                reader.Close();

                if (eventIDs.Count > 1)
                {
                    return null;
                }

                return eventIDs;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }

        public async Task<List<int>> SelectActiveEvents(int ID_User_Teach)
        {
            try
            {
                string query = $"select ID_Event from Events where ID_User_Teach = @ID_User_Teach and IsActive = 'Да'";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@ID_User_Teach", ID_User_Teach);
                MySqlDataReader reader = await command.ExecuteReaderAsync();
                List<int> eventIDs = new List<int>();

                while (await reader.ReadAsync())
                {
                    int eventID = reader.GetInt32(0);
                    eventIDs.Add(eventID);
                }

                reader.Close();

                if (eventIDs.Count > 0)
                {
                    return null;
                }

                return eventIDs;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }

        public async Task<DataTable> SelectFromAppConfigFullData()
        {
            try
            {
                string query = $"SELECT * FROM App_config";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                MySqlDataReader reader = await command.ExecuteReaderAsync();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }

        public async Task<DataTable> SelectFromQrCodesFullData()
        {
            try
            {
                string query = $"SELECT Q.ID_QRcode, E.Event_Name, Concat(U.Name,' ', U.Surname) AS TeachFullName, Q.QR_Code_info FROM QR_Codes Q Inner join Events E on E.ID_Event = Q.ID_Event INNER join Users_Teach U on U.ID_User_Teach = E.ID_User_Teach;";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                MySqlDataReader reader = await command.ExecuteReaderAsync();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }

        public async Task<DataTable> SelectFromDisciplineFullData()
        {
            try
            {
                string query = $"Select D.ID_Discipline, D.Discipline_Name, G.Group_number from Disciplines D Inner join _Groups G on G.ID_Group = D.Group_ID;";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                MySqlDataReader reader = await command.ExecuteReaderAsync();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }

        public async Task<DataTable> SelectFromEventsFullData()
        {
            try
            {
                string query = $"SELECT E.ID_Event, CONCAT(U.Name,' ', U.Surname) AS Teach_Name, G.Group_Number, E.Event_Name, E.Event_Location, DATE_FORMAT(E.Event_Begin, '%d.%m.%Y %H:%i:%s')AS Event_Begin, DATE_FORMAT(E.Event_End, '%d.%m.%Y %H:%i:%s')AS Event_End, E.IsActive FROM Events E INNER JOIN _Groups G ON G.ID_Group = E.ID_Group INNER JOIN Users_Teach U ON U.ID_User_Teach = E.ID_User_Teach;";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                MySqlDataReader reader = await command.ExecuteReaderAsync();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }

        public async Task<DataTable> SelectAttendance(int ID_Event)
        {
            try
            {
                string query = $"SELECT E.Event_Name, DATE_FORMAT(A.Attendance_Date, '%d.%m.%Y %H:%i:%s') As Attendance_date, A.Attendance_Status, CONCAT(U.Name, ' ', U.Surname) AS UserFullName FROM Attendance A INNER JOIN Events E ON A.ID_Event = E.ID_Event INNER JOIN Users_Stud U ON A.ID_User_Stud = U.ID_User_Stud where A.ID_Event = @ID_Event;";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@ID_Event", ID_Event);
                MySqlDataReader reader = await command.ExecuteReaderAsync();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (Exception)
            {
                bool checkConnection = QueryConnection.IsConnectionOpen();
                if (!checkConnection)
                {
                    CustomMessageRetryConn retryConn = new CustomMessageRetryConn();
                    retryConn.ShowDialog();
                }
            }
            return null;
        }

        public async Task<DataTable> SelectGroupsFullData()
        {
            try
            {
                string query = $"SELECT G.ID_Group, G.Group_Number, G.Specialities, Concat(U.Name,' ', U.Surname) as CuratorFullName, G.Classroom FROM _Groups G INNER Join Users_Teach U on U.ID_User_Teach = G.Curator;";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                MySqlDataReader reader = await command.ExecuteReaderAsync();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }

        public async Task<DataTable> SelectUsersTeachFullData()
        {
            try
            {
                string query = $"SELECT ID_User_Teach, CONCAT(Name, ' ', Surname, ' ', Patronymic) as FullName, Email, Phone_Number, Specialization, Role, Login, Password, Enable FROM Users_Teach;";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                MySqlDataReader reader = await command.ExecuteReaderAsync();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }

        public async Task<DataTable> SelectUsersStudFullData()
        {
            try
            {
                string query = $"SELECT U.ID_User_Stud, CONCAT(U.Name, ' ', U.Surname, ' ', U.Patronymic) AS UserFullName, U.Phone_Model, U.Phone_Number, U.App_ID, U.Email, U.Login, U.Password, G.Group_Number FROM Users_Stud U INNER JOIN _Groups G on G.ID_Group = U.ID_Group;";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                MySqlDataReader reader = await command.ExecuteReaderAsync();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }

        public async Task<DataTable> SelectAttendanceFullData()
        {
            try
            {
                string query = $"SELECT A.ID_Attendance, CONCAT(U.Name, ' ', U.Surname, ' ', U.Patronymic) AS UserFullName, G.Group_Number, E.Event_Name, DATE_FORMAT(A.Attendance_Date, '%d.%m.%Y %H:%i:%s') As Attendance_date, A.Attendance_Status FROM Attendance A INNER JOIN Events E ON A.ID_Event = E.ID_Event INNER JOIN Users_Stud U ON A.ID_User_Stud = U.ID_User_Stud INNER JOIN _Groups G ON U.ID_Group = G.ID_Group;";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                MySqlDataReader reader = await command.ExecuteReaderAsync();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }

        public async Task<bool> QrCodeDelete(int ID_User_Teach)
        {
            try
            {
                string query = "Delete from QR_Codes where ID_User_Teach = @ID_User_Teach";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@ID_User_Teach", ID_User_Teach);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> QrCodeDeleteQrID(int ID_QRcode)
        {
            try
            {
                string querySelectQr = "select * From QR_Codes where ID_QRcode = @ID_QRcode";
                MySqlCommand commandSelectQr = new MySqlCommand(querySelectQr, QueryConnection.connection);
                commandSelectQr.Parameters.AddWithValue("@ID_QRcode", ID_QRcode);
                MySqlDataReader readerQr = await commandSelectQr.ExecuteReaderAsync();

                if (readerQr.HasRows)
                {
                    readerQr.Close();
                    string query = "Delete from QR_Codes where ID_QRcode = @ID_QRcode";
                    MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                    command.Parameters.AddWithValue("@ID_QRcode", ID_QRcode);
                    await command.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    readerQr.Close();
                    message = new CustomMessage("QR-кода с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                    return false;
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> insertGroups(string Group_Number, string Specialities, int Curator, string Classroom)
        {
            try
            {
                string queryInsert = "insert into _Groups (Group_Number, Specialities, Curator, Classroom) values (@Group_Number, @Specialities, @Curator, @Classroom)";
                MySqlCommand commandInsert = new MySqlCommand(queryInsert, QueryConnection.connection);
                commandInsert.Parameters.AddWithValue("@Group_Number", Group_Number);
                commandInsert.Parameters.AddWithValue("@Specialities", Specialities);
                commandInsert.Parameters.AddWithValue("@Curator", Curator);
                commandInsert.Parameters.AddWithValue("@Classroom", Classroom);
                await commandInsert.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> UpdateGroups(int ID_Group, string Group_Number, string Specialities, int Curator, string Classroom)
        {
            try
            {
                string querySelectGroups = "SELECT * FROM _Groups WHERE ID_Group = @ID_Group";
                MySqlCommand commandSelectGroups = new MySqlCommand(querySelectGroups, QueryConnection.connection);
                commandSelectGroups.Parameters.AddWithValue("@ID_Group", ID_Group);
                MySqlDataReader readerGroups = await commandSelectGroups.ExecuteReaderAsync();

                if (readerGroups.HasRows)
                {
                    readerGroups.Close();

                    string queryUpdate = "UPDATE _Groups SET ";
                    List<string> updateFields = new List<string>();
                    List<MySqlParameter> parameters = new List<MySqlParameter>();

                    if (!string.IsNullOrEmpty(Group_Number))
                    {
                        updateFields.Add("Group_Number = @Group_Number");
                        parameters.Add(new MySqlParameter("@Group_Number", Group_Number));
                    }

                    if (!string.IsNullOrEmpty(Specialities))
                    {
                        updateFields.Add("Specialities = @Specialities");
                        parameters.Add(new MySqlParameter("@Specialities", Specialities));
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(Curator)))
                    {
                        updateFields.Add("Curator = @Curator");
                        parameters.Add(new MySqlParameter("@Curator", Curator));
                    }

                    if (!string.IsNullOrEmpty(Classroom))
                    {
                        updateFields.Add("Classroom = @Classroom");
                        parameters.Add(new MySqlParameter("@Classroom", Classroom));
                    }

                    queryUpdate += string.Join(", ", updateFields);
                    queryUpdate += " WHERE ID_Group = @ID_Group";

                    MySqlCommand commandUpdate = new MySqlCommand(queryUpdate, QueryConnection.connection);
                    commandUpdate.Parameters.AddWithValue("@ID_Group", ID_Group);
                    commandUpdate.Parameters.AddRange(parameters.ToArray());
                    await commandUpdate.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    readerGroups.Close();
                    message = new CustomMessage("Группы с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> DeleteGroups(int ID_Group)
        {
            try
            {
                string querySelectGroups = "SELECT * FROM _Groups WHERE ID_Group = @ID_Group";
                MySqlCommand commandSelectGroups = new MySqlCommand(querySelectGroups, QueryConnection.connection);
                commandSelectGroups.Parameters.AddWithValue("@ID_Group", ID_Group);
                MySqlDataReader readerGroups = await commandSelectGroups.ExecuteReaderAsync();

                if (readerGroups.HasRows)
                {
                    readerGroups.Close();

                    string query = "Delete from _Groups where ID_Group = @ID_Group";
                    MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                    command.Parameters.AddWithValue("@ID_Group", ID_Group);
                    await command.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    readerGroups.Close();
                    message = new CustomMessage("Группы с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> insertAttendance(int ID_User, int ID_Event, DateTime Attendance_Date, string Attendance_Status)
        {
            try
            {
                string querySelectStud = "select * From Users_Stud where ID_User_Stud = @ID_User_Stud";
                MySqlCommand commandSelectStud = new MySqlCommand(querySelectStud, QueryConnection.connection);
                commandSelectStud.Parameters.AddWithValue("@ID_User_Stud", ID_User);
                MySqlDataReader readerStud = await commandSelectStud.ExecuteReaderAsync();

                if (readerStud.HasRows)
                {
                    readerStud.Close();

                    string querySelectEvents = "select * From Events where ID_Event = @ID_Event";
                    MySqlCommand commandSelectEvents = new MySqlCommand(querySelectEvents, QueryConnection.connection);
                    commandSelectEvents.Parameters.AddWithValue("@ID_Event", ID_Event);
                    MySqlDataReader readerEvents = await commandSelectEvents.ExecuteReaderAsync();

                    if (readerEvents.HasRows)
                    {
                        readerEvents.Close();

                        string queryInsert = "insert into Attendance (ID_User_Stud, ID_Event, Attendance_Date, Attendance_Status) values (@ID_User_Stud, @ID_Event, @Attendance_Date, @Attendance_Status)";
                        MySqlCommand commandInsert = new MySqlCommand(queryInsert, QueryConnection.connection);
                        commandInsert.Parameters.AddWithValue("@ID_User_Stud", ID_User);
                        commandInsert.Parameters.AddWithValue("@ID_Event", ID_Event);
                        commandInsert.Parameters.AddWithValue("@Attendance_Date", Attendance_Date);
                        commandInsert.Parameters.AddWithValue("@Attendance_Status", Attendance_Status);
                        await commandInsert.ExecuteNonQueryAsync();
                        return true;

                    }
                    else
                    {
                        readerEvents.Close();
                        message = new CustomMessage("События с таким ID не существует", "Ошибка", false, 3);
                        message.ShowDialog();
                    }
                }
                else
                {
                    readerStud.Close();
                    message = new CustomMessage("Пользователя с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> UpdateEventData(int ID_Event, string Event_Name, string Event_Location)
        {
            try
            {
                string querySelectEvents = "SELECT * FROM Events WHERE ID_Event = @ID_Event";
                MySqlCommand commandSelectEvents = new MySqlCommand(querySelectEvents, QueryConnection.connection);
                commandSelectEvents.Parameters.AddWithValue("@ID_Event", ID_Event);
                MySqlDataReader readerEvents = await commandSelectEvents.ExecuteReaderAsync();

                if (readerEvents.HasRows)
                {
                    readerEvents.Close();

                    string queryUpdate = "UPDATE Events SET ";
                    List<string> updateFields = new List<string>();
                    List<MySqlParameter> parameters = new List<MySqlParameter>();

                    if (!string.IsNullOrEmpty(Event_Name))
                    {
                        updateFields.Add("Event_Name = @Event_Name");
                        parameters.Add(new MySqlParameter("@Event_Name", Event_Name));
                    }

                    if (!string.IsNullOrEmpty(Event_Location))
                    {
                        updateFields.Add("Event_Location = @Event_Location");
                        parameters.Add(new MySqlParameter("@Event_Location", Event_Location));
                    }

                    queryUpdate += string.Join(", ", updateFields);
                    queryUpdate += " WHERE ID_Event = @ID_Event";

                    MySqlCommand commandUpdate = new MySqlCommand(queryUpdate, QueryConnection.connection);
                    commandUpdate.Parameters.AddWithValue("@ID_Event", ID_Event);
                    commandUpdate.Parameters.AddRange(parameters.ToArray());
                    await commandUpdate.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    readerEvents.Close();
                    message = new CustomMessage("События с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> updateAttendance(int ID_Attendance, int ID_User, int ID_Event, DateTime Attendance_Date, string Attendance_Status)
        {
            try
            {
                string querySelectStud = "select * From Users_Stud where ID_User_Stud = @ID_User_Stud";
                MySqlCommand commandSelectStud = new MySqlCommand(querySelectStud, QueryConnection.connection);
                commandSelectStud.Parameters.AddWithValue("@ID_User_Stud", ID_User);
                MySqlDataReader readerStud = await commandSelectStud.ExecuteReaderAsync();

                if (readerStud.HasRows)
                {
                    readerStud.Close();

                    string querySelectEvents = "select * From Events where ID_Event = @ID_Event";
                    MySqlCommand commandSelectEvents = new MySqlCommand(querySelectEvents, QueryConnection.connection);
                    commandSelectEvents.Parameters.AddWithValue("@ID_Event", ID_Event);
                    MySqlDataReader readerEvents = await commandSelectEvents.ExecuteReaderAsync();

                    if (readerEvents.HasRows)
                    {
                        readerEvents.Close();

                        string querySelectAttendance = "select * From Attendance where ID_Attendance = @ID_Attendance";
                        MySqlCommand commandSelectAttendance = new MySqlCommand(querySelectAttendance, QueryConnection.connection);
                        commandSelectAttendance.Parameters.AddWithValue("@ID_Attendance", ID_Attendance);
                        MySqlDataReader readerAttendance = await commandSelectAttendance.ExecuteReaderAsync();

                        if (readerAttendance.HasRows)
                        {
                            readerAttendance.Close();

                            string queryInsert = "update Attendance set ID_User_Stud = @ID_User_Stud, ID_Event = @ID_Event, Attendance_Date = @Attendance_Date, Attendance_Status = @Attendance_Status where ID_Attendance = @ID_Attendance";
                            MySqlCommand commandInsert = new MySqlCommand(queryInsert, QueryConnection.connection);
                            commandInsert.Parameters.AddWithValue("@ID_Attendance", ID_Attendance);
                            commandInsert.Parameters.AddWithValue("@ID_User_Stud", ID_User);
                            commandInsert.Parameters.AddWithValue("@ID_Event", ID_Event);
                            commandInsert.Parameters.AddWithValue("@Attendance_Date", Attendance_Date);
                            commandInsert.Parameters.AddWithValue("@Attendance_Status", Attendance_Status);
                            await commandInsert.ExecuteNonQueryAsync();
                            return true;
                        }
                        else
                        {
                            readerAttendance.Close();
                            message = new CustomMessage("Записи посещения с таким ID не существует", "Ошибка", false, 3);
                            message.ShowDialog();
                        }
                    }
                    else
                    {
                        readerEvents.Close();
                        message = new CustomMessage("События с таким ID не существует", "Ошибка", false, 3);
                        message.ShowDialog();
                    }
                }
                else
                {
                    readerStud.Close();
                    message = new CustomMessage("Пользователя с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> insertAppConfig(string IP, string Telegram_ID)
        {
            try
            {
                string querySelect = "select * from App_config;";
                MySqlCommand commandSelect = new MySqlCommand(querySelect, QueryConnection.connection);
                await commandSelect.ExecuteNonQueryAsync();
                MySqlDataReader reader = await commandSelect.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    reader.Close();
                    message = new CustomMessage("Для избежания возмоных неполадок нельзя добавить более 1 параметра", "Ошибка", false, 3);
                    message.ShowDialog();
                    return false;
                }
                else
                {
                    reader.Close();
                    string query = "INSERT INTO App_config(IP, Telegram_ID) VALUES (@IP, @Telegram_ID)";
                    MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                    command.Parameters.AddWithValue("@IP", IP);
                    command.Parameters.AddWithValue("@Telegram_ID", Telegram_ID);
                    await command.ExecuteNonQueryAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> deleteAppConfig(int IdAppConfig)
        {
            try
            {
                string querySelectIdConfig = "select * From App_config where ID_Config = @ID_Config";
                MySqlCommand commandSelectIdConfig = new MySqlCommand(querySelectIdConfig, QueryConnection.connection);
                commandSelectIdConfig.Parameters.AddWithValue("@ID_Config", IdAppConfig);
                MySqlDataReader readerConfig = await commandSelectIdConfig.ExecuteReaderAsync();

                if (readerConfig.HasRows)
                {
                    readerConfig.Close();
                    string query = "delete from App_config where ID_Config = @ID_Config";
                    MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                    command.Parameters.AddWithValue("@ID_Config", IdAppConfig);
                    await command.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    readerConfig.Close();
                    message = new CustomMessage("Параметра с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> deleteAttendance(int ID_Attendance)
        {
            try
            {
                string querySelectIdAttendance = "select * From Attendance where ID_Attendance = @ID_Attendance";
                MySqlCommand commandSelectIdAttendance = new MySqlCommand(querySelectIdAttendance, QueryConnection.connection);
                commandSelectIdAttendance.Parameters.AddWithValue("@ID_Attendance", ID_Attendance);
                MySqlDataReader readerIdAttendance = await commandSelectIdAttendance.ExecuteReaderAsync();

                if (readerIdAttendance.HasRows)
                {
                    readerIdAttendance.Close();
                    string query = "delete from Attendance where ID_Attendance = @ID_Attendance";
                    MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                    command.Parameters.AddWithValue("@ID_Attendance", ID_Attendance);
                    await command.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    readerIdAttendance.Close();
                    message = new CustomMessage("Записи посещения с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                }

            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> DeleteUserStud(int ID_User_Stud)
        {
            try
            {
                string querySelectIdUser = "SELECT * FROM Users_Stud WHERE ID_User_Stud = @ID_User_Stud";
                MySqlCommand commandSelectIdUser = new MySqlCommand(querySelectIdUser, QueryConnection.connection);
                commandSelectIdUser.Parameters.AddWithValue("@ID_User_Stud", ID_User_Stud);
                MySqlDataReader readerIdUser = await commandSelectIdUser.ExecuteReaderAsync();

                if (readerIdUser.HasRows)
                {
                    readerIdUser.Close();
                    string queryDelete = "DELETE FROM Users_Stud WHERE ID_User_Stud = @ID_User_Stud";
                    MySqlCommand commandDelete = new MySqlCommand(queryDelete, QueryConnection.connection);
                    commandDelete.Parameters.AddWithValue("@ID_User_Stud", ID_User_Stud);
                    await commandDelete.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    readerIdUser.Close();
                    message = new CustomMessage("Пользователя с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> SetAppIDToNull(int ID_User_Stud)
        {
            try
            {
                string queryUpdate = "UPDATE Users_Stud SET App_ID = NULL WHERE ID_User_Stud = @ID_User_Stud";
                MySqlCommand commandUpdate = new MySqlCommand(queryUpdate, QueryConnection.connection);
                commandUpdate.Parameters.AddWithValue("@ID_User_Stud", ID_User_Stud);
                await commandUpdate.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> updateAppConfig(int IdAppConfig, string IP, string TelegramID)
        {
            try
            {
                string querySelectIdConfig = "select * From App_config where ID_Config = @ID_Config";
                MySqlCommand commandSelectIdConfig = new MySqlCommand(querySelectIdConfig, QueryConnection.connection);
                commandSelectIdConfig.Parameters.AddWithValue("@ID_Config", IdAppConfig);
                MySqlDataReader readerConfig = await commandSelectIdConfig.ExecuteReaderAsync();

                if (readerConfig.HasRows)
                {
                    readerConfig.Close();
                    string query = "UPDATE App_config SET ";
                    List<string> updateFields = new List<string>();
                    List<MySqlParameter> parameters = new List<MySqlParameter>();

                    if (!string.IsNullOrEmpty(IP))
                    {
                        updateFields.Add("IP = @IP");
                        parameters.Add(new MySqlParameter("@IP", IP));
                    }

                    if (!string.IsNullOrEmpty(TelegramID))
                    {
                        updateFields.Add("Telegram_ID = @Telegram_ID");
                        parameters.Add(new MySqlParameter("@Telegram_ID", TelegramID));
                    }

                    query += string.Join(", ", updateFields);
                    query += " WHERE ID_Config = @ID_Config";

                    MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                    command.Parameters.AddWithValue("@ID_Config", IdAppConfig);
                    command.Parameters.AddRange(parameters.ToArray());
                    await command.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    readerConfig.Close();
                    message = new CustomMessage("Параметра с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                }

            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> insertDiscipline(string Discipline_Name, int Group_ID)
        {
            try
            {
                string querySelectGroup = "select * From Disciplines where Group_ID = @Group_ID";
                MySqlCommand commandSelectGroup = new MySqlCommand(querySelectGroup, QueryConnection.connection);
                commandSelectGroup.Parameters.AddWithValue("@Group_ID", Group_ID);
                MySqlDataReader readerGroup = await commandSelectGroup.ExecuteReaderAsync();

                if (readerGroup.HasRows)
                {
                    readerGroup.Close();

                    string queryInsert = "insert into Disciplines (Discipline_Name, Group_ID) values (@Discipline_Name, @Group_ID)";
                    MySqlCommand commandInsert = new MySqlCommand(queryInsert, QueryConnection.connection);
                    commandInsert.Parameters.AddWithValue("@Discipline_Name", Discipline_Name);
                    commandInsert.Parameters.AddWithValue("@Group_ID", Group_ID);
                    await commandInsert.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    readerGroup.Close();
                    message = new CustomMessage("Группы с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> updateDiscipline(int ID_Discipline, string Discipline_Name, int Group_ID)
        {
            try
            {
                string querySelectIdDiscipline = "select * From Disciplines where ID_Discipline = @ID_Discipline";
                MySqlCommand commandSelectIdDiscipline = new MySqlCommand(querySelectIdDiscipline, QueryConnection.connection);
                commandSelectIdDiscipline.Parameters.AddWithValue("@ID_Discipline", ID_Discipline);
                MySqlDataReader readerIdDiscipline = await commandSelectIdDiscipline.ExecuteReaderAsync();

                if (readerIdDiscipline.HasRows)
                {
                    readerIdDiscipline.Close();
                    string query = "UPDATE Disciplines SET Discipline_Name = @Discipline_Name, Group_ID = @Group_ID WHERE ID_Discipline = @ID_Discipline";
                    MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                    command.Parameters.AddWithValue("@ID_Discipline", ID_Discipline);
                    command.Parameters.AddWithValue("@Discipline_Name", Discipline_Name);
                    command.Parameters.AddWithValue("@Group_ID", Group_ID);
                    await command.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    readerIdDiscipline.Close();
                    message = new CustomMessage("Предмета с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                }

            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> deleteDiscipline(int ID_Discipline)
        {
            try
            {
                string querySelectIdDiscipline = "select * From Disciplines where ID_Discipline = @ID_Discipline";
                MySqlCommand commandSelectIdDiscipline = new MySqlCommand(querySelectIdDiscipline, QueryConnection.connection);
                commandSelectIdDiscipline.Parameters.AddWithValue("@ID_Discipline", ID_Discipline);
                MySqlDataReader readerIdDiscipline = await commandSelectIdDiscipline.ExecuteReaderAsync();

                if (readerIdDiscipline.HasRows)
                {
                    readerIdDiscipline.Close();
                    string query = "delete from Disciplines where ID_Discipline = @ID_Discipline";
                    MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                    command.Parameters.AddWithValue("@ID_Discipline", ID_Discipline);
                    await command.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    readerIdDiscipline.Close();
                    message = new CustomMessage("Предмета с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                }

            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> RegisterUser(string Login, string Email, string Password)
        {
            try
            {
                string query = "insert into Users_Teach (Login,Email,Password,Role,Enable) Values (@Login, @Email, @Password, 'User', 'Да')";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@Login", Login);
                command.Parameters.AddWithValue("@Email", Email);
                command.Parameters.AddWithValue("@Password", Password);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> QrCodeUpdate(int ID_User_Teach, string QR_Code_info)
        {
            try
            {
                string query = "Update QR_Codes Set QR_Code_info = @QR_Code_info where ID_User_Teach = @ID_User_Teach";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@ID_User_Teach", ID_User_Teach);
                command.Parameters.AddWithValue("@QR_Code_info", QR_Code_info);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> QrCodeGenerate(int ID_Event, int ID_User_Teach, string QR_Code_info)
        {
            try
            {
                string query = "insert into QR_Codes (ID_Event, ID_User_Teach, QR_Code_info) values (@ID_Event, @ID_User_Teach, @QR_Code_info)";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@ID_Event", ID_Event);
                command.Parameters.AddWithValue("@ID_User_Teach", ID_User_Teach);
                command.Parameters.AddWithValue("@QR_Code_info", QR_Code_info);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> EventCreate(int ID_User_Teach, int? ID_Group, string Event_Name, string Event_Location, string Event_Begin, string Event_End, string IsActive)
        {
            try
            {
                string query = "insert into Events (ID_User_Teach, ID_Group, Event_Name, Event_Location, Event_Begin, Event_End, IsActive) values (@ID_User_Teach, @ID_Group, @Event_Name, @Event_Location, @Event_Begin, @Event_End, @IsActive)";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@ID_User_Teach", ID_User_Teach);
                command.Parameters.AddWithValue("@ID_Group", ID_Group);
                command.Parameters.AddWithValue("@Event_Name", Event_Name);
                command.Parameters.AddWithValue("@Event_Location", Event_Location);
                command.Parameters.AddWithValue("@Event_Begin", Event_Begin);
                command.Parameters.AddWithValue("@Event_End", Event_End);
                command.Parameters.AddWithValue("@IsActive", IsActive);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<int> CheckEmail(string email)
        {
            try
            {
                string query = $"select * from Users_Teach where BINARY Email = @Email";
                MySqlCommand mySqlCommand = new MySqlCommand(query, QueryConnection.connection);
                mySqlCommand.Parameters.AddWithValue("@Email", email);
                MySqlDataReader reader = await mySqlCommand.ExecuteReaderAsync();

                int ID_User = 0;

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        ID_User = reader.GetInt32("ID_User_Teach");
                    }
                    reader.Close();

                    return ID_User;
                }
                reader.Close();
                return 0;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return 0;
        }

        public async Task<int> CheckEmailStud(string email)
        {
            try
            {
                string query = $"select * from Users_Stud where BINARY Email = @Email";
                MySqlCommand mySqlCommand = new MySqlCommand(query, QueryConnection.connection);
                mySqlCommand.Parameters.AddWithValue("@Email", email);
                MySqlDataReader reader = await mySqlCommand.ExecuteReaderAsync();

                int ID_User = 0;

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        ID_User = reader.GetInt32("ID_User_Stud");
                    }
                    reader.Close();

                    return ID_User;
                }
                reader.Close();
                return 0;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return 0;
        }

        public async Task<bool> UpdateUserData(int ID_User_Teach, string Login, string Password)
        {
            try
            {
                string query = $"Update Users_Teach set Password = @Password, Login = @Login where ID_User_Teach = @ID_User_Teach";
                MySqlCommand mySqlCommand = new MySqlCommand(query, QueryConnection.connection);
                mySqlCommand.Parameters.AddWithValue("@ID_User_Teach", ID_User_Teach);
                mySqlCommand.Parameters.AddWithValue("@Login", Login);
                mySqlCommand.Parameters.AddWithValue("@Password", Password);
                await mySqlCommand.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<DataClass> SelectFromUsers(string login, string password)
        {
            try
            {
                string query = "select * From Users_Teach where BINARY Login = @login and BINARY Password = @password LIMIT 1";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);
                MySqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        _dataClass.ID_User = reader.GetInt32("ID_User_Teach");
                        _dataClass.Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString("Name");
                        _dataClass.Surname = reader.IsDBNull(reader.GetOrdinal("Surname")) ? string.Empty : reader.GetString("Surname");
                        _dataClass.Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString("Email");
                        _dataClass.Phone_Number = reader.IsDBNull(reader.GetOrdinal("Phone_Number")) ? string.Empty : reader.GetString("Phone_Number");
                        _dataClass.Specialization = reader.IsDBNull(reader.GetOrdinal("Specialization")) ? string.Empty : reader.GetString("Specialization");
                        _dataClass.Role = reader.GetString("Role");
                        _dataClass.Login = reader.GetString("Login");
                        _dataClass.Password = reader.GetString("Password");
                        _dataClass.Patronymic = reader.IsDBNull(reader.GetOrdinal("Patronymic")) ? string.Empty : reader.GetString("Patronymic");
                        _dataClass.Enable = reader.GetString("Enable");
                    }
                    if (_dataClass.Enable == "Нет")
                    {
                        message = new CustomMessage("Аккаунт не активен", "Ошибка", false, 3);
                        message.ShowDialog();
                        reader.Close();
                        return null;
                    }
                    else
                    {
                        reader.Close();
                        return _dataClass;
                    }
                }
                else
                {
                    reader.Close();
                    message = new CustomMessage("Неверный логин или пароль", "Ошибка", false, 3);
                    message.ShowDialog();
                    return null;
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }

        public async Task<bool> UpdateUsersMainData(int ID_User_Teach, string Name, string Surname, string Patronymic, string Email, string Phone_Number, string Specialization)
        {
            try
            {
                string query = $"update Users_Teach set Name = @Name, Surname = @Surname, Patronymic = @Patronymic, Email = @Email, Phone_Number = @Phone_Number, Specialization = @Specialization where ID_User_Teach = @ID_User_Teach";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@ID_User_Teach", ID_User_Teach);
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@Surname", Surname);
                command.Parameters.AddWithValue("@Patronymic", Patronymic);
                command.Parameters.AddWithValue("@Email", Email);
                command.Parameters.AddWithValue("@Phone_Number", Phone_Number);
                command.Parameters.AddWithValue("@Specialization", Specialization);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> UpdateUsersAuthData(int ID_User_Teach, string Login, string NewPassword)
        {
            try
            {
                string query = "update Users_Teach set Login = @Login, Password = @NewPassword where ID_User_Teach = @ID_User_Teach";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@ID_User_Teach", ID_User_Teach);
                command.Parameters.AddWithValue("@Login", Login);
                command.Parameters.AddWithValue("@NewPassword", NewPassword);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> UpdateUserStud(int ID_User_Stud, string Name, string Surname, string Patronymic, string Phone_Model, string Phone_Number, string Email, string Login, string Password, int ID_Group, string Enable)
        {
            try
            {
                string querySelectUser = "SELECT * FROM Users_Stud WHERE ID_User_Stud = @ID_User_Stud";
                MySqlCommand commandSelectUser = new MySqlCommand(querySelectUser, QueryConnection.connection);
                commandSelectUser.Parameters.AddWithValue("@ID_User_Stud", ID_User_Stud);
                MySqlDataReader readerUser = await commandSelectUser.ExecuteReaderAsync();

                if (readerUser.HasRows)
                {
                    readerUser.Close();

                    string queryUpdate = "UPDATE Users_Stud SET ";
                    List<string> updateFields = new List<string>();
                    List<MySqlParameter> parameters = new List<MySqlParameter>();

                    if (!string.IsNullOrEmpty(Name))
                    {
                        updateFields.Add("Name = @Name");
                        parameters.Add(new MySqlParameter("@Name", Name));
                    }

                    if (!string.IsNullOrEmpty(Surname))
                    {
                        updateFields.Add("Surname = @Surname");
                        parameters.Add(new MySqlParameter("@Surname", Surname));
                    }

                    if (!string.IsNullOrEmpty(Patronymic))
                    {
                        updateFields.Add("Patronymic = @Patronymic");
                        parameters.Add(new MySqlParameter("@Patronymic", Patronymic));
                    }

                    if (!string.IsNullOrWhiteSpace(Phone_Model))
                    {
                        updateFields.Add("Phone_Model = @Phone_Model");
                        parameters.Add(new MySqlParameter("@Phone_Model", Phone_Model));
                    }

                    if (!string.IsNullOrEmpty(Phone_Number))
                    {
                        updateFields.Add("Phone_Number = @Phone_Number");
                        parameters.Add(new MySqlParameter("@Phone_Number", Phone_Number));
                    }

                    if (!string.IsNullOrEmpty(Email))
                    {
                        updateFields.Add("Email = @Email");
                        parameters.Add(new MySqlParameter("@Email", Email));
                    }

                    if (!string.IsNullOrEmpty(Login))
                    {
                        updateFields.Add("Login = @Login");
                        parameters.Add(new MySqlParameter("@Login", Login));
                    }

                    if (!string.IsNullOrEmpty(Password))
                    {
                        updateFields.Add("Password = @Password");
                        parameters.Add(new MySqlParameter("@Password", Password));
                    }

                    if (ID_Group != 0)
                    {
                        updateFields.Add("ID_Group = @ID_Group");
                        parameters.Add(new MySqlParameter("@ID_Group", ID_Group));
                    }

                    if (!string.IsNullOrEmpty(Enable))
                    {
                        updateFields.Add("Enable = @Enable");
                        parameters.Add(new MySqlParameter("@Enable", Enable));
                    }

                    queryUpdate += string.Join(", ", updateFields);
                    queryUpdate += " WHERE ID_User_Stud = @ID_User_Stud";

                    MySqlCommand commandUpdate = new MySqlCommand(queryUpdate, QueryConnection.connection);
                    commandUpdate.Parameters.AddWithValue("@ID_User_Stud", ID_User_Stud);
                    commandUpdate.Parameters.AddRange(parameters.ToArray());
                    await commandUpdate.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    readerUser.Close();
                    message = new CustomMessage("Пользователя с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> UpdateUserTeach(int ID_User_Teach, string Name, string Surname, string Patronymic, string Email, string Phone_Number, string Specialization, string Role, string Login, string Password, string Enable)
        {
            try
            {
                string querySelectUser = "SELECT * FROM Users_Teach WHERE ID_User_Teach = @ID_User_Teach";
                MySqlCommand commandSelectUser = new MySqlCommand(querySelectUser, QueryConnection.connection);
                commandSelectUser.Parameters.AddWithValue("@ID_User_Teach", ID_User_Teach);
                MySqlDataReader readerUser = await commandSelectUser.ExecuteReaderAsync();

                if (readerUser.HasRows)
                {
                    readerUser.Close();

                    string queryUpdate = "UPDATE Users_Teach SET ";
                    List<string> updateFields = new List<string>();
                    List<MySqlParameter> parameters = new List<MySqlParameter>();

                    if (!string.IsNullOrEmpty(Name))
                    {
                        updateFields.Add("Name = @Name");
                        parameters.Add(new MySqlParameter("@Name", Name));
                    }

                    if (!string.IsNullOrEmpty(Surname))
                    {
                        updateFields.Add("Surname = @Surname");
                        parameters.Add(new MySqlParameter("@Surname", Surname));
                    }

                    if (!string.IsNullOrEmpty(Patronymic))
                    {
                        updateFields.Add("Patronymic = @Patronymic");
                        parameters.Add(new MySqlParameter("@Patronymic", Patronymic));
                    }

                    if (!string.IsNullOrEmpty(Email))
                    {
                        updateFields.Add("Email = @Email");
                        parameters.Add(new MySqlParameter("@Email", Email));
                    }

                    if (!string.IsNullOrEmpty(Phone_Number))
                    {
                        updateFields.Add("Phone_Number = @Phone_Number");
                        parameters.Add(new MySqlParameter("@Phone_Number", Phone_Number));
                    }

                    if (!string.IsNullOrEmpty(Specialization))
                    {
                        updateFields.Add("Specialization = @Specialization");
                        parameters.Add(new MySqlParameter("@Specialization", Specialization));
                    }

                    if (!string.IsNullOrEmpty(Role))
                    {
                        updateFields.Add("Role = @Role");
                        parameters.Add(new MySqlParameter("@Role", Role));
                    }

                    if (!string.IsNullOrEmpty(Login))
                    {
                        updateFields.Add("Login = @Login");
                        parameters.Add(new MySqlParameter("@Login", Login));
                    }

                    if (!string.IsNullOrEmpty(Password))
                    {
                        updateFields.Add("Password = @Password");
                        parameters.Add(new MySqlParameter("@Password", Password));
                    }

                    if (!string.IsNullOrEmpty(Enable))
                    {
                        updateFields.Add("Enable = @Enable");
                        parameters.Add(new MySqlParameter("@Enable", Enable));
                    }

                    queryUpdate += string.Join(", ", updateFields);
                    queryUpdate += " WHERE ID_User_Teach = @ID_User_Teach";

                    MySqlCommand commandUpdate = new MySqlCommand(queryUpdate, QueryConnection.connection);
                    commandUpdate.Parameters.AddWithValue("@ID_User_Teach", ID_User_Teach);
                    commandUpdate.Parameters.AddRange(parameters.ToArray());
                    await commandUpdate.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    readerUser.Close();
                    message = new CustomMessage("Пользователя с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> InsertUserStud(string Name, string Surname, string Patronymic, string Phone_Model, string Phone_Number, string Email, string Login, string Password, int ID_Group, string Enable)
        {
            try
            {
                string queryInsert = "INSERT INTO Users_Stud (Name, Surname, Patronymic, Phone_Model, Phone_Number, Email, Login, Password, ID_Group, Enable) VALUES (@Name, @Surname, @Patronymic, @Phone_Model, @Phone_Number, @Email, @Login, @Password, @ID_Group, @Enable)";
                MySqlCommand commandInsert = new MySqlCommand(queryInsert, QueryConnection.connection);
                commandInsert.Parameters.AddWithValue("@Name", Name);
                commandInsert.Parameters.AddWithValue("@Surname", Surname);
                commandInsert.Parameters.AddWithValue("@Patronymic", Patronymic);
                commandInsert.Parameters.AddWithValue("@Phone_Model", Phone_Model);
                commandInsert.Parameters.AddWithValue("@Phone_Number", Phone_Number);
                commandInsert.Parameters.AddWithValue("@Email", Email);
                commandInsert.Parameters.AddWithValue("@Login", Login);
                commandInsert.Parameters.AddWithValue("@Password", Password);
                commandInsert.Parameters.AddWithValue("@ID_Group", ID_Group);
                commandInsert.Parameters.AddWithValue("@Enable", Enable);
                await commandInsert.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> InsertUserTeach(string Name, string Surname, string Patronymic, string Email, string Phone_Number, string Specialization, string Role, string Login, string Password, string Enable)
        {
            try
            {
                string queryInsert = "INSERT INTO Users_Teach (Name, Surname, Patronymic, Email, Phone_Number, Specialization, Role, Login, Password, Enable) VALUES (@Name, @Surname, @Patronymic, @Email, @Phone_Number, @Specialization, @Role, @Login, @Password, @Enable)";
                MySqlCommand commandInsert = new MySqlCommand(queryInsert, QueryConnection.connection);
                commandInsert.Parameters.AddWithValue("@Name", Name);
                commandInsert.Parameters.AddWithValue("@Surname", Surname);
                commandInsert.Parameters.AddWithValue("@Patronymic", Patronymic);
                commandInsert.Parameters.AddWithValue("@Email", Email);
                commandInsert.Parameters.AddWithValue("@Phone_Number", Phone_Number);
                commandInsert.Parameters.AddWithValue("@Specialization", Specialization);
                commandInsert.Parameters.AddWithValue("@Role", Role);
                commandInsert.Parameters.AddWithValue("@Login", Login);
                commandInsert.Parameters.AddWithValue("@Password", Password);
                commandInsert.Parameters.AddWithValue("@Enable", Enable);
                await commandInsert.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }


        public async Task<bool> UpdateUserTeach(int ID_User_Teach, string Name, string Surname, string Patronymic, string Email, string Phone_Number, string Specialization, string Role, string Login, string Password, bool Enable)
        {
            try
            {
                string querySelectUserTeach = "SELECT * FROM Users_Teach WHERE ID_User_Teach = @ID_User_Teach";
                MySqlCommand commandSelectUserTeach = new MySqlCommand(querySelectUserTeach, QueryConnection.connection);
                commandSelectUserTeach.Parameters.AddWithValue("@ID_User_Teach", ID_User_Teach);
                MySqlDataReader readerUserTeach = await commandSelectUserTeach.ExecuteReaderAsync();

                if (readerUserTeach.HasRows)
                {
                    readerUserTeach.Close();

                    string queryUpdate = "UPDATE Users_Teach SET ";
                    List<string> updateFields = new List<string>();
                    List<MySqlParameter> parameters = new List<MySqlParameter>();

                    if (!string.IsNullOrEmpty(Name))
                    {
                        updateFields.Add("Name = @Name");
                        parameters.Add(new MySqlParameter("@Name", Name));
                    }

                    if (!string.IsNullOrEmpty(Surname))
                    {
                        updateFields.Add("Surname = @Surname");
                        parameters.Add(new MySqlParameter("@Surname", Surname));
                    }

                    if (!string.IsNullOrEmpty(Patronymic))
                    {
                        updateFields.Add("Patronymic = @Patronymic");
                        parameters.Add(new MySqlParameter("@Patronymic", Patronymic));
                    }

                    if (!string.IsNullOrEmpty(Email))
                    {
                        updateFields.Add("Email = @Email");
                        parameters.Add(new MySqlParameter("@Email", Email));
                    }

                    if (!string.IsNullOrEmpty(Phone_Number))
                    {
                        updateFields.Add("Phone_Number = @Phone_Number");
                        parameters.Add(new MySqlParameter("@Phone_Number", Phone_Number));
                    }

                    if (!string.IsNullOrEmpty(Specialization))
                    {
                        updateFields.Add("Specialization = @Specialization");
                        parameters.Add(new MySqlParameter("@Specialization", Specialization));
                    }

                    if (!string.IsNullOrEmpty(Role))
                    {
                        updateFields.Add("Role = @Role");
                        parameters.Add(new MySqlParameter("@Role", Role));
                    }

                    if (!string.IsNullOrEmpty(Login))
                    {
                        updateFields.Add("Login = @Login");
                        parameters.Add(new MySqlParameter("@Login", Login));
                    }

                    if (!string.IsNullOrEmpty(Password))
                    {
                        updateFields.Add("Password = @Password");
                        parameters.Add(new MySqlParameter("@Password", Password));
                    }

                    updateFields.Add("Enable = @Enable");
                    parameters.Add(new MySqlParameter("@Enable", Enable));

                    queryUpdate += string.Join(", ", updateFields);
                    queryUpdate += " WHERE ID_User_Teach = @ID_User_Teach";

                    MySqlCommand commandUpdate = new MySqlCommand(queryUpdate, QueryConnection.connection);
                    commandUpdate.Parameters.AddWithValue("@ID_User_Teach", ID_User_Teach);
                    commandUpdate.Parameters.AddRange(parameters.ToArray());
                    await commandUpdate.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    readerUserTeach.Close();
                    message = new CustomMessage("Пользователя с таким ID не существует", "Ошибка", false, 3);
                    message.ShowDialog();
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> DisableUser(int ID_User_Teach)
        {
            try
            {
                string query = "update Users_Teach set Enable = 'Нет' where ID_User_Teach = @ID_User_Teach";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@ID_User_Teach", ID_User_Teach);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> SelectUsersUnmarked(int ID_Group, int ID_Event, string Event_End)
        {
            try
            {
                string query = "INSERT INTO Attendance (ID_User_Stud, ID_Event, Attendance_Date, Attendance_Status) SELECT us.ID_User_Stud, @ID_Event, @Event_End, 'Отсутствовал' FROM Users_Stud us WHERE us.ID_Group = @ID_Group AND NOT EXISTS (SELECT * FROM Attendance a INNER JOIN Events e ON a.ID_Event = e.ID_Event WHERE a.ID_User_Stud = us.ID_User_Stud AND a.ID_Event = @ID_Event AND e.Event_End = @Event_End);";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@ID_Group", ID_Group);
                command.Parameters.AddWithValue("@ID_Event", ID_Event);
                command.Parameters.AddWithValue("@Event_End", Event_End);
                int affectedRows = await command.ExecuteNonQueryAsync();

                if (affectedRows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> SelectUsersLogin(string Login)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Users_Teach WHERE Binary Login = @Login";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@Login", Login);

                int count = Convert.ToInt32(await command.ExecuteScalarAsync());

                return count == 0;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<bool> SelectUsersStudLogin(string Login)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Users_Stud WHERE Binary Login = @Login";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@Login", Login);

                int count = Convert.ToInt32(await command.ExecuteScalarAsync());

                return count == 0;
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return false;
        }

        public async Task<List<string>> SelectUsersForAttendace()
        {
            try
            {
                List<string> users = new List<string>();

                string query = "SELECT CONCAT(ID_User_Stud, ' - ', Name, ' ', Surname, ' - ', G.Group_Number) AS Result from Users_Stud U Inner join _Groups G on G.ID_Group = U.ID_Group;";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                MySqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        string user = reader.GetString(0);
                        users.Add(user);
                    }
                    reader.Close();
                    return users;
                }
                else
                {
                    users.Add("<Нет данных>");
                    reader.Close();
                    return users;
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }

        public async Task<List<string>> SelectCurators()
        {
            try
            {
                List<string> teachers = new List<string>();

                string query = "SELECT CONCAT(ID_User_Teach, ' - ', CASE WHEN Name IS NOT NULL AND Name <> '' THEN CONCAT(Name, ' ') ELSE '' END, CASE WHEN Surname IS NOT NULL AND Surname <> '' THEN CONCAT(Surname, ' ') ELSE '' END, CASE WHEN Patronymic IS NOT NULL AND Patronymic <> '' THEN CONCAT(Patronymic, ' - ') ELSE '' END, Email) AS Result FROM Users_Teach WHERE NOT EXISTS (SELECT * FROM _Groups WHERE _Groups.Curator = Users_Teach.ID_User_Teach)";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                MySqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        string teachersID = reader.IsDBNull(0) ? "<Нет данных>" : reader.GetString(0);
                        teachers.Add(teachersID);
                    }
                    reader.Close();
                    return teachers;
                }
                else
                {
                    teachers.Add("<Нет данных>");
                    reader.Close();
                    return teachers;
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }

        public async Task<List<string>> SelectEventsForAttendace()
        {
            try
            {
                List<string> events = new List<string>();

                string query = "SELECT CONCAT_WS(' - ', E.ID_Event, E.Event_Name, G.Group_Number, DATE_FORMAT(E.Event_Begin, '%d.%m.%Y %H:%i:%s')) AS Result FROM Events E INNER JOIN _Groups G ON G.ID_Group = E.ID_Group;";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                MySqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        string user = reader.GetString(0);
                        events.Add(user);
                    }
                    reader.Close();
                    return events;
                }
                else
                {
                    events.Add("<Нет данных>");
                    reader.Close();
                    return events;
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }


        public async Task<List<string>> SelectFromGroups()
        {
            try
            {
                List<string> groupNumbers = new List<string>();

                string query = "SELECT CONCAT(ID_Group, ' - ', Group_Number) AS Result FROM _Groups";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                MySqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        string groupNumber = reader.GetString(0);
                        groupNumbers.Add(groupNumber);
                    }
                    reader.Close();
                    return groupNumbers;
                }
                else
                {
                    groupNumbers.Add("<Нет данных>");
                    reader.Close();
                    return groupNumbers;
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }

        public async Task<List<string>> SelectFromDisciplines(int Group_ID)
        {
            try
            {
                List<string> disciplineNumbers = new List<string>();

                string query = "select concat(ID_Discipline, ' - ', Discipline_Name) As Result From Disciplines where Group_ID = @Group_ID";
                MySqlCommand command = new MySqlCommand(query, QueryConnection.connection);
                command.Parameters.AddWithValue("@Group_ID", Group_ID);
                MySqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        string groupNumber = reader.GetString(0);
                        disciplineNumbers.Add(groupNumber);
                    }
                    reader.Close();
                    return disciplineNumbers;
                }
                else
                {
                    disciplineNumbers.Add("<Нет данных>");
                    reader.Close();
                    return disciplineNumbers;
                }
            }
            catch (Exception)
            {
                ExceptionAnswer();
            }
            return null;
        }

        private void ExceptionAnswer()
        {
            bool checkConnection = QueryConnection.IsConnectionOpen();
            if (checkConnection)
            {
                message = new CustomMessage("Произошла неизвестная ошибка. Повторите попытку!", "Ошибка", false, 3);
                message.ShowDialog();
            }
            else
            {
                CustomMessageRetryConn retryConn = new CustomMessageRetryConn();
                retryConn.ShowDialog();
            }
        }
    }
}
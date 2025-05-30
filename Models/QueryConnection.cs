using MySqlConnector;
using System;
using System.Data;
using System.Threading.Tasks;

namespace QR_Checking_winVersion
{
    class QueryConnection
    {
        static string connectionString = "server=db4free.net;port=3306;user=dimedroll;password=dimasik123;database=qr_checking_db;";
        private static MySqlConnection sqlConnection = null;
        private static bool isConnectionOpen = false;

        public static MySqlConnection connection
        {
            get
            {
                if (sqlConnection == null)
                {
                    sqlConnection = new MySqlConnection(connectionString);
                    return sqlConnection;
                }
                return sqlConnection;
            }
        }
        public static async Task<bool> OpenConnection()
        {
            while (!isConnectionOpen)
            {
                try
                {
                    if (connection.State != ConnectionState.Connecting)
                    {
                        await connection.OpenAsync();
                        isConnectionOpen = true;
                    }                    

                    if (isConnectionOpen)
                    {
                        return true;
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2));
                    }
                }
                catch (Exception)
                {
                    await CloseConnection();
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    return false;
                }
            }
            return false;
        }

        public static async Task CloseConnection()
        {
            await connection.CloseAsync();
            isConnectionOpen = false;
        }

        public static bool IsConnectionOpen()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                isConnectionOpen = true;
                return true;
            }
            else
            {
                isConnectionOpen = false;
                return false;
            }
        }
    }
}

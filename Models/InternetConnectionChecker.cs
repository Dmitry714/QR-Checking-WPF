using System.Net;
using System.Threading.Tasks;

namespace QR_Checking_winVersion
{
    public class InternetConnectionChecker
    {
        public static bool IsInternetConnected()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    using (client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> InternetChecking()
        {
            bool isConnected = await Task.Run(() => IsInternetConnected());
            if (isConnected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

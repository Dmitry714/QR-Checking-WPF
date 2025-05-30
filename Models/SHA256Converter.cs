using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace QR_Checking_winVersion
{
    public class SHA256Converter
    {
        public static string ConvertToSHA256(string password)
        {
            SHA256 sha = SHA256.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
            byte[] hashBytes = sha.ComputeHash(inputBytes);
            string hexString = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            return hexString;
        }

    }
}

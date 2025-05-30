using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Threading.Tasks;

namespace QR_Checking_winVersion
{
    public class MailSender
    {
        private static CustomMessage message;
        public static async Task<bool> SendEmail(string subject, string body, string Email)
        {
            try
            {
                bool checkInternet = await InternetConnectionChecker.InternetChecking();
                if (checkInternet)
                {
                    string fromAddress = @"qr.checking@gmail.com";
                    string fromPassword = @"lqpkwkpjhrfzgmfq";
                    string toAddress = Email;

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("", fromAddress));
                    message.To.Add(new MailboxAddress("", toAddress));
                    message.Subject = subject;
                    message.Body = new TextPart("plain")
                    {
                        Text = body
                    };

                    var smtpClient = new SmtpClient();
                    smtpClient.Connect("smtp.gmail.com", 465, true);
                    smtpClient.Authenticate(fromAddress, fromPassword);

                    await smtpClient.SendAsync(message);
                    smtpClient.Disconnect(true);
                    return true;
                }
                else
                {
                    message = new CustomMessage("Нет доступа в интернет", "Ошибка", false, 3);
                    message.ShowDialog();
                    return false;
                }
            }
            catch (Exception)
            {
                message = new CustomMessage("Не удалось отправить сообщение", "Ошибка", false, 3);
                message.ShowDialog();
                return false;
            }
        }

    }
}

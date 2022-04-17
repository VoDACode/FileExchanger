using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FileExchanger.Services
{
    public static class EmailService
    {
        public static Task SendAsyn(string to, string subject, string text)
        {
            return Task.Factory.StartNew(() => {
                using (SmtpClient client = Connect())
                using (MailMessage message = new MailMessage(Config.Instance.Email.Address, to, subject, text))
                    client.Send(message);
            });
        }
        public static void Send(string to, string subject, string text)
        {
            using (SmtpClient client = Connect())
            using (MailMessage message = new MailMessage(Config.Instance.Email.Address, to, subject, text))
                client.Send(message);
        }
        private static SmtpClient Connect()
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = Config.Instance.Email.Host;
            smtpClient.Port = Config.Instance.Email.Port;
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(Config.Instance.Email.Address, Config.Instance.Email.Password);
            return smtpClient;
        }
    }
}

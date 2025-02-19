using System.Net.Mail;
using System.Net;

namespace BookStore.Services.Mail
{
    public class MailService
    {
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly bool _enableSsl;

        public MailService(IConfiguration configuration)
        {
            _smtpServer = configuration["MailSettings:smtpServer"];
            _port = Convert.ToInt32(configuration["MailSettings:port"]);
            _username = configuration["MailSettings:username"]; ;
            _password = configuration["MailSettings:password"]; ;
            _enableSsl = true;
        }
        public async Task SendMailAsync(string content, string to, string header)
        {
            try
            {
                using (var client = new SmtpClient(_smtpServer, _port))
                {
                    client.EnableSsl = _enableSsl;
                    client.Credentials = new NetworkCredential(_username, _password);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_username),
                        Subject = header,
                        Body = content,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(to);

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email: {ex.Message}", ex);
            }
        }

        public void SendMail(string content, string to, string header)
        {
            SendMailAsync(content, to, header).GetAwaiter().GetResult();
        }

    }
}

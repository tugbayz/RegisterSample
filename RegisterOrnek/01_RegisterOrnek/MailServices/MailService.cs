using _01_RegisterOrnek.Settings;
using MailKit;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace _01_RegisterOrnek.MailServices
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendMailAsync(string email, string subject, string message)
        {
            try
            {
                var newEmail = new MimeMessage();
                newEmail.From.Add(MailboxAddress.Parse("bilgeadam2024@gmail.com"));
                newEmail.To.Add(MailboxAddress.Parse(email));
                newEmail.Subject = subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = message;
                newEmail.Body = builder.ToMessageBody();
                var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync("xxx4@gmail.com", "soubzozkdbkgyxpb");
                await smtp.SendAsync(newEmail);
                await smtp.DisconnectAsync(true);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("hata olusştu:" + ex.Message);
            }

        }
    }
    
}

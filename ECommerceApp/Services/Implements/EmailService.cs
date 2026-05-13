using System.Net.Mail;
using System.Net;
using ECommerceApp.Services.Interfaces;

namespace ECommerceApp.Services.Implements
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = false)
        {
            var mailServer = configuration["EmailSettings:MailServer"] ?? throw new Exception("EmailSettings:MailServer is not set in appsettings.json");
            var fromEmail = configuration["EmailSettings:FromEmail"] ?? throw new Exception("EmailSettings:FromEmail is not set in appsettings.json");
            var password = configuration["EmailSettings:Password"] ?? throw new Exception("EmailSettings:Password is not set in appsettings.json");
            var senderName = configuration["EmailSettings:SenderName"] ?? throw new Exception("EmailSettings:SenderName is not set in appsettings.json");
            var portString = configuration["EmailSettings:MailPort"] ?? throw new Exception("EmailSettings:MailPort is not set in appsettings.json");
            var port = int.Parse(portString);
            
            var client = new SmtpClient(mailServer, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true,
            };

            var fromAddress = new MailAddress(fromEmail, senderName);

            var mailMessage = new MailMessage
            {
                From = fromAddress,
                Subject = subject, 
                Body = body,
                IsBodyHtml = isBodyHtml
            };

            mailMessage.To.Add(toEmail);

            return client.SendMailAsync(mailMessage);
        }
    }
}
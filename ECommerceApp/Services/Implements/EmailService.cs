using System.Net.Mail;
using System.Net;
using ECommerceApp.Services.Interfaces;

namespace ECommerceApp.Services.Implements
{
    public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = false)
        {
            try
            {
                var mailServer = configuration["EmailSettings:MailServer"] ?? throw new Exception("EmailSettings:MailServer is not set in appsettings.json");
                var fromEmail = configuration["EmailSettings:FromEmail"] ?? throw new Exception("EmailSettings:FromEmail is not set in appsettings.json");
                var password = configuration["EmailSettings:Password"] ?? throw new Exception("EmailSettings:Password is not set in appsettings.json");
                var senderName = configuration["EmailSettings:SenderName"] ?? throw new Exception("EmailSettings:SenderName is not set in appsettings.json");
                var portString = configuration["EmailSettings:MailPort"] ?? throw new Exception("EmailSettings:MailPort is not set in appsettings.json");
                var port = int.Parse(portString);

                using var client = new SmtpClient(mailServer, port)
                {
                    Credentials = new NetworkCredential(fromEmail, password),
                    EnableSsl = true,
                };

                var fromAddress = new MailAddress(fromEmail, senderName);

                using var mailMessage = new MailMessage
                {
                    From = fromAddress,
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isBodyHtml
                };

                mailMessage.To.Add(toEmail);

                logger.LogInformation("Sending email to {ToEmail} with subject {Subject}.", toEmail, subject);
                await client.SendMailAsync(mailMessage);
                logger.LogInformation("Email sent to {ToEmail} with subject {Subject}.", toEmail, subject);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send email to {ToEmail} with subject {Subject}.", toEmail, subject);
                throw;
            }
        }
    }
}

namespace ECommerceApp.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = false);
}
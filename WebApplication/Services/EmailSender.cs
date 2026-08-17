using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace WebApplication.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // For development, just log the email instead of actually sending it
            // In production, replace this with actual email sending logic (e.g., SendGrid, SMTP, etc.)
            System.Console.WriteLine($"Email sent to: {email}");
            System.Console.WriteLine($"Subject: {subject}");
            System.Console.WriteLine($"Message: {htmlMessage}");
            
            return Task.CompletedTask;
        }
    }
}

using Microsoft.AspNetCore.Identity.UI.Services;

namespace Platform.Services.MailService;

public class EmailSender : IEmailSender
{

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Console.WriteLine($"Email sent to {email}");
        Console.WriteLine($"Subject - {subject}");
        Console.WriteLine($"Message - {htmlMessage}");
        return Task.CompletedTask;
    }
}
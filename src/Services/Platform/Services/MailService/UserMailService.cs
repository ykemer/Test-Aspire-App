using Microsoft.AspNetCore.Identity;

using Platform.Database.Entities;

namespace Platform.Services.MailService;

public class UserMailService : IEmailSender<ApplicationUser>
{
  public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
  {
    Console.WriteLine($"Confirmation link sent to {email}");
    Console.WriteLine($"Confirmation link - {confirmationLink}");
    return Task.CompletedTask;
  }

  public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
  {
    Console.WriteLine($"Reset link sent to {email}");
    Console.WriteLine($"Reset link - {resetLink}");
    return Task.CompletedTask;
  }

  public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
  {
    Console.WriteLine($"Reset code sent to {email}");
    Console.WriteLine($"Reset code - {resetCode}");
    return Task.CompletedTask;
  }
}

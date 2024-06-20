namespace Aspire_App.ApiService.Application.Auth.Commmands;

public class UserRegistrationCommand
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
    public string RepeatPassword { get; init; }
    public DateTime DateOfBirth { get; init; }
}
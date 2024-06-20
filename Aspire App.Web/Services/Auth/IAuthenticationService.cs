using Aspire_App.Web.Models.Auth;

namespace Aspire_App.Web.Services.Auth;

public interface IAuthenticationService
{
    ValueTask<string> GetJwtAsync();
    Task LogoutAsync();
    Task<DateTime> LoginAsync(LoginModel model);
    Task<bool> RefreshAsync();
    
    string GetUsername(string token);
    string GetUserRole(string token);
}
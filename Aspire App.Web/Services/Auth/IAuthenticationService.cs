using Aspire_App.Web.Contracts.Requests.Auth;
using Aspire_App.Web.Models.Auth;

namespace Aspire_App.Web.Services.Auth;

public interface IAuthenticationService
{
    ValueTask<string> GetJwtAsync();
    Task LogoutAsync();
    Task<DateTime> LoginAsync(LoginRequest request);
    Task<bool> RefreshAsync();

    string GetUsername(string token);
    string GetUserRole(string token);
}
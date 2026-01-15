using Contracts.Users.Requests;

namespace Aspire_App.Web.Services.Auth;

public interface IAuthenticationService
{
  ValueTask<string> GetJwtAsync();
  Task LogoutAsync();
  Task<DateTime> LoginAsync(UserLoginRequest request);
  Task RegisterAsync(UserRegisterRequest request);
  Task<bool> RefreshAsync();

  string GetUserId(string token);
  string GetUserRole(string token);
}

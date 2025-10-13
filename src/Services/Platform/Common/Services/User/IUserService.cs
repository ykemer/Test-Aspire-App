using System.Security.Claims;

namespace Platform.Common.Services.User;

public interface IUserService
{
  Guid GetUserId(ClaimsPrincipal user);
  string GetUserFirstName(ClaimsPrincipal user);
  string GetUserLastName(ClaimsPrincipal user);
  bool IsAdmin(ClaimsPrincipal user);
}

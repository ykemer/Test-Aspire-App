using System.Security.Claims;

namespace Platform.Common.Services.User;

public interface IUserService
{
  Guid GetUserId(ClaimsPrincipal user);
  bool IsAdmin(ClaimsPrincipal user);
}

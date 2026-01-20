using System.Security.Claims;

namespace Platform.Common.Services.User;

public class UserService : IUserService
{
  private const string AdministratorRole = "Administrator";


  public Guid GetUserId(ClaimsPrincipal user)
  {
    Guid.TryParse(user.FindFirstValue(ClaimTypes.Sid), out var id);
    return id;
  }

  public bool IsAdmin(ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.Role) == AdministratorRole;
}

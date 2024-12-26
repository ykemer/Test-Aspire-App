using System.Security.Claims;

namespace Aspire_App.ApiService.Services.User;

public class UserService : IUserService
{
    private const string AdministratorRole = "Administrator";

    public Guid GetUserId(ClaimsPrincipal user)
    {
        Guid.TryParse(user.FindFirstValue(ClaimTypes.Sid), out var id);
        return id;
    }

    public bool IsAdmin(ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Role) == AdministratorRole;
    }
}
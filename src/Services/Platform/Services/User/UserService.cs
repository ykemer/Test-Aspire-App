using System.Data;
using System.Security.Claims;

namespace Platform.Services.User;

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

    public string GetUserLastName(ClaimsPrincipal user)
    {
        return GetStringValueFromClaims(user, ClaimTypes.Surname);
    }

    public string GetUserFirstName(ClaimsPrincipal user)
    {
        return GetStringValueFromClaims(user, ClaimTypes.Name);
    }
    
    private string GetStringValueFromClaims(ClaimsPrincipal user, string claimType)
    {
        var value = user.FindFirstValue(claimType);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DataException($"User's {claimType} not found");
        }

        return value;
    }
}
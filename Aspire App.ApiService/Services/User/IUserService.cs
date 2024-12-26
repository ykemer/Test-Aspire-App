using System.Security.Claims;

namespace Aspire_App.ApiService.Services.User;

public interface IUserService
{
    Guid GetUserId(ClaimsPrincipal user);
    bool IsAdmin(ClaimsPrincipal user);
}
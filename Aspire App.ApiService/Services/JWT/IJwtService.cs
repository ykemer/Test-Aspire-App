using Aspire_App.ApiService.Features.Auth;

namespace Aspire_App.ApiService.Services.JWT;

public interface IJwtService
{
    Task<JwtTokenServiceResponse> GenerateJwtToken(ApplicationUser user);
}
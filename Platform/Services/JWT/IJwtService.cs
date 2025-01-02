using Platform.Features.Auth;

namespace Platform.Services.JWT;

public interface IJwtService
{
    Task<JwtTokenServiceResponse> GenerateJwtToken(ApplicationUser user);
}
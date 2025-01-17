using Platform.Entities;

namespace Platform.Services.JWT;

public interface IJwtService
{
  Task<string> GenerateToken(ApplicationUser user);
  Task<JwtTokenServiceResponse> GenerateJwtToken(ApplicationUser user);
}

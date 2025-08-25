using Platform.Database.Entities;

namespace Platform.Services.JWT;

public interface IJwtService
{
  Task<JwtTokenServiceResponse> GenerateJwtToken(ApplicationUser user);
}

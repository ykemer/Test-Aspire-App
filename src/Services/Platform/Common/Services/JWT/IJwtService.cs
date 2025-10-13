using Platform.Common.Database.Entities;

namespace Platform.Common.Services.JWT;

public interface IJwtService
{
  Task<JwtTokenServiceResponse> GenerateJwtToken(ApplicationUser user);
}

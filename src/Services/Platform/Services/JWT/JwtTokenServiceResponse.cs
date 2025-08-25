namespace Platform.Services.JWT;

public class JwtTokenServiceResponse
{
  public required string AccessToken { get; init; }
  public required long ExpiresIn { get; init; }
}

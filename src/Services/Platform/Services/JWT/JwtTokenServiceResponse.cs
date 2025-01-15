namespace Platform.Services.JWT;

public class JwtTokenServiceResponse
{
  public string AccessToken { get; init; }
  public long ExpiresIn { get; init; }
}

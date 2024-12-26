namespace Aspire_App.ApiService.Services.JWT;

public class JwtTokenServiceResponse
{
    public string AccessToken { get; init; }
    public long ExpiresIn { get; init; }
}
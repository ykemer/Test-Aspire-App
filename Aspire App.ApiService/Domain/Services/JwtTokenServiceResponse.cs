namespace Aspire_App.ApiService.Domain.Services;

public class JwtTokenServiceResponse
{
    public string AccessToken { get; init; }
    public long ExpiresIn { get; init; }
}
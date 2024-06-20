namespace Aspire_App.ApiService.Domain.Services;

public class JwtTokenServiceResponse
{
    public string AccessToken { get; set; }
    public long ExpiresIn { get; set; }
}
namespace Aspire_App.Web.Services.TokenServices;

public interface ITokenService
{
    Task<string> GetAccessTokenAsync();
    Task SetAccessTokenAsync(string token, TimeSpan? expiration = null);
    Task ClearAccessTokenAsync();


    Task<string?> GetRefreshTokenAsync();
    Task SetRefreshTokenAsync(string token, TimeSpan? expiration = null);
    Task ClearRefreshTokenAsync();
}
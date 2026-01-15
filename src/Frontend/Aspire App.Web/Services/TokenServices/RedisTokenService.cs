using Aspire_App.Web.Services.CookiesServices;

using Microsoft.Extensions.Caching.Distributed;

namespace Aspire_App.Web.Services.TokenServices;

public class RedisTokenService : ITokenService
{
  private readonly IDistributedCache _cache;
  private readonly ICookiesService _cookiesService;


  public RedisTokenService(IDistributedCache cache, ICookiesService cookiesService)
  {
    _cache = cache;
    _cookiesService = cookiesService;
  }

  public async Task<string> GetAccessTokenAsync()
  {
    var userId = GetUserId();
    var key = GetAccessTokenCacheKey(userId);
    return await _cache.GetStringAsync(key);
  }

  public async Task SetAccessTokenAsync(string token, TimeSpan? expiration = null)
  {
    var userId = GetUserId();
    var options = new DistributedCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow =
        expiration ?? TimeSpan.FromHours(1) // Default to 1 hour if no expiration is specified
    };

    await _cache.SetStringAsync(GetAccessTokenCacheKey(userId), token, options);
  }


  public async Task SetRefreshTokenAsync(string refreshToken, TimeSpan? expiration = null)
  {
    var userId = GetUserId();
    var options = new DistributedCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow =
        expiration ?? TimeSpan.FromDays(30) // Set according to your refresh token policy
    };
    var key = GetRefreshTokenCacheKey(userId);
    await _cache.SetStringAsync(key, refreshToken, options);
  }

  public async Task<string?> GetRefreshTokenAsync()
  {
    var userId = GetUserId();
    var tokenKey = GetRefreshTokenCacheKey(userId);
    var token = await _cache.GetStringAsync(tokenKey);
    return token;
  }

  public async Task ClearTokensAsync()
  {
    var userId = GetUserId();
    await Task.WhenAll(
      _cache.RemoveAsync(GetAccessTokenCacheKey(userId)),
      _cache.RemoveAsync(GetRefreshTokenCacheKey(userId))
    );
  }


  private string GetUserId() => _cookiesService.GetUserId();

  private string GetAccessTokenCacheKey(string userId) => $"AuthToken-{userId}";

  private string GetRefreshTokenCacheKey(string userId) => $"RefreshToken-{userId}";
}

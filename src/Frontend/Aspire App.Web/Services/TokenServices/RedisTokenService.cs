using Aspire_App.Web.Services.CookiesServices;

using Microsoft.Extensions.Caching.Distributed;

namespace Aspire_App.Web.Services.TokenServices;

public class RedisTokenService : ITokenService
{
  private readonly IDistributedCache _cache;
  private readonly ICookiesService _cookiesService;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public RedisTokenService(IDistributedCache cache, IHttpContextAccessor httpContextAccessor,
    ICookiesService cookiesService)
  {
    _cache = cache;
    _httpContextAccessor = httpContextAccessor;
    _cookiesService = cookiesService;
  }

  public async Task<string> GetAccessTokenAsync()
  {
    string? userId = GetUserId();
    return await _cache.GetStringAsync(GetAccessTokenCacheKey(userId));
  }

  public async Task SetAccessTokenAsync(string token, TimeSpan? expiration = null)
  {
    string? userId = GetUserId();
    DistributedCacheEntryOptions? options = new()
    {
      AbsoluteExpirationRelativeToNow =
        expiration ?? TimeSpan.FromHours(1) // Default to 1 hour if no expiration is specified
    };

    await _cache.SetStringAsync(GetAccessTokenCacheKey(userId), token, options);
  }

  public async Task ClearAccessTokenAsync()
  {
    string? userId = GetUserId();
    await _cache.RemoveAsync(GetAccessTokenCacheKey(userId));
  }


  public async Task SetRefreshTokenAsync(string refreshToken, TimeSpan? expiration = null)
  {
    string? userId = GetUserId();
    DistributedCacheEntryOptions? options = new()
    {
      AbsoluteExpirationRelativeToNow =
        expiration ?? TimeSpan.FromDays(30) // Set according to your refresh token policy
    };

    await _cache.SetStringAsync(GetRefreshTokenCacheKey(userId), refreshToken, options);
  }

  public async Task<string?> GetRefreshTokenAsync()
  {
    string? userId = GetUserId();
    string? tokenKey = GetRefreshTokenCacheKey(userId);
    string? token = await _cache.GetStringAsync(tokenKey);
    return token;
  }

  public async Task ClearRefreshTokenAsync()
  {
    string? userId = GetUserId();
    await _cache.RemoveAsync(GetRefreshTokenCacheKey(userId));
  }


  private string GetUserId() => _cookiesService.GetUserId();

  private string GetAccessTokenCacheKey(string userId) => $"AuthToken-{userId}";

  private string GetRefreshTokenCacheKey(string userId) => $"RefreshToken-{userId}";
}

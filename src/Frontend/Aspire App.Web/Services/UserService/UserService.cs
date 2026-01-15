using System.Text.Json;
using Aspire_App.Web.Helpers;
using Aspire_App.Web.Services.CookiesServices;

using Contracts.Users.Responses;

using Microsoft.Extensions.Caching.Distributed;

namespace Aspire_App.Web.Services.UserService;

public class UserService : IUserService
{
  private readonly HttpClient _httpClient;
  private readonly ICookiesService _cookiesService;
  private readonly IDistributedCache _cache;

  public UserService(HttpClient httpClient, IDistributedCache cache, ICookiesService cookiesService)
  {
    _httpClient = httpClient;
    _cache = cache;
    _cookiesService = cookiesService;
  }

  public async Task<UserInfoResponse> GetUserInfoAsync(CancellationToken cancellationToken = default)
  {
    var cachedUser = await GetUserData();
    if (cachedUser != null)
      return cachedUser;

    var response = await _httpClient.GetAsync($"api/auth/whoami",
      cancellationToken);
    var result =
      await FrontendHelper.ReadJsonOrThrowForErrors<UserInfoResponse>(response);

    if (result is null)
    {
      throw new UnauthorizedAccessException("User is not authorized.");
    }

    await SetUserNameAsync(result!, TimeSpan.FromHours(1));
    return result;

  }

  private async Task<UserInfoResponse?> GetUserData()
  {
    var userId = GetUserId();
    if (string.IsNullOrEmpty(userId))
      return null;

    var key = GetUserNameTokenCacheKey(userId);
    var data = await _cache.GetAsync(key);
    if (data == null || data.Length == 0)
      return null;

    try
    {
      var user = JsonSerializer.Deserialize<UserInfoResponse>(data);
      return user;
    }
    catch (JsonException)
    {
      await _cache.RemoveAsync(key);
      return null;
    }
  }

  private async Task SetUserNameAsync(UserInfoResponse userInfo, TimeSpan? expiration = null)
  {
    var userId = GetUserId();
    if (string.IsNullOrEmpty(userId))
      return;

    var options = new DistributedCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow =
        expiration ?? TimeSpan.FromHours(1) // Default to 1 hour if no expiration is specified
    };

    var key = GetUserNameTokenCacheKey(userId);
    var bytes = JsonSerializer.SerializeToUtf8Bytes(userInfo);
    await _cache.SetAsync(key, bytes, options);
  }

  private string GetUserId() => _cookiesService.GetUserId();

  private string GetUserNameTokenCacheKey(string userId) => $"UserName-{userId}";
}

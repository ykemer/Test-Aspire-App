using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Aspire_App.Web.Helpers;
using Aspire_App.Web.Services.TokenServices;

using Contracts.Auth.Responses;
using Contracts.Users.Requests;

namespace Aspire_App.Web.Services.Auth;

public class AuthenticationService : IAuthenticationService
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<AuthenticationService> _logger;
  private readonly ITokenService _tokenService;

  public AuthenticationService(ITokenService tokenService,
    ILogger<AuthenticationService> logger, HttpClient httpClient)
  {
    _tokenService = tokenService;
    _logger = logger;
    _httpClient = httpClient;
  }

  public async ValueTask<string> GetJwtAsync()
  {
    var token = await _tokenService.GetAccessTokenAsync();
    if (!string.IsNullOrEmpty(token))
    {
      return token;
    }

    var success = await TryRefreshTokenAsync();
    if (success)
    {
      return await _tokenService.GetAccessTokenAsync();
    }

    return string.Empty;
  }

  public async Task LogoutAsync()
  {
    var refreshToken = await _tokenService.GetRefreshTokenAsync();
    if (string.IsNullOrEmpty(refreshToken))
    {
      return;
    }

    var request = new RefreshAccessTokenRequest { RefreshToken = refreshToken };

    await _tokenService.ClearTokensAsync();

    try
    {
      await _httpClient
        .PostAsync("api/auth/revoke", JsonContent.Create(request));
    }
    catch (HttpRequestException e)
    {
      _logger.LogError(e, "Failed to revoke refresh token during logout.");
    }
  }

  public string GetUserId(string token) => GetClaimFromToken(token, ClaimTypes.Sid);

  public string GetUserRole(string token) => GetClaimFromToken(token, ClaimTypes.Role);


  public async Task<DateTime> LoginAsync(UserLoginRequest request)
  {
    var content = await GetApiResponseAsync("/api/auth/login", request);
    await SetTokensAsync(content);
    return content.ExpiresAt;
  }

  public async Task RegisterAsync(UserRegisterRequest request)
  {
    var content = await GetApiResponseAsync("/api/auth/register", request);
    await SetTokensAsync(content);
  }

  public async Task<bool> RefreshAsync()
  {
    try
    {
      var content = await GetApiResponseAsync("/api/auth/refresh",
        new RefreshAccessTokenRequest { RefreshToken = await _tokenService.GetRefreshTokenAsync() });

      await SetTokensAsync(content);

      return true;
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Token refresh failed");
      await _tokenService.ClearTokensAsync();
      return false;
    }
  }

  private async Task<bool> TryRefreshTokenAsync()
  {
    var refreshToken = await _tokenService.GetRefreshTokenAsync();
    if (string.IsNullOrEmpty(refreshToken))
    {
      return false;
    }

    return await RefreshAsync();
  }

  private static string GetClaimFromToken(string token, string claimType)
  {
    var jwt = new JwtSecurityToken(token);
    var claim = jwt.Claims.FirstOrDefault(c => c.Type == claimType);
    if (claim is null)
    {
      throw new InvalidOperationException($"The token does not contain the required '{claimType}' claim.");
    }

    return claim.Value;
  }

  private async Task<LoginResponse> GetApiResponseAsync(string url, object request)
  {
    var response = await _httpClient.PostAsJsonAsync(url, request);

    var content = await FrontendHelper.ReadJsonOrThrowForErrors<LoginResponse>(response, "Authentication failed");
    return content!;
  }

  private async Task SetTokensAsync(LoginResponse content)
  {
    await _tokenService.SetAccessTokenAsync(content.AccessToken, content.ExpiresAt - DateTime.UtcNow);
    await _tokenService.SetRefreshTokenAsync(content.RefreshToken);
  }
}

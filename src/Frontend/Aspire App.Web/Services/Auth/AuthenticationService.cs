using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Aspire_App.Web.Helpers;
using Aspire_App.Web.Services.TokenServices;

using Contracts.Auth.Responses;
using Contracts.Users.Requests;

namespace Aspire_App.Web.Services.Auth;

public class AuthenticationService : IAuthenticationService
{
  private readonly IHttpClientFactory _factory;
  private readonly ITokenService _tokenService;

  public AuthenticationService(IHttpClientFactory factory, ITokenService tokenService)
  {
    _factory = factory;
    _tokenService = tokenService;
  }

  public async ValueTask<string> GetJwtAsync()
  {
    string? token = await _tokenService.GetAccessTokenAsync();
    if (!string.IsNullOrEmpty(token))
    {
      return token;
    }

    bool success = await TryRefreshTokenAsync();
    if (success)
    {
      return await _tokenService.GetAccessTokenAsync();
    }

    return string.Empty;
  }

  public async Task LogoutAsync()
  {
    string? refreshToken = await _tokenService.GetRefreshTokenAsync();
    if (string.IsNullOrEmpty(refreshToken))
    {
      return;
    }

    RefreshAccessTokenRequest? request = new() { RefreshToken = refreshToken };

    await _tokenService.ClearRefreshTokenAsync();
    await _tokenService.ClearAccessTokenAsync();

    HttpResponseMessage? response = await _factory.CreateClient("ServerApi").PostAsync("api/auth/revoke",
      JsonContent.Create(request));


    await Console.Out.WriteLineAsync($"Revoke gave response {response.StatusCode}");
  }


  public string GetUsername(string token) => GetClaimFromToken(token, ClaimTypes.Name);

  public string GetUserRole(string token) => GetClaimFromToken(token, ClaimTypes.Role);

  public async Task<DateTime> LoginAsync(UserLoginRequest request)
  {
    LoginResponse? content = await GetApiResponseAsync("/api/auth/login", request);

    await SetTokensAsync(content);

    return content.ExpiresAt;
  }

  public async Task RegisterAsync(UserRegisterRequest request)
  {
    LoginResponse? content = await GetApiResponseAsync("/api/auth/register", request);
    await SetTokensAsync(content);
  }

  public async Task<bool> RefreshAsync()
  {
    LoginResponse? content = await GetApiResponseAsync("api/auth/refresh",
      new RefreshAccessTokenRequest { RefreshToken = await _tokenService.GetRefreshTokenAsync() });

    if (content == null)
    {
      await LogoutAsync();
      return false;
    }

    await SetTokensAsync(content);

    return true;
  }

  private async Task<bool> TryRefreshTokenAsync()
  {
    string? refreshToken = await _tokenService.GetRefreshTokenAsync();
    if (string.IsNullOrEmpty(refreshToken))
    {
      return false;
    }

    return await RefreshAsync();
  }

  private static string GetClaimFromToken(string token, string claimType)
  {
    JwtSecurityToken? jwt = new(token);
    return jwt.Claims.First(c => c.Type == claimType).Value;
  }

  private async Task<LoginResponse> GetApiResponseAsync(string url, object request)
  {
    HttpResponseMessage? response = await _factory.CreateClient("ServerApi").PostAsJsonAsync(url, request);

    if (response.IsSuccessStatusCode)
    {
      LoginResponse? content = await response.Content.ReadFromJsonAsync<LoginResponse>();
      if (content == null)
      {
        throw new InvalidDataException("Invalid response content.");
      }

      return content;
    }

    await FrontendHelper.ProcessValidationDetails(response);

    string? errorContent = await response.Content.ReadAsStringAsync();
    throw new ArgumentException($"Request failed with status code {response.StatusCode}: {errorContent}");
  }

  private async Task SetTokensAsync(LoginResponse content)
  {
    await _tokenService.SetAccessTokenAsync(content.AccessToken, content.ExpiresAt - DateTime.UtcNow);
    await _tokenService.SetRefreshTokenAsync(content.RefreshToken);
  }
}

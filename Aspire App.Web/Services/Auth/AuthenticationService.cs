using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Aspire_App.Web.Contracts.Requests.Auth;
using Aspire_App.Web.Handlers;
using Aspire_App.Web.Models.Auth;
using Aspire_App.Web.Services.TokenServices;

namespace Aspire_App.Web.Services.Auth;

public class AuthenticationService : IAuthenticationService
{
    private readonly ApiAuthenticationStateProvider _authenticationStateProvider;
    private readonly IHttpClientFactory _factory;
    private readonly ITokenService _tokenService;

    public AuthenticationService(IHttpClientFactory factory, ITokenService tokenService)
    {
        _factory = factory;
        _tokenService = tokenService;
    }

    public async ValueTask<string> GetJwtAsync()
    {
        var token = await _tokenService.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(token)) return token;

        var success = await RefreshAsync();
        if (success) return await _tokenService.GetAccessTokenAsync();

        return string.Empty;
    }

    public async Task LogoutAsync()
    {
        var response = await _factory.CreateClient("ServerApi").DeleteAsync("api/authentication/revoke");

        await _tokenService.ClearRefreshTokenAsync();
        await _tokenService.ClearAccessTokenAsync();

        await Console.Out.WriteLineAsync($"Revoke gave response {response.StatusCode}");
    }

    public string GetUsername(string token)
    {
        var jwt = new JwtSecurityToken(token);
        return jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value;
    }

    public string GetUserRole(string token)
    {
        var jwt = new JwtSecurityToken(token);
        return jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value;
    }

    public async Task<DateTime> LoginAsync(LoginRequest request)
    {
        var response = await _factory.CreateClient("ServerApi").PostAsJsonAsync("/api/auth/login", request);

        if (!response.IsSuccessStatusCode)
            throw new UnauthorizedAccessException("Login failed.");

        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (content == null)
            throw new InvalidDataException();


        await _tokenService.SetAccessTokenAsync(content.AccessToken, content.ExpiresAt - DateTime.UtcNow);
        await _tokenService.SetRefreshTokenAsync(content.RefreshToken);

        return content.ExpiresAt;
    }

    public async Task<bool> RefreshAsync()
    {
        var refreshToken = await _tokenService.GetRefreshTokenAsync();
        if (string.IsNullOrEmpty(refreshToken)) return false;

        var model = new RefreshAccessTokenRequest
        {
            RefreshToken = refreshToken
        };

        var response = await _factory.CreateClient("ServerApi").PostAsync("api/auth/refresh",
            JsonContent.Create(model));

        if (!response.IsSuccessStatusCode)
        {
            await LogoutAsync();

            return false;
        }

        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (content == null)
            throw new InvalidDataException();

        await _tokenService.SetAccessTokenAsync(content.AccessToken, content.ExpiresAt - DateTime.UtcNow);

        return true;
    }

    public event Action<string?>? LoginChange;
}
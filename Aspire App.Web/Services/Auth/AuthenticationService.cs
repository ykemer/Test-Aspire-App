using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Aspire_App.Web.Exceptions;
using Aspire_App.Web.Services.TokenServices;
using Contracts.Auth.Requests;
using Contracts.Auth.Responses;
using ProblemDetails = FastEndpoints.ProblemDetails;

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
        var token = await _tokenService.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(token)) return token;

        var success = await TryRefreshTokenAsync();
        if (success) return await _tokenService.GetAccessTokenAsync();

        return string.Empty;
    }

    public async Task LogoutAsync()
    {
        var refreshToken = await _tokenService.GetRefreshTokenAsync();
        if (string.IsNullOrEmpty(refreshToken)) return;

        var request = new RefreshAccessTokenRequest
        {
            RefreshToken = refreshToken
        };

        await _tokenService.ClearRefreshTokenAsync();
        await _tokenService.ClearAccessTokenAsync();

        var response = await _factory.CreateClient("ServerApi").PostAsync("api/auth/revoke",
            JsonContent.Create(request));


        await Console.Out.WriteLineAsync($"Revoke gave response {response.StatusCode}");
    }


    public string GetUsername(string token)
    {
        return GetClaimFromToken(token, ClaimTypes.Name);
    }

    public string GetUserRole(string token)
    {
        return GetClaimFromToken(token, ClaimTypes.Role);
    }

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
        var content = await GetApiResponseAsync("api/auth/refresh", new RefreshAccessTokenRequest
        {
            RefreshToken = await _tokenService.GetRefreshTokenAsync()
        });

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
        var refreshToken = await _tokenService.GetRefreshTokenAsync();
        if (string.IsNullOrEmpty(refreshToken)) return false;

        return await RefreshAsync();
    }

    private static string GetClaimFromToken(string token, string claimType)
    {
        var jwt = new JwtSecurityToken(token);
        return jwt.Claims.First(c => c.Type == claimType).Value;
    }

    private async Task<LoginResponse> GetApiResponseAsync(string url, object request)
    {
        var response = await _factory.CreateClient("ServerApi").PostAsJsonAsync(url, request);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (content == null)
                throw new InvalidDataException("Invalid response content.");

            return content;
        }
        
        if(response.Content.Headers.ContentType?.MediaType == "application/problem+json")
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(options);
            throw new ValidationException(problemDetails?.Errors.ToDictionary(e => e.Name, e => new [] {e.Reason}) ?? new Dictionary<string, string[]>());
        }


        var errorContent = await response.Content.ReadAsStringAsync();
        throw new Exception($"Request failed with status code {response.StatusCode}: {errorContent}");
    }

    private async Task SetTokensAsync(LoginResponse content)
    {
        await _tokenService.SetAccessTokenAsync(content.AccessToken, content.ExpiresAt - DateTime.UtcNow);
        await _tokenService.SetRefreshTokenAsync(content.RefreshToken);
    }
}
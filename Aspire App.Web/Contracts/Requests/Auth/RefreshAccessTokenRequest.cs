namespace Aspire_App.Web.Contracts.Requests.Auth;

public class RefreshAccessTokenRequest
{
    public required string? RefreshToken { get; set; }
}
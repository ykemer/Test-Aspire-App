namespace Contracts.Auth.Requests;

public class RefreshAccessTokenRequest
{
    public required string? RefreshToken { get; set; }
}
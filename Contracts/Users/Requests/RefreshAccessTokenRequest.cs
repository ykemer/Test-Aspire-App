namespace Contracts.Users.Requests;

public class RefreshAccessTokenRequest
{
    public required string? RefreshToken { get; set; }
}
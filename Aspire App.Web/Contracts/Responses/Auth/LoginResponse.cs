namespace Aspire_App.Web.Models.Auth;

public class LoginResponse
{
    private int _expiresIn;

    public required string TokenType { get; set; }
    public required string AccessToken { get; set; }

    public int ExpiresIn
    {
        get => _expiresIn;
        set
        {
            _expiresIn = value;
            ExpiresAt = DateTime.UtcNow.AddSeconds(_expiresIn);
        }
    }

    public required string RefreshToken { get; set; }

    public DateTime ExpiresAt { get; private set; }
}
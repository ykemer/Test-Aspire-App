using System.Security.Cryptography;
using System.Text;

namespace Platform.Common.Auth;

/// <summary>
/// Refresh tokens are stored only as a SHA-256 hash, like passwords.
/// If the database leaks, the stored values cannot be used to sign in.
/// (A fast hash is fine here: the tokens are long random values, not guessable passwords.)
/// </summary>
public static class RefreshTokenHasher
{
  public static string Hash(string refreshToken) =>
    Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
}

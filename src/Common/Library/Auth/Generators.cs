using System.Security.Cryptography;

namespace Library.Auth;

public static class Generators
{
  public static string GenerateToken(int length = 64)
  {
    var randomNumber = new byte[length];

    using var generator = RandomNumberGenerator.Create();

    generator.GetBytes(randomNumber);

    return Convert.ToBase64String(randomNumber);
  }
}

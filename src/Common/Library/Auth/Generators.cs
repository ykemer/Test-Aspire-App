using System.Security.Cryptography;

namespace Library.Auth;

public static class Generators
{
  public static string GenerateToken(int length = 64)
  {
    byte[] randomNumber = new byte[length];

    using RandomNumberGenerator generator = RandomNumberGenerator.Create();

    generator.GetBytes(randomNumber);

    return Convert.ToBase64String(randomNumber);
  }
}

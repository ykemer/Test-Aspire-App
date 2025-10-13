namespace Library.Validators;

public static class Validators
{
  public static bool IsValidGuid(string value) => Guid.TryParse(value, out _);
}

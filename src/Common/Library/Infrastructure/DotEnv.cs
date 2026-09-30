namespace Library.Infrastructure;

public static class DotEnv
{
  public static void Load(string filePath)
  {
    if (!File.Exists(filePath))
    {
      return;
    }

    foreach (var rawLine in File.ReadAllLines(filePath))
    {
      var line = rawLine.Trim();
      if (line.Length == 0 || line.StartsWith('#'))
      {
        continue;
      }

      // Split on the first '=' only: values such as base64 keys may contain '=' themselves.
      var separator = line.IndexOf('=');
      if (separator <= 0)
      {
        continue;
      }

      var key = line[..separator].Trim();
      var value = Unquote(line[(separator + 1)..].Trim());

      Environment.SetEnvironmentVariable(key, value);
    }
  }

  private static string Unquote(string value) =>
    value.Length >= 2 && (value[0] == '"' || value[0] == '\'') && value[^1] == value[0]
      ? value[1..^1]
      : value;
}

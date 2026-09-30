using Library.Infrastructure;

namespace Test.Library.Infrastructure;

[TestFixture]
public class DotEnvTests
{
  private string _filePath = string.Empty;
  private readonly List<string> _keys = [];

  [SetUp]
  public void SetUp() => _filePath = Path.GetTempFileName();

  [TearDown]
  public void TearDown()
  {
    File.Delete(_filePath);
    foreach (var key in _keys)
    {
      Environment.SetEnvironmentVariable(key, null);
    }
  }

  [TestCase("KEY=plain", "plain")]
  [TestCase("KEY=abc+/def==", "abc+/def==")]
  [TestCase("KEY='abc+/def='", "abc+/def=")]
  [TestCase("KEY=\"quoted value\"", "quoted value")]
  [TestCase("  KEY = spaced  ", "spaced")]
  public void Load_ParsesValue(string line, string expected)
  {
    var key = UniqueKey();
    File.WriteAllText(_filePath, line.Replace("KEY", key));

    DotEnv.Load(_filePath);

    Assert.That(Environment.GetEnvironmentVariable(key), Is.EqualTo(expected));
  }

  [Test]
  public void Load_SkipsCommentsAndBlankLines_AndHandlesBom()
  {
    var key = UniqueKey();
    File.WriteAllText(_filePath, $"# comment\n\n{key}=value\n", new System.Text.UTF8Encoding(true));

    DotEnv.Load(_filePath);

    Assert.That(Environment.GetEnvironmentVariable(key), Is.EqualTo("value"));
  }

  [Test]
  public void Load_MissingFile_DoesNothing() =>
    Assert.DoesNotThrow(() => DotEnv.Load(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())));

  private string UniqueKey()
  {
    var key = $"DOTENV_TEST_{Guid.NewGuid():N}";
    _keys.Add(key);
    return key;
  }
}

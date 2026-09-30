using Microsoft.Extensions.Time.Testing;

namespace Test.Enrollments.Setup;

/// <summary>
/// A fixed "now" for tests, so results never depend on when the tests run.
/// </summary>
public static class TestClock
{
  public static readonly DateTime Now = new(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc);

  public static FakeTimeProvider Create() => new(Now);
}

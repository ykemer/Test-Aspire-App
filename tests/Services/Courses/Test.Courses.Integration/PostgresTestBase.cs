using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;

using Service.Courses.Common.Database;

namespace Test.Courses.Integration;

/// <summary>
/// Base class for tests that run against real PostgreSQL. Every test starts with empty tables
/// and a clock fixed at <see cref="Now"/>.
/// </summary>
public abstract class PostgresTestBase
{
  protected static readonly DateTime Now = new(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc);

  protected ApplicationDbContext DbContext { get; private set; } = null!;
  protected FakeTimeProvider Clock { get; private set; } = null!;

  [SetUp]
  public async Task StartWithEmptyTables()
  {
    // A new clock for every test: NUnit reuses one instance of the test class for all its tests.
    Clock = new FakeTimeProvider(Now);
    DbContext = PostgresDatabase.CreateDbContext();
    await DbContext.Database.ExecuteSqlRawAsync("TRUNCATE \"Classes\", \"Courses\", \"InboxMessages\"");
  }

  [TearDown]
  public async Task DisposeDbContext() => await DbContext.DisposeAsync();
}

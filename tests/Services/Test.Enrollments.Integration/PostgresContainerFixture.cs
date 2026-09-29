using Microsoft.EntityFrameworkCore;

using Service.Enrollments.Common.Database;

using Testcontainers.PostgreSql;

namespace Test.Enrollments.Integration;

/// <summary>
/// Starts one real Postgres container for the whole test run, so tests in this project can prove
/// behavior (like the xmin-based concurrency token) that EF Core's InMemory provider can't simulate.
/// </summary>
[SetUpFixture]
public class PostgresContainerFixture
{
  private static PostgreSqlContainer? _container;

  private static string ConnectionString =>
    _container?.GetConnectionString()
    ?? throw new InvalidOperationException("The Postgres test container has not been started yet.");

  [OneTimeSetUp]
  public async Task StartContainerAsync()
  {
    _container = new PostgreSqlBuilder().Build();
    await _container.StartAsync();

    await using var dbContext = CreateDbContext();
    await dbContext.Database.MigrateAsync();
  }

  [OneTimeTearDown]
  public async Task StopContainerAsync()
  {
    if (_container is not null)
    {
      await _container.DisposeAsync();
    }
  }

  public static ApplicationDbContext CreateDbContext()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseNpgsql(ConnectionString)
      .Options;

    return new ApplicationDbContext(options);
  }
}

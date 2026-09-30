using Microsoft.EntityFrameworkCore;

using Platform.Common.Database;

using Testcontainers.PostgreSql;

namespace Test.Platform.Integration;

/// <summary>
/// Starts one real PostgreSQL server in Docker for all tests in this project.
/// Each test class decides how far to migrate (usually: all the way).
/// </summary>
[SetUpFixture]
public class PostgresDatabase
{
  private static PostgreSqlContainer? s_container;

  public static string ConnectionString =>
    s_container?.GetConnectionString() ?? throw new InvalidOperationException("PostgreSQL container is not started.");

  public static ApplicationDbContext CreateDbContext(string? connectionString = null) =>
    new(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(connectionString ?? ConnectionString).Options);

  [OneTimeSetUp]
  public async Task StartDatabase()
  {
    s_container = new PostgreSqlBuilder("postgres:17-alpine").Build();
    await s_container.StartAsync();
  }

  [OneTimeTearDown]
  public async Task StopDatabase()
  {
    if (s_container is not null)
    {
      await s_container.DisposeAsync();
    }
  }
}

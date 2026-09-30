using Microsoft.EntityFrameworkCore;

using Service.Courses.Common.Database;

using Testcontainers.PostgreSql;

namespace Test.Courses.Integration;

/// <summary>
/// Starts one real PostgreSQL server in Docker for all tests in this namespace, and applies the migrations.
/// Needed for things the in-memory database cannot do: atomic SQL updates, transactions,
/// unique indexes, full-text search.
/// </summary>
[SetUpFixture]
public class PostgresDatabase
{
  private static PostgreSqlContainer? s_container;

  public static ApplicationDbContext CreateDbContext()
  {
    var connectionString = s_container?.GetConnectionString()
                           ?? throw new InvalidOperationException("PostgreSQL container is not started.");

    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseNpgsql(connectionString)
      .Options;

    return new ApplicationDbContext(options);
  }

  [OneTimeSetUp]
  public async Task StartDatabase()
  {
    s_container = new PostgreSqlBuilder("postgres:17-alpine").Build();
    await s_container.StartAsync();

    await using var dbContext = CreateDbContext();
    await dbContext.Database.MigrateAsync();
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

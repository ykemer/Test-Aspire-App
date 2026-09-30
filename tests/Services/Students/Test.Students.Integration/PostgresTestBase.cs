using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;

using Service.Students.Common.Database;
using Service.Students.Common.Database.Entities;

namespace Test.Students.Integration;

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
    await DbContext.Database.ExecuteSqlRawAsync("TRUNCATE \"Students\", \"InboxMessages\"");
  }

  [TearDown]
  public async Task DisposeDbContext() => await DbContext.DisposeAsync();

  protected async Task<Student> AddStudent(int enrollmentsCount = 0, string? email = null)
  {
    var student = new Student
    {
      FirstName = "Jane",
      LastName = "Doe",
      Email = email ?? $"{Guid.NewGuid()}@example.com",
      DateOfBirth = new DateTime(2000, 1, 1),
      EnrollmentsCount = enrollmentsCount
    };
    DbContext.Students.Add(student);
    await DbContext.SaveChangesAsync();
    DbContext.ChangeTracker.Clear();
    return student;
  }

  /// <summary>Reads the enrollments count with a fresh connection (null when the student does not exist).</summary>
  protected static async Task<int?> ReadEnrollmentsCount(Guid studentId)
  {
    await using var freshContext = PostgresDatabase.CreateDbContext();
    return await freshContext.Students.Where(s => s.Id == studentId)
      .Select(s => (int?)s.EnrollmentsCount).SingleOrDefaultAsync();
  }
}

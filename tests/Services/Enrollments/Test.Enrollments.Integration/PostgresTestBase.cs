using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Entities;

namespace Test.Enrollments.Integration;

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
    await DbContext.Database.ExecuteSqlRawAsync(
      "TRUNCATE \"Enrollments\", \"Classes\", \"IdempotencyRecords\"");
  }

  [TearDown]
  public async Task DisposeDbContext() => await DbContext.DisposeAsync();

  /// <summary>Saves a class whose registration is open (deadline tomorrow, starts in two days).</summary>
  protected async Task<Class> AddOpenClass(int maxStudents = 10)
  {
    var courseClass = new Class
    {
      CourseId = Guid.NewGuid(),
      MaxStudents = maxStudents,
      RegistrationDeadline = Now.AddDays(1),
      CourseStartDate = Now.AddDays(2),
      CourseEndDate = Now.AddDays(3)
    };
    DbContext.Classes.Add(courseClass);
    await DbContext.SaveChangesAsync();
    DbContext.ChangeTracker.Clear();
    return courseClass;
  }

  /// <summary>Reads the enrolled count and the number of enrollment rows with a fresh connection.</summary>
  protected static async Task<(int EnrolledCount, int EnrollmentRows)> ReadSeats(Guid classId)
  {
    await using var freshContext = PostgresDatabase.CreateDbContext();
    var enrolledCount = await freshContext.Classes.Where(c => c.Id == classId)
      .Select(c => c.EnrolledCount).SingleAsync();
    var enrollmentRows = await freshContext.Enrollments.CountAsync(e => e.ClassId == classId);
    return (enrolledCount, enrollmentRows);
  }
}

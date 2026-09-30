using Microsoft.EntityFrameworkCore;

using Service.Enrollments.Common.Database;

namespace Test.Enrollments.Setup;

public static class ApplicationDbContextCreator
{
  /// <summary>
  /// Creates an empty in-memory database. Every call gets its own database, so tests never see each other's data.
  /// When the code needs real PostgreSQL features, write the test in the Test.Enrollments.Integration project instead.
  /// </summary>
  public static ApplicationDbContext GetDbContext()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(Guid.NewGuid().ToString())
      .Options;

    return new ApplicationDbContext(options);
  }
}

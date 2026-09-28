using Microsoft.EntityFrameworkCore;

using Service.Enrollments.Common.Database;

namespace Test.Enrollments.Setup;

public static class ApplicationDbContextCreator
{
  public static ApplicationDbContext GetDbContext()
  {
    var dbContext = GetAdditionalDbContext("TestDatabase");
    dbContext.Classes.RemoveRange(dbContext.Classes);
    dbContext.Enrollments.RemoveRange(dbContext.Enrollments);
    dbContext.IdempotencyRecords.RemoveRange(dbContext.IdempotencyRecords);
    dbContext.SaveChanges();
    return dbContext;
  }

  /// <summary>
  /// Returns a second context pointed at the same InMemory database, so tests can observe changes
  /// committed by another context (simulating two concurrent requests) without wiping existing data.
  /// </summary>
  public static ApplicationDbContext GetAdditionalDbContext(string databaseName = "TestDatabase")
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(databaseName)
      .Options;

    return new ApplicationDbContext(options);
  }
}

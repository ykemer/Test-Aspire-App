using Microsoft.EntityFrameworkCore;

using Service.Enrollments.Common.Database;

namespace Test.Enrollments.Setup;

public static class ApplicationDbContextCreator
{
  public static ApplicationDbContext GetDbContext()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase("TestDatabase")
      .Options;

    var dbContext = new ApplicationDbContext(options);
    dbContext.Classes.RemoveRange(dbContext.Classes);
    dbContext.Enrollments.RemoveRange(dbContext.Enrollments);
    dbContext.SaveChanges();
    return dbContext;
  }
}

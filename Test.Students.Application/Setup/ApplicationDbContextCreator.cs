using Microsoft.EntityFrameworkCore;

using Service.Students.Database;

namespace Test.Students.Application.Setup;

public static class ApplicationDbContextCreator
{
  public static ApplicationDbContext GetDbContext()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase("TestDatabase")
      .Options;

    var dbContext = new ApplicationDbContext(options);
    dbContext.Students.RemoveRange(dbContext.Students);
    dbContext.SaveChanges();
    return dbContext;
  }
}

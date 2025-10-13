using Microsoft.EntityFrameworkCore;

using Service.Courses.Common.Database;

namespace Courses.Application.Setup;

public static class ApplicationDbContextCreator
{
  public static ApplicationDbContext GetDbContext()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase("TestDatabase")
      .Options;

    var dbContext = new ApplicationDbContext(options);

    return dbContext;
  }
}

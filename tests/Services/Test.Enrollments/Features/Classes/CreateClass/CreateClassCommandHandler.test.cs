using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Entities;
using Service.Enrollments.Features.Classes.CreateClass;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Classes.CreateClass;

[TestFixture]
public class CreateClassCommandHandlerTests
{
  private ApplicationDbContext _dbContext;
  private CreateClassCommandHandler _handler;
  private ILogger<CreateClassCommandHandler> _logger;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _logger = Substitute.For<ILogger<CreateClassCommandHandler>>();
    _handler = new CreateClassCommandHandler(_dbContext, _logger);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenClassAlreadyExists()
  {
    var existing = Builder<Class>.CreateNew().Build();
    await _dbContext.Classes.AddAsync(existing);
    await _dbContext.SaveChangesAsync();

    var cmd = new CreateClassCommand
    {
      Id = existing.Id,
      CourseId = existing.CourseId,
      MaxStudents = existing.MaxStudents,
      RegistrationDeadline = existing.RegistrationDeadline,
      CourseStartDate = existing.CourseStartDate,
      CourseEndDate = existing.CourseEndDate
    };

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("class_service.create_class.already_exists"));
  }

  [Test]
  public async Task Handle_ShouldCreateClass_WhenNotExists()
  {
    var cmd = Builder<CreateClassCommand>.CreateNew()
      .With(c => c.RegistrationDeadline, DateTime.UtcNow.AddDays(1))
      .With(c => c.CourseStartDate, DateTime.UtcNow.AddDays(2))
      .With(c => c.CourseEndDate, DateTime.UtcNow.AddDays(3))
      .Build();

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(await _dbContext.Classes.FindAsync(cmd.Id), Is.Not.Null);
  }
}

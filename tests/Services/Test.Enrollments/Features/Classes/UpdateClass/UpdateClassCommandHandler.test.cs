using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Entities;
using Service.Enrollments.Features.Classes.CreateClass;
using Service.Enrollments.Features.Classes.UpdateClass;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Classes.UpdateClass;

[TestFixture]
public class UpdateClassCommandHandlerTests
{
  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _logger = Substitute.For<ILogger<CreateClassCommandHandler>>();
    _handler = new UpdateClassCommandHandler(_dbContext, _logger);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  private ApplicationDbContext _dbContext;
  private UpdateClassCommandHandler _handler;
  private ILogger<CreateClassCommandHandler> _logger;

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenClassDoesNotExist()
  {
    var cmd = Builder<UpdateClassCommand>.CreateNew().Build();

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("class_service.update_class.not_found"));
  }

  [Test]
  public async Task Handle_ShouldUpdateClass_WhenExists()
  {
    var existing = Builder<Class>.CreateNew()
      .With(c => c.MaxStudents, 10)
      .With(c => c.RegistrationDeadline, DateTime.UtcNow.AddDays(1))
      .With(c => c.CourseStartDate, DateTime.UtcNow.AddDays(2))
      .With(c => c.CourseEndDate, DateTime.UtcNow.AddDays(3))
      .Build();
    await _dbContext.Classes.AddAsync(existing);
    await _dbContext.SaveChangesAsync();

    var cmd = new UpdateClassCommand
    {
      Id = existing.Id,
      CourseId = existing.CourseId,
      MaxStudents = 25,
      RegistrationDeadline = existing.RegistrationDeadline.AddDays(1),
      CourseStartDate = existing.CourseStartDate.AddDays(1),
      CourseEndDate = existing.CourseEndDate.AddDays(1)
    };

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    var updated = await _dbContext.Classes.FindAsync(existing.Id);
    Assert.That(updated!.MaxStudents, Is.EqualTo(25));
    Assert.That(updated.RegistrationDeadline, Is.EqualTo(cmd.RegistrationDeadline).Within(TimeSpan.FromSeconds(1)));
    Assert.That(updated.CourseStartDate, Is.EqualTo(cmd.CourseStartDate).Within(TimeSpan.FromSeconds(1)));
    Assert.That(updated.CourseEndDate, Is.EqualTo(cmd.CourseEndDate).Within(TimeSpan.FromSeconds(1)));
  }
}

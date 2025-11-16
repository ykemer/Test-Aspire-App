using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Entities;
using Service.Enrollments.Features.Classes.CourseDeleted;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Classes.CourseDeleted;

public class DeleteClassesByCourseIdCommandHandlerTest
{
  private ApplicationDbContext _dbContext;
  private DeleteClassesByCourseIdCommandHandler _handler;
  private ILogger<DeleteClassesByCourseIdCommandHandler> _logger;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _logger = Substitute.For<ILogger<DeleteClassesByCourseIdCommandHandler>>();
    _handler = new DeleteClassesByCourseIdCommandHandler(_dbContext, _logger);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnDeletedWhenNoClassesWithThisCourseIdExist()
  {
    var request = new DeleteClassesByCourseIdCommand(Guid.NewGuid().ToString());

    var result = await _handler.Handle(request, CancellationToken.None);

    Assert.That(result.IsError, Is.False);

  }

  [Test]
  public async Task Handle_ShouldReturnErrorWhenExistingEnrollmentsExist()
  {
    var existingCourse = Builder<Class>.CreateNew().Build();
    var existingEnrollment = Builder<Enrollment>.CreateNew()
      .With(e => e.CourseId = existingCourse.CourseId)
      .With(e => e.ClassId = existingCourse.Id)
      .Build();

    await _dbContext.Classes.AddAsync(existingCourse);
    await _dbContext.Enrollments.AddAsync(existingEnrollment);
    ;
    await _dbContext.SaveChangesAsync();

    var request = new DeleteClassesByCourseIdCommand(existingCourse.CourseId);
    var result = await _handler.Handle(request, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("General.Conflict"));
    Assert.That(result.FirstError.Description,
      Is.EqualTo(
        $"Classes for course with ID {request.CourseId} can not be deleted because of existing subscriptions"));
  }

  [Test]
  public async Task Handle_ShouldDeleteClassesWhenNoEnrollmentsExist()
  {
    var existingCourse = Builder<Class>.CreateNew().Build();
    await _dbContext.Classes.AddAsync(existingCourse);
    await _dbContext.SaveChangesAsync();

    var request = new DeleteClassesByCourseIdCommand(existingCourse.CourseId);
    var result = await _handler.Handle(request, CancellationToken.None);
    Assert.That(result.IsError, Is.False);
    Assert.That(await _dbContext.Classes.FindAsync(existingCourse.CourseId), Is.Null);
  }
}

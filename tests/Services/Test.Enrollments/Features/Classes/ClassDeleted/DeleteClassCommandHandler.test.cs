using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Entities;
using Service.Enrollments.Features.Classes.ClassDeleted;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Classes.ClassDeleted;

public class DeleteClassByClassIdCommandHandlerTest
{
  private ApplicationDbContext _dbContext;
  private DeleteClassByClassIdCommandHandler _handler;
  private ILogger<DeleteClassByClassIdCommandHandler> _logger;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _logger = Substitute.For<ILogger<DeleteClassByClassIdCommandHandler>>();
    _handler = new DeleteClassByClassIdCommandHandler(_dbContext, _logger);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();


  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenClassDoesNotExist()
  {
    var request = new DeleteClassByClassIdCommand(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

    var result = await _handler.Handle(request, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("General.NotFound"));
    Assert.That(result.FirstError.Description, Is.EqualTo($"Class with ID {request.ClassId} not found"));
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenEnrollmentsExist()
  {
    var existingClass = Builder<Class>
      .CreateNew()
      .With(c => c.Id, Guid.NewGuid().ToString())
      .With(c => c.CourseId, Guid.NewGuid().ToString())
      .Build();

    var existingEnrollments = Builder<Enrollment>
      .CreateNew()
      .With(e => e.ClassId, existingClass.Id)
      .With(e => e.CourseId, existingClass.CourseId)
      .Build();


    await _dbContext.Classes.AddAsync(existingClass);
    await _dbContext.Enrollments.AddAsync(existingEnrollments);
    await _dbContext.SaveChangesAsync();

    var request = new DeleteClassByClassIdCommand(existingClass.CourseId, existingClass.Id);

    var result = await _handler.Handle(request, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("General.Conflict"));
    Assert.That(result.FirstError.Description,
      Is.EqualTo("Class with ID {ClassId} can not be deleted because of existing subscriptions"));
  }

  [Test]
  public async Task Handle_ShouldDeleteClass_WhenNoEnrollmentsExist()
  {
    var existingClass = Builder<Class>
      .CreateNew()
      .With(c => c.Id, Guid.NewGuid().ToString())
      .With(c => c.CourseId, Guid.NewGuid().ToString())
      .Build();


    await _dbContext.Classes.AddAsync(existingClass);

    await _dbContext.SaveChangesAsync();

    var request = new DeleteClassByClassIdCommand(existingClass.CourseId, existingClass.Id);
    var result = await _handler.Handle(request, CancellationToken.None);
    Assert.That(result.IsError, Is.False);
    Assert.That(await _dbContext.Classes.FindAsync(existingClass.Id), Is.Null);
  }
}

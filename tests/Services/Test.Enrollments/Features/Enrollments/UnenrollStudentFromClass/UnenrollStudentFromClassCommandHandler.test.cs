using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Entities;
using Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

[TestFixture]
public class UnenrollStudentFromClassCommandHandlerTests
{
  private ApplicationDbContext _dbContext;
  private UnenrollStudentFromClassCommandHandler _handler;
  private ILogger<UnenrollStudentFromClassCommandHandler> _logger;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _logger = Substitute.For<ILogger<UnenrollStudentFromClassCommandHandler>>();
    _handler = new UnenrollStudentFromClassCommandHandler(_logger, _dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenClassDoesNotExist()
  {
    var cmd = Builder<UnenrollStudentFromClassCommand>.CreateNew().Build();

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("enrollment_service.unenroll_student_from_course.class_not_found"));
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenEnrollmentDoesNotExist()
  {
    var cls = Builder<Class>.CreateNew()
      .With(c => c.RegistrationDeadline, DateTime.UtcNow.AddDays(1))
      .With(c => c.CourseStartDate, DateTime.UtcNow.AddDays(5))
      .With(c => c.CourseEndDate, DateTime.UtcNow.AddDays(6))
      .Build();
    await _dbContext.Classes.AddAsync(cls);
    await _dbContext.SaveChangesAsync();

    var cmd = new UnenrollStudentFromClassCommand
    {
      CourseId = cls.CourseId,
      ClassId = cls.Id,
      StudentId = "student-1"
    };

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("enrollment_service.unenroll_student_from_course.not_enrolled"));
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenClassAlreadyStarted()
  {
    var cls = Builder<Class>.CreateNew()
      .With(c => c.CourseStartDate, DateTime.UtcNow.AddMinutes(-1))
      .With(c => c.CourseEndDate, DateTime.UtcNow.AddDays(5))
      .Build();
    await _dbContext.Classes.AddAsync(cls);

    var enrollment = Builder<Enrollment>.CreateNew()
      .With(e => e.CourseId, cls.CourseId)
      .With(e => e.ClassId, cls.Id)
      .With(e => e.StudentId, "student-1")
      .With(e => e.StudentFirstName, "John")
      .With(e => e.StudentLastName, "Doe")
      .Build();
    await _dbContext.Enrollments.AddAsync(enrollment);
    await _dbContext.SaveChangesAsync();

    var cmd = new UnenrollStudentFromClassCommand
    {
      CourseId = cls.CourseId,
      ClassId = cls.Id,
      StudentId = "student-1"
    };

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("enrollment_service.unenroll_student_from_course.class_already_started"));
  }

  [Test]
  public async Task Handle_ShouldDeleteEnrollment_WhenValid()
  {
    var cls = Builder<Class>.CreateNew()
      .With(c => c.CourseStartDate, DateTime.UtcNow.AddDays(2))
      .With(c => c.CourseEndDate, DateTime.UtcNow.AddDays(5))
      .Build();
    await _dbContext.Classes.AddAsync(cls);

    var enrollment = Builder<Enrollment>.CreateNew()
      .With(e => e.CourseId, cls.CourseId)
      .With(e => e.ClassId, cls.Id)
      .With(e => e.StudentId, "student-1")
      .With(e => e.StudentFirstName, "John")
      .With(e => e.StudentLastName, "Doe")
      .Build();
    await _dbContext.Enrollments.AddAsync(enrollment);
    await _dbContext.SaveChangesAsync();

    var cmd = new UnenrollStudentFromClassCommand
    {
      CourseId = cls.CourseId,
      ClassId = cls.Id,
      StudentId = "student-1"
    };

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(await _dbContext.Enrollments.CountAsync(), Is.EqualTo(0));
  }
}

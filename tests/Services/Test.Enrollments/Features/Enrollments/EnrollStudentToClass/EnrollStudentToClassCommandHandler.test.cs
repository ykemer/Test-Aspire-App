using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Enrollments.Database;
using Service.Enrollments.Database.Entities;
using Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Enrollments.EnrollStudentToClass;

[TestFixture]
public class EnrollStudentToClassCommandHandlerTests
{
  private ApplicationDbContext _dbContext;
  private EnrollStudentToClassCommandHandler _handler;
  private ILogger<EnrollStudentToClassCommandHandler> _logger;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _logger = Substitute.For<ILogger<EnrollStudentToClassCommandHandler>>();
    _handler = new EnrollStudentToClassCommandHandler(_logger, _dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenAlreadyEnrolled()
  {
    var cls = Builder<Class>.CreateNew()
      .With(c => c.MaxStudents, 2)
      .With(c => c.RegistrationDeadline, DateTime.UtcNow.AddDays(1))
      .With(c => c.CourseStartDate, DateTime.UtcNow.AddDays(2))
      .With(c => c.CourseEndDate, DateTime.UtcNow.AddDays(3))
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

    var cmd = new EnrollStudentToClassCommand
    {
      CourseId = cls.CourseId,
      ClassId = cls.Id,
      StudentId = "student-1",
      FirstName = "John",
      LastName = "Doe"
    };

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("enrollment_service.enroll_student_to_course.already_enrolled"));
  }

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenClassDoesNotExist()
  {
    var cmd = Builder<EnrollStudentToClassCommand>.CreateNew().Build();

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("enrollment_service.enroll_student_to_course.class_not_found"));
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenRegistrationDeadlinePassed()
  {
    var cls = Builder<Class>.CreateNew()
      .With(c => c.RegistrationDeadline, DateTime.UtcNow.AddDays(-1))
      .With(c => c.CourseStartDate, DateTime.UtcNow.AddDays(1))
      .With(c => c.CourseEndDate, DateTime.UtcNow.AddDays(2))
      .With(c => c.MaxStudents, 10)
      .Build();
    await _dbContext.Classes.AddAsync(cls);
    await _dbContext.SaveChangesAsync();

    var cmd = new EnrollStudentToClassCommand
    {
      CourseId = cls.CourseId,
      ClassId = cls.Id,
      StudentId = "student-1",
      FirstName = "John",
      LastName = "Doe"
    };

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("enrollment_service.enroll_student_to_course.registration_deadline_passed"));
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenClassIsFull()
  {
    var cls = Builder<Class>.CreateNew()
      .With(c => c.RegistrationDeadline, DateTime.UtcNow.AddDays(1))
      .With(c => c.CourseStartDate, DateTime.UtcNow.AddDays(2))
      .With(c => c.CourseEndDate, DateTime.UtcNow.AddDays(3))
      .With(c => c.MaxStudents, 1)
      .Build();
    await _dbContext.Classes.AddAsync(cls);

    // Fill class with one enrollment
    var enrollment = Builder<Enrollment>.CreateNew()
      .With(e => e.CourseId, cls.CourseId)
      .With(e => e.ClassId, cls.Id)
      .With(e => e.StudentId, "student-1")
      .Build();
    await _dbContext.Enrollments.AddAsync(enrollment);
    await _dbContext.SaveChangesAsync();

    var cmd = new EnrollStudentToClassCommand
    {
      CourseId = cls.CourseId,
      ClassId = cls.Id,
      StudentId = "student-2",
      FirstName = "Jane",
      LastName = "Doe"
    };

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("enrollment_service.enroll_student_to_course.class_full"));
  }

  [Test]
  public async Task Handle_ShouldCreateEnrollment_WhenValid()
  {
    var cls = Builder<Class>.CreateNew()
      .With(c => c.RegistrationDeadline, DateTime.UtcNow.AddDays(1))
      .With(c => c.CourseStartDate, DateTime.UtcNow.AddDays(2))
      .With(c => c.CourseEndDate, DateTime.UtcNow.AddDays(3))
      .With(c => c.MaxStudents, 2)
      .Build();
    await _dbContext.Classes.AddAsync(cls);
    await _dbContext.SaveChangesAsync();

    var cmd = new EnrollStudentToClassCommand
    {
      CourseId = cls.CourseId,
      ClassId = cls.Id,
      StudentId = "student-1",
      FirstName = "John",
      LastName = "Doe"
    };

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(await _dbContext.Enrollments.CountAsync(), Is.EqualTo(1));
  }
}

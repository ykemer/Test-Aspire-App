using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Entities;
using Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Enrollments.EnrollStudentToClass;

[TestFixture]
public class EnrollStudentToClassCommandHandlerTests
{
  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _logger = Substitute.For<ILogger<EnrollStudentToClassCommandHandler>>();
    _handler = new EnrollStudentToClassCommandHandler(_logger, _dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  private ApplicationDbContext _dbContext;
  private EnrollStudentToClassCommandHandler _handler;
  private ILogger<EnrollStudentToClassCommandHandler> _logger;

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenAlreadyEnrolled()
  {
    var studentId = Guid.NewGuid();
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
      .With(e => e.StudentId, studentId)
      .With(e => e.StudentFirstName, "John")
      .With(e => e.StudentLastName, "Doe")
      .Build();

    await _dbContext.Enrollments.AddAsync(enrollment);
    await _dbContext.SaveChangesAsync();

    var cmd = new EnrollStudentToClassCommand
    {
      CourseId = cls.CourseId,
      ClassId = cls.Id,
      StudentId = studentId,
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
    var studentId = Guid.NewGuid();
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
      StudentId = studentId,
      FirstName = "John",
      LastName = "Doe"
    };

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code,
      Is.EqualTo("enrollment_service.enroll_student_to_course.registration_deadline_passed"));
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenClassIsFull()
  {
    var student1Id = Guid.NewGuid();
    var student2Id = Guid.NewGuid();
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
      .With(e => e.StudentId, student1Id)
      .Build();
    await _dbContext.Enrollments.AddAsync(enrollment);
    await _dbContext.SaveChangesAsync();

    var cmd = new EnrollStudentToClassCommand
    {
      CourseId = cls.CourseId,
      ClassId = cls.Id,
      StudentId = student2Id,
      FirstName = "Jane",
      LastName = "Doe"
    };

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("enrollment_service.enroll_student_to_course.class_full"));
  }

  [Test]
  public async Task Handle_ShouldEnrollStudentSuccessfully()
  {
    var studentId = Guid.NewGuid();
    var cls = Builder<Class>.CreateNew()
      .With(c => c.MaxStudents, 2)
      .With(c => c.RegistrationDeadline, DateTime.UtcNow.AddDays(1))
      .With(c => c.CourseStartDate, DateTime.UtcNow.AddDays(2))
      .With(c => c.CourseEndDate, DateTime.UtcNow.AddDays(3))
      .Build();
    await _dbContext.Classes.AddAsync(cls);
    await _dbContext.SaveChangesAsync();

    var cmd = new EnrollStudentToClassCommand
    {
      CourseId = cls.CourseId,
      ClassId = cls.Id,
      StudentId = studentId,
      FirstName = "John",
      LastName = "Doe"
    };

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.False);

    var enrollment = await _dbContext.Enrollments.FirstOrDefaultAsync(e =>
      e.StudentId == studentId && e.ClassId == cls.Id);

    Assert.That(enrollment, Is.Not.Null);
    Assert.That(enrollment.StudentId, Is.EqualTo(studentId));
    Assert.That(enrollment.CourseId, Is.EqualTo(cls.CourseId));
    Assert.That(enrollment.ClassId, Is.EqualTo(cls.Id));
  }
}

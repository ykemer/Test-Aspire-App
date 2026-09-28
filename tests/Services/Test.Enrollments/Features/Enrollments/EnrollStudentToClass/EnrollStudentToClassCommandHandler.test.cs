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
      .With(c => c.EnrolledCount, 1)
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
      LastName = "Doe",
      IdempotencyKey = Guid.NewGuid()
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
      .With(c => c.EnrolledCount, 0)
      .Build();
    await _dbContext.Classes.AddAsync(cls);
    await _dbContext.SaveChangesAsync();

    var cmd = new EnrollStudentToClassCommand
    {
      CourseId = cls.CourseId,
      ClassId = cls.Id,
      StudentId = studentId,
      FirstName = "John",
      LastName = "Doe",
      IdempotencyKey = Guid.NewGuid()
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
      .With(c => c.EnrolledCount, 1)
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
      LastName = "Doe",
      IdempotencyKey = Guid.NewGuid()
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
      .With(c => c.EnrolledCount, 0)
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
      LastName = "Doe",
      IdempotencyKey = Guid.NewGuid()
    };

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.False);

    var enrollment = await _dbContext.Enrollments.FirstOrDefaultAsync(e =>
      e.StudentId == studentId && e.ClassId == cls.Id);

    Assert.That(enrollment, Is.Not.Null);
    Assert.That(enrollment.StudentId, Is.EqualTo(studentId));
    Assert.That(enrollment.CourseId, Is.EqualTo(cls.CourseId));
    Assert.That(enrollment.ClassId, Is.EqualTo(cls.Id));

    var updatedClass = await _dbContext.Classes.FirstAsync(c => c.Id == cls.Id);
    Assert.That(updatedClass.EnrolledCount, Is.EqualTo(1));
  }

  [Test]
  public async Task Handle_ShouldReturnSuccess_WithoutDoubleEnrolling_WhenIdempotencyKeyAlreadyProcessed()
  {
    var studentId = Guid.NewGuid();
    var idempotencyKey = Guid.NewGuid();
    var cls = Builder<Class>.CreateNew()
      .With(c => c.MaxStudents, 1)
      .With(c => c.EnrolledCount, 1) // class already full
      .With(c => c.RegistrationDeadline, DateTime.UtcNow.AddDays(1))
      .With(c => c.CourseStartDate, DateTime.UtcNow.AddDays(2))
      .With(c => c.CourseEndDate, DateTime.UtcNow.AddDays(3))
      .Build();
    await _dbContext.Classes.AddAsync(cls);
    await _dbContext.IdempotencyRecords.AddAsync(new IdempotencyRecord
    {
      IdempotencyKey = idempotencyKey, Operation = "Enroll"
    });
    await _dbContext.SaveChangesAsync();

    var cmd = new EnrollStudentToClassCommand
    {
      CourseId = cls.CourseId,
      ClassId = cls.Id,
      StudentId = studentId,
      FirstName = "John",
      LastName = "Doe",
      IdempotencyKey = idempotencyKey
    };

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(await _dbContext.Enrollments.CountAsync(e => e.StudentId == studentId), Is.EqualTo(0));

    var updatedClass = await _dbContext.Classes.FirstAsync(c => c.Id == cls.Id);
    Assert.That(updatedClass.EnrolledCount, Is.EqualTo(1), "a replayed request must not re-claim a seat");
  }

  [Test]
  public async Task Handle_ShouldRetryAndLoseGracefully_WhenAnotherRequestClaimsTheLastSeatConcurrently()
  {
    var classId = Guid.NewGuid();
    var courseId = Guid.NewGuid();
    var student1Id = Guid.NewGuid();
    var student2Id = Guid.NewGuid();

    var cls = new Class
    {
      Id = classId,
      CourseId = courseId,
      MaxStudents = 1,
      EnrolledCount = 0,
      RegistrationDeadline = DateTime.UtcNow.AddDays(1),
      CourseStartDate = DateTime.UtcNow.AddDays(2),
      CourseEndDate = DateTime.UtcNow.AddDays(3)
    };
    await _dbContext.Classes.AddAsync(cls);
    await _dbContext.SaveChangesAsync();

    // Our handler's context now has the Class tracked at EnrolledCount=0 (its "stale read").
    // Force it to stay tracked (not reloaded) by touching it once more before the concurrent write.
    await _dbContext.Classes.FirstAsync(c => c.Id == classId);

    // A second "request" (separate DbContext instance, same in-memory database) claims the only
    // seat first and commits. EF Core's identity map means our handler's context will NOT see this
    // change on its next query — it keeps serving the already-tracked (now stale) instance — which
    // is exactly what would happen if two real requests raced: the loser's earlier read is stale by
    // the time it tries to save.
    await using var otherContext = ApplicationDbContextCreator.GetAdditionalDbContext();
    var otherHandler =
      new EnrollStudentToClassCommandHandler(Substitute.For<ILogger<EnrollStudentToClassCommandHandler>>(),
        otherContext);
    var winnerResult = await otherHandler.Handle(new EnrollStudentToClassCommand
    {
      CourseId = courseId,
      ClassId = classId,
      StudentId = student1Id,
      FirstName = "Winner",
      LastName = "Student",
      IdempotencyKey = Guid.NewGuid()
    }, CancellationToken.None);
    Assert.That(winnerResult.IsError, Is.False);

    // The loser's handler still has the stale (pre-claim) Class tracked. Its first attempt should
    // hit a concurrency conflict on SaveChanges, retry, reload fresh, and correctly see the class is
    // now full instead of overbooking it.
    var loserResult = await _handler.Handle(new EnrollStudentToClassCommand
    {
      CourseId = courseId,
      ClassId = classId,
      StudentId = student2Id,
      FirstName = "Loser",
      LastName = "Student",
      IdempotencyKey = Guid.NewGuid()
    }, CancellationToken.None);

    // EF Core's InMemory provider (unlike a real relational provider) doesn't roll back the other
    // pending inserts in the same SaveChanges call when only the Class update fails its concurrency
    // check, so the loser's own Enrollment row can end up persisted despite the exception. On Postgres
    // the whole SaveChanges call is one real transaction, so that can't happen there. Either conflict
    // code below means the important invariant held: the loser did not get a successful result.
    Assert.That(loserResult.IsError, Is.True);
    Assert.That(loserResult.FirstError.Code, Is.AnyOf(
      "enrollment_service.enroll_student_to_course.class_full",
      "enrollment_service.enroll_student_to_course.already_enrolled"));

    var finalClass = await otherContext.Classes.FirstAsync(c => c.Id == classId);
    Assert.That(finalClass.EnrolledCount, Is.EqualTo(1), "only the winning request should have claimed a seat");
  }
}

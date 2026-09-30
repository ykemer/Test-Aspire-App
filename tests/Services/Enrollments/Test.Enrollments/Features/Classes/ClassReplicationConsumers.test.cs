using Contracts.Classes.Events;
using Contracts.Courses.Events;

using ErrorOr;

using Mediator;

using NSubstitute;

using Service.Enrollments.Features.Classes.ClassDeleted;
using Service.Enrollments.Features.Classes.CourseDeleted;
using Service.Enrollments.Features.Classes.CreateClass;
using Service.Enrollments.Features.Classes.UpdateClass;

namespace Test.Enrollments.Features.Classes;

/// <summary>
/// The consumers that copy classes from Courses must never lose a message silently:
/// a failure has to throw, so Rebus retries it and finally moves it to the error queue.
/// </summary>
[TestFixture]
public class ClassReplicationConsumersTests
{
  private static readonly Error s_failure = Error.NotFound("enrollments_service.class.not_found", "not here yet");

  private IMediator _mediator = null!;

  [SetUp]
  public void SetUp() => _mediator = Substitute.For<IMediator>();

  private static ClassCreatedEvent CreatedEvent() =>
    new()
    {
      Id = Guid.NewGuid(),
      CourseId = Guid.NewGuid(),
      MaxStudents = 10,
      UserId = "user-1",
      RegistrationDeadline = DateTime.UtcNow,
      CourseStartDate = DateTime.UtcNow.AddDays(1),
      CourseEndDate = DateTime.UtcNow.AddDays(2)
    };

  private static ClassUpdatedEvent UpdatedEvent() =>
    new()
    {
      Id = Guid.NewGuid(),
      CourseId = Guid.NewGuid(),
      MaxStudents = 10,
      UserId = "user-1",
      RegistrationDeadline = DateTime.UtcNow,
      CourseStartDate = DateTime.UtcNow.AddDays(1),
      CourseEndDate = DateTime.UtcNow.AddDays(2)
    };

  [Test]
  public async Task ClassCreated_Success_DoesNotThrow()
  {
    _mediator.Send(Arg.Any<CreateClassCommand>(), Arg.Any<CancellationToken>()).Returns(Result.Created);
    var message = CreatedEvent();

    await new ClassCreatedEventConsumer(_mediator).Handle(message);

    await _mediator.Received(1).Send(Arg.Is<CreateClassCommand>(c => c.Id == message.Id && c.MaxStudents == 10),
      Arg.Any<CancellationToken>());
  }

  [Test]
  public void ClassCreated_Failure_Throws()
  {
    _mediator.Send(Arg.Any<CreateClassCommand>(), Arg.Any<CancellationToken>()).Returns(s_failure);

    Assert.ThrowsAsync<InvalidOperationException>(() => new ClassCreatedEventConsumer(_mediator).Handle(CreatedEvent()));
  }

  [Test]
  public void ClassUpdated_Failure_Throws()
  {
    _mediator.Send(Arg.Any<UpdateClassCommand>(), Arg.Any<CancellationToken>()).Returns(s_failure);

    Assert.ThrowsAsync<InvalidOperationException>(() => new ClassUpdatedEventConsumer(_mediator).Handle(UpdatedEvent()));
  }

  [Test]
  public void ClassDeleted_Failure_Throws()
  {
    _mediator.Send(Arg.Any<DeleteClassByClassIdCommand>(), Arg.Any<CancellationToken>()).Returns(s_failure);

    Assert.ThrowsAsync<InvalidOperationException>(() =>
      new ClassDeletedEventConsumer(_mediator).Handle(new ClassDeletedEvent
      {
        ClassId = Guid.NewGuid(), CourseId = Guid.NewGuid(), UserId = "user-1"
      }));
  }

  [Test]
  public void CourseDeleted_Failure_Throws()
  {
    _mediator.Send(Arg.Any<DeleteClassesByCourseIdCommand>(), Arg.Any<CancellationToken>()).Returns(s_failure);

    Assert.ThrowsAsync<InvalidOperationException>(() =>
      new CourseDeletedEventConsumer(_mediator).Handle(new CourseDeletedEvent
      {
        CourseId = Guid.NewGuid(), UserId = "user-1"
      }));
  }
}

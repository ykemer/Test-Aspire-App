using Contracts.Classes.Events;
using Contracts.Classes.Events.DecreaseClassEnrollmentsCount;
using Contracts.Classes.Events.IncreaseClassEnrollmentsCount;
using Contracts.Courses.Events;

using ErrorOr;

using Mediator;

using NSubstitute;

using Rebus.Bus;

using Service.Courses.Features.Classes.DeleteClass;
using Service.Courses.Features.Classes.UpdateNumberOfEnrolledStudents;
using Service.Courses.Features.Courses.DeleteCourse;

namespace Test.Courses.Features;

/// <summary>
/// Consumers turn a bus message into a command, and publish exactly one result event.
/// </summary>
[TestFixture]
public class ConsumersTests
{
  private IBus _bus = null!;
  private IMediator _mediator = null!;

  [SetUp]
  public void SetUp()
  {
    _bus = Substitute.For<IBus>();
    _mediator = Substitute.For<IMediator>();
  }

  [TearDown]
  public void TearDown() => _bus.Dispose();

  [Test]
  public async Task DeleteCourse_Success_PublishesExactlyOneDeletedEvent()
  {
    var message = new Contracts.Courses.Commands.DeleteCourseCommand { CourseId = Guid.NewGuid(), UserId = "user-1" };
    _mediator.Send(Arg.Any<DeleteCourseCommand>(), Arg.Any<CancellationToken>()).Returns(Result.Deleted);

    await new DeleteCourseCommandConsumer(_mediator, _bus).Handle(message);

    await _bus.Received(1).Publish(Arg.Is<CourseDeletedEvent>(e =>
      e.CourseId == message.CourseId && e.UserId == message.UserId));
    Assert.That(_bus.ReceivedCalls().Count(), Is.EqualTo(1));
  }

  [Test]
  public async Task DeleteCourse_Failure_PublishesRejectionWithReason()
  {
    var message = new Contracts.Courses.Commands.DeleteCourseCommand { CourseId = Guid.NewGuid(), UserId = "user-1" };
    _mediator.Send(Arg.Any<DeleteCourseCommand>(), Arg.Any<CancellationToken>())
      .Returns(Error.Conflict("code", "has students"));

    await new DeleteCourseCommandConsumer(_mediator, _bus).Handle(message);

    await _bus.Received(1).Publish(Arg.Is<CourseDeleteRejectionEvent>(e => e.Reason == "has students"));
    await _bus.DidNotReceive().Publish(Arg.Any<CourseDeletedEvent>());
  }

  [Test]
  public async Task DeleteClass_Success_PublishesExactlyOneDeletedEvent()
  {
    var message = new Contracts.Classes.Commands.DeleteClassCommand
    {
      ClassId = Guid.NewGuid(), CourseId = Guid.NewGuid(), UserId = "user-1"
    };
    _mediator.Send(Arg.Any<DeleteClassCommand>(), Arg.Any<CancellationToken>()).Returns(Result.Deleted);

    await new DeleteClassCommandConsumer(_mediator, _bus).Handle(message);

    await _bus.Received(1).Publish(Arg.Is<ClassDeletedEvent>(e => e.ClassId == message.ClassId));
    Assert.That(_bus.ReceivedCalls().Count(), Is.EqualTo(1));
  }

  [Test]
  public async Task IncreaseEnrollments_SendsEventIdAndAddStudent_AndPublishesSuccess()
  {
    var message = new IncreaseClassEnrollmentsCountEvent { CourseId = Guid.NewGuid(), ClassId = Guid.NewGuid() };
    _mediator.Send(Arg.Any<UpdateNumberOfEnrolledStudentsCommand>(), Arg.Any<CancellationToken>())
      .Returns(Result.Updated);

    await new IncreaseCourseEnrollmentsCountEventConsumer(_mediator, _bus).Handle(message);

    await _mediator.Received(1).Send(
      new UpdateNumberOfEnrolledStudentsCommand(message.EventId, message.CourseId, message.ClassId,
        EnrollmentChange.AddStudent),
      Arg.Any<CancellationToken>());
    await _bus.Received(1).Publish(Arg.Is<IncreaseClassEnrollmentsCountSuccessEvent>(e =>
      e.EventId == message.EventId));
  }

  [Test]
  public async Task DecreaseEnrollments_Failure_PublishesFailedEventWithErrorMessage()
  {
    var message = new DecreaseClassEnrollmentsCountEvent { CourseId = Guid.NewGuid(), ClassId = Guid.NewGuid() };
    _mediator.Send(Arg.Any<UpdateNumberOfEnrolledStudentsCommand>(), Arg.Any<CancellationToken>())
      .Returns(Error.Conflict("code", "no students"));

    await new DecreaseCourseEnrollmentsCountEventConsumer(_mediator, _bus).Handle(message);

    await _mediator.Received(1).Send(
      Arg.Is<UpdateNumberOfEnrolledStudentsCommand>(c => c.Change == EnrollmentChange.RemoveStudent),
      Arg.Any<CancellationToken>());
    await _bus.Received(1).Publish(Arg.Is<DecreaseClassEnrollmentsCountFailedEvent>(e =>
      e.EventId == message.EventId && e.ErrorMessage == "no students"));
  }
}

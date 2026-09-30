using Contracts.Students.Events.DecreaseStudentEnrollmentCount;
using Contracts.Students.Events.IncreaseStudentEnrollmentsCount;
using Contracts.Users.Events;

using ErrorOr;

using Mediator;

using NSubstitute;

using Rebus.Bus;

using Service.Students.Features.CreateStudent;
using Service.Students.Features.UpdateStudentEnrollmentsCount;

namespace Test.Students.Features;

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

  private static UserCreatedEvent UserCreated() =>
    new()
    {
      Id = Guid.NewGuid(),
      FirstName = "Jane",
      LastName = "Doe",
      Email = "jane@example.com",
      DateOfBirth = new DateTime(2000, 1, 1)
    };

  [Test]
  public async Task UserCreated_Success_DoesNotThrow()
  {
    _mediator.Send(Arg.Any<CreateStudentCommand>(), Arg.Any<CancellationToken>()).Returns(Result.Created);
    var message = UserCreated();

    await new UserCreatedEventConsumer(_mediator).Handle(message);

    await _mediator.Received(1).Send(Arg.Is<CreateStudentCommand>(c => c.Id == message.Id),
      Arg.Any<CancellationToken>());
  }

  [Test]
  public void UserCreated_Failure_Throws_SoTheMessageIsRetriedNotLost()
  {
    _mediator.Send(Arg.Any<CreateStudentCommand>(), Arg.Any<CancellationToken>())
      .Returns(Error.Conflict("students_service.student.email_already_taken", "taken"));

    Assert.ThrowsAsync<InvalidOperationException>(() => new UserCreatedEventConsumer(_mediator).Handle(UserCreated()));
  }

  [Test]
  public async Task IncreaseEnrollments_SendsEventId_AndPublishesSuccess()
  {
    var message = new IncreaseStudentEnrollmentsCountEvent { StudentId = Guid.NewGuid() };
    _mediator.Send(Arg.Any<UpdateStudentEnrollmentsCountCommand>(), Arg.Any<CancellationToken>())
      .Returns(Result.Updated);

    await new IncreaseStudentEnrollmentsEventConsumer(_mediator, _bus).Handle(message);

    await _mediator.Received(1).Send(
      new UpdateStudentEnrollmentsCountCommand(message.EventId, message.StudentId, EnrollmentChange.AddEnrollment),
      Arg.Any<CancellationToken>());
    await _bus.Received(1).Publish(Arg.Is<IncreaseStudentEnrollmentsCountSuccessEvent>(e =>
      e.EventId == message.EventId && e.StudentId == message.StudentId));
  }

  [Test]
  public async Task DecreaseEnrollments_Failure_PublishesFailedEventWithReason()
  {
    var message = new DecreaseStudentEnrollmentCountEvent { StudentId = Guid.NewGuid() };
    _mediator.Send(Arg.Any<UpdateStudentEnrollmentsCountCommand>(), Arg.Any<CancellationToken>())
      .Returns(Error.Conflict("code", "no enrollments"));

    await new DecreaseStudentEnrollmentsEventConsumer(_mediator, _bus).Handle(message);

    await _mediator.Received(1).Send(
      Arg.Is<UpdateStudentEnrollmentsCountCommand>(c => c.Change == EnrollmentChange.RemoveEnrollment),
      Arg.Any<CancellationToken>());
    await _bus.Received(1).Publish(Arg.Is<DecreaseStudentEnrollmentCountFailedEvent>(e =>
      e.EventId == message.EventId && e.ErrorMessage == "no enrollments"));
  }
}

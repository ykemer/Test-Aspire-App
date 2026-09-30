using Contracts.Classes.Events.IncreaseClassEnrollmentsCount;
using Contracts.Enrollments.Events;
using Contracts.Enrollments.Hub;
using Contracts.Students.Events.IncreaseStudentEnrollmentsCount;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Time.Testing;

using NSubstitute;

using Platform.Common.StateMachines;
using Platform.Features.Enrollments;

using Rebus.TestHelpers;
using Rebus.TestHelpers.Events;

namespace Test.Platform.Common.StateMachines;

/// <summary>
/// Walks the enroll saga step by step with Rebus's in-memory saga test helper.
/// </summary>
[TestFixture]
public class EnrollSagaTests
{
  private FakeBus _bus = null!;
  private IClientProxy _studentConnection = null!;
  private SagaFixture<StudentEnrollStateMachine> _saga = null!;

  [SetUp]
  public void SetUp()
  {
    _bus = new FakeBus();
    _studentConnection = Substitute.For<IClientProxy>();
    var hub = Substitute.For<IHubContext<EnrollmentHub>>();
    hub.Clients.User(Arg.Any<string>()).Returns(_studentConnection);

    var clock = new FakeTimeProvider(new DateTimeOffset(2030, 1, 1, 12, 0, 0, TimeSpan.Zero));
    _saga = SagaFixture.For(() => new StudentEnrollStateMachine(_bus, hub, clock));
  }

  [TearDown]
  public void TearDown()
  {
    _saga.Dispose();
    _bus.Dispose();
  }

  private T SinglePublished<T>() =>
    _bus.Events.OfType<MessagePublished<T>>().Single().EventMessage;

  [Test]
  public void EnrollmentCreated_AsksStudentsToIncreaseTheCount()
  {
    var created = new EnrollmentCreatedEvent { StudentId = Guid.NewGuid(), ClassId = Guid.NewGuid() };

    _saga.Deliver(created);

    var published = SinglePublished<IncreaseStudentEnrollmentsCountEvent>();
    Assert.That(published.StudentId, Is.EqualTo(created.StudentId));
    Assert.That(published.EventId, Is.EqualTo(created.EventId));
    Assert.That(_saga.Data.OfType<StudentEnrollState>().Single().State,
      Is.EqualTo(SagaStates.ChangingStudentEnrollmentsCount));
  }

  [Test]
  public void StudentCountUpdated_ThenAsksCoursesToIncreaseTheClassCount()
  {
    var created = new EnrollmentCreatedEvent { StudentId = Guid.NewGuid(), ClassId = Guid.NewGuid() };
    _saga.Deliver(created);

    _saga.Deliver(new IncreaseStudentEnrollmentsCountSuccessEvent { EventId = created.EventId });

    var published = SinglePublished<IncreaseClassEnrollmentsCountEvent>();
    Assert.That(published.ClassId, Is.EqualTo(created.ClassId));
  }

  [Test]
  public async Task BothCountsUpdated_NotifiesTheStudent_AndFinishes()
  {
    var created = new EnrollmentCreatedEvent { StudentId = Guid.NewGuid(), ClassId = Guid.NewGuid() };
    _saga.Deliver(created);
    _saga.Deliver(new IncreaseStudentEnrollmentsCountSuccessEvent { EventId = created.EventId });

    _saga.Deliver(new IncreaseClassEnrollmentsCountSuccessEvent { EventId = created.EventId });

    await _studentConnection.Received(1).SendCoreAsync(EnrollmentHubMessages.EnrollmentCreated,
      Arg.Any<object?[]>(), Arg.Any<CancellationToken>());
    Assert.That(_saga.Data, Is.Empty, "a finished saga is removed");
  }

  [Test]
  public async Task StudentCountFailed_NotifiesTheStudent_AndFinishes()
  {
    var created = new EnrollmentCreatedEvent { StudentId = Guid.NewGuid(), ClassId = Guid.NewGuid() };
    _saga.Deliver(created);

    _saga.Deliver(new IncreaseStudentEnrollmentsCountFailedEvent { EventId = created.EventId, ErrorMessage = "x" });

    await _studentConnection.Received(1).SendCoreAsync(EnrollmentHubMessages.EnrollmentCreateRequestRejected,
      Arg.Any<object?[]>(), Arg.Any<CancellationToken>());
    Assert.That(_saga.Data, Is.Empty);
    Assert.That(_bus.Events.OfType<MessagePublished<IncreaseClassEnrollmentsCountEvent>>(), Is.Empty,
      "the class count must not be touched after a failure");
  }

  [Test]
  public void RepeatedStudentSuccess_DoesNotAskCoursesTwice()
  {
    var created = new EnrollmentCreatedEvent { StudentId = Guid.NewGuid(), ClassId = Guid.NewGuid() };
    _saga.Deliver(created);

    _saga.Deliver(new IncreaseStudentEnrollmentsCountSuccessEvent { EventId = created.EventId });
    _saga.Deliver(new IncreaseStudentEnrollmentsCountSuccessEvent { EventId = created.EventId });

    Assert.That(_bus.Events.OfType<MessagePublished<IncreaseClassEnrollmentsCountEvent>>().Count(), Is.EqualTo(1));
  }
}

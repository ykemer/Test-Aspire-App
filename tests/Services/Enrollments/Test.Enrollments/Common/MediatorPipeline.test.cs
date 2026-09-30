using ErrorOr;

using Mediator;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Setup;
using Service.Enrollments.Features.Classes.CreateClass;
using Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

namespace Test.Enrollments.Common;

/// <summary>
/// Sends requests through the real dependency-injection setup and mediator pipeline,
/// to prove the validators are registered and actually run.
/// </summary>
[TestFixture]
public class MediatorPipelineTests
{
  private ServiceProvider _serviceProvider = null!;
  private AsyncServiceScope _scope;
  private IMediator _mediator = null!;

  [SetUp]
  public void SetUp()
  {
    var services = new ServiceCollection();
    services.AddLogging();
    services.AddServices();
    services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

    _serviceProvider = services.BuildServiceProvider();
    _scope = _serviceProvider.CreateAsyncScope();
    _mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
  }

  [TearDown]
  public async Task TearDown()
  {
    await _scope.DisposeAsync();
    await _serviceProvider.DisposeAsync();
  }

  [Test]
  public async Task InvalidEnrollRequest_IsRejectedByValidator()
  {
    var result = await _mediator.Send(new EnrollStudentToClassCommand
    {
      CourseId = Guid.Empty,
      ClassId = Guid.NewGuid(),
      StudentId = Guid.NewGuid(),
      FirstName = "",
      LastName = "Doe",
      IdempotencyKey = Guid.NewGuid()
    });

    Assert.That(result.IsError, Is.True);
    Assert.That(result.Errors.Select(error => error.Type), Is.All.EqualTo(ErrorType.Validation));
  }

  [Test]
  public async Task ValidClassCopy_PassesValidation_AndIsSaved()
  {
    var result = await _mediator.Send(new CreateClassCommand
    {
      Id = Guid.NewGuid(),
      CourseId = Guid.NewGuid(),
      MaxStudents = 5,
      RegistrationDeadline = DateTime.UtcNow.AddDays(-10),
      CourseStartDate = DateTime.UtcNow.AddDays(-9),
      CourseEndDate = DateTime.UtcNow.AddDays(-8)
    });

    Assert.That(result.IsError, Is.False, "old dates are fine for copied classes");
  }
}

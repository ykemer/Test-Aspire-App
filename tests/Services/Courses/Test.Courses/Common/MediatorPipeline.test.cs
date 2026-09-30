using ErrorOr;

using Mediator;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Service.Courses.Common.Database;
using Service.Courses.Common.Setup;
using Service.Courses.Features.Courses.CreateCourse;
using Service.Courses.Features.Courses.ListCourses;

namespace Test.Courses.Common;

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
  public async Task InvalidCommand_IsRejectedByValidator_AndNothingIsSaved()
  {
    var result = await _mediator.Send(new CreateCourseCommand("ab", ""));

    Assert.That(result.IsError, Is.True);
    Assert.That(result.Errors.Select(error => error.Type), Is.All.EqualTo(ErrorType.Validation));
    var dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    Assert.That(await dbContext.Courses.CountAsync(), Is.Zero);
  }

  [Test]
  public async Task ValidCommand_PassesValidation_AndIsSaved()
  {
    var result = await _mediator.Send(new CreateCourseCommand("Algebra", "Numbers and letters"));

    Assert.That(result.IsError, Is.False);
  }

  [TestCase(0, 10)]
  [TestCase(1, 0)]
  [TestCase(-1, 10)]
  public async Task ListCourses_WithInvalidPaging_IsRejected(int pageNumber, int pageSize)
  {
    var result = await _mediator.Send(new ListCoursesQuery { PageNumber = pageNumber, PageSize = pageSize });

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.Validation));
  }
}

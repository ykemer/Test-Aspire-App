using ErrorOr;

using Mediator;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Service.Students.Common.Database;
using Service.Students.Common.Setup;
using Service.Students.Features.CreateStudent;
using Service.Students.Features.ListStudents;

namespace Test.Students.Common;

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
  public async Task InvalidStudent_IsRejectedByValidator_AndNothingIsSaved()
  {
    var result = await _mediator.Send(new CreateStudentCommand
    {
      Id = Guid.NewGuid(), FirstName = "", LastName = "Doe", Email = "not-an-email", DateOfBirth = default
    });

    Assert.That(result.IsError, Is.True);
    Assert.That(result.Errors.Select(error => error.Type), Is.All.EqualTo(ErrorType.Validation));
    var dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    Assert.That(await dbContext.Students.CountAsync(), Is.Zero);
  }

  [TestCase(0, 10)]
  [TestCase(1, 0)]
  public async Task ListStudents_WithInvalidPaging_IsRejected(int pageNumber, int pageSize)
  {
    var result = await _mediator.Send(new ListStudentsQuery { PageNumber = pageNumber, PageSize = pageSize });

    Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.Validation));
  }
}

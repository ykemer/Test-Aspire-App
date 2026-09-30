using ErrorOr;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

using Service.Students.Features;
using Service.Students.Features.CreateStudent;

namespace Test.Students.Integration;

public class CreateStudentTests : PostgresTestBase
{
  private async Task<ErrorOr<Created>> CreateInParallel(CreateStudentCommand command)
  {
    await using var dbContext = PostgresDatabase.CreateDbContext();
    return await new CreateStudentCommandHandler(dbContext, NullLogger<CreateStudentCommandHandler>.Instance, Clock)
      .Handle(command, CancellationToken.None);
  }

  private static CreateStudentCommand Command(Guid id, string email) =>
    new()
    {
      Id = id, FirstName = "Jane", LastName = "Doe", Email = email, DateOfBirth = new DateTime(2000, 1, 1)
    };

  [Test]
  public async Task SameUserDeliveredInParallel_CreatesOneStudent_AndAllSucceed()
  {
    var command = Command(Guid.NewGuid(), "jane@example.com");

    var results = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => CreateInParallel(command)));

    Assert.That(results.Select(result => result.IsError), Is.All.False);
    Assert.That(await DbContext.Students.CountAsync(), Is.EqualTo(1));
  }

  [Test]
  public async Task DifferentUsersWithSameEmailInParallel_OnlyOneWins()
  {
    var results = await Task.WhenAll(Enumerable.Range(0, 5)
      .Select(i => CreateInParallel(Command(Guid.NewGuid(), i % 2 == 0 ? "JOE@example.com" : "joe@example.com"))));

    Assert.That(results.Count(result => !result.IsError), Is.EqualTo(1));
    Assert.That(results.Where(result => result.IsError).Select(result => result.FirstError.Code),
      Is.All.EqualTo(StudentErrors.EmailAlreadyTaken("x").Code));
    Assert.That(await DbContext.Students.CountAsync(), Is.EqualTo(1));
  }
}

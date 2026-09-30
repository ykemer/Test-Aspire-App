using FluentValidation.TestHelper;

using Microsoft.Extensions.Time.Testing;

using Service.Courses.Features.Classes.CreateClass;

using Test.Courses.Setup;

namespace Test.Courses.Features.Classes.CreateClass;

[TestFixture]
public class CreateClassCommandValidatorTests
{
  private static readonly DateTime s_now = TestClock.Now;

  private FakeTimeProvider _clock = null!;
  private CreateClassCommandValidator _validator = null!;

  [SetUp]
  public void SetUp()
  {
    _clock = TestClock.Create();
    _validator = new CreateClassCommandValidator(_clock);
  }

  private static CreateClassCommand ValidCommand() =>
    new()
    {
      CourseId = Guid.NewGuid(),
      RegistrationDeadline = s_now.AddDays(1),
      CourseStartDate = s_now.AddDays(2),
      CourseEndDate = s_now.AddDays(3),
      MaxStudents = 10
    };

  [Test]
  public void ValidRequest_ShouldPass() =>
    _validator.TestValidate(ValidCommand()).ShouldNotHaveAnyValidationErrors();

  [Test]
  public void EmptyCourseId_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { CourseId = Guid.Empty })
      .ShouldHaveValidationErrorFor(x => x.CourseId);

  [Test]
  public void RegistrationDeadline_InPast_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { RegistrationDeadline = s_now.AddDays(-1) })
      .ShouldHaveValidationErrorFor(x => x.RegistrationDeadline);

  [Test]
  public void RegistrationDeadline_AfterCourseStartDate_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { RegistrationDeadline = s_now.AddDays(2.5) })
      .ShouldHaveValidationErrorFor(x => x.RegistrationDeadline);

  [Test]
  public void CourseStartDate_InPast_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { CourseStartDate = s_now.AddDays(-1) })
      .ShouldHaveValidationErrorFor(x => x.CourseStartDate);

  [Test]
  public void CourseStartDate_AfterCourseEndDate_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { CourseStartDate = s_now.AddDays(4) })
      .ShouldHaveValidationErrorFor(x => x.CourseStartDate);

  [Test]
  public void CourseEndDate_InPast_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { CourseEndDate = s_now.AddDays(-1) })
      .ShouldHaveValidationErrorFor(x => x.CourseEndDate);

  [TestCase(0)]
  [TestCase(-5)]
  public void MaxStudents_ZeroOrNegative_ShouldFail(int maxStudents) =>
    _validator.TestValidate(ValidCommand() with { MaxStudents = maxStudents })
      .ShouldHaveValidationErrorFor(x => x.MaxStudents);

  [Test]
  public void ReusedValidator_ShouldUseCurrentTime_NotTheTimeItWasCreated()
  {
    // Arrange: a command that is valid right now...
    var command = ValidCommand();
    _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();

    // Act: ...time moves past the registration deadline
    _clock.Advance(TimeSpan.FromDays(1.5));

    // Assert
    _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.RegistrationDeadline);
  }
}

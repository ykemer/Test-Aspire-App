using FluentValidation.TestHelper;

using Service.Courses.Features.Classes.UpdateClass;

using Test.Courses.Setup;

namespace Test.Courses.Features.Classes.UpdateClass;

[TestFixture]
public class UpdateClassCommandValidatorTests
{
  private static readonly DateTime s_now = TestClock.Now;

  private UpdateClassCommandValidator _validator = null!;

  [SetUp]
  public void SetUp() => _validator = new UpdateClassCommandValidator(TestClock.Create());

  private static UpdateClassCommand ValidCommand() =>
    new()
    {
      Id = Guid.NewGuid(),
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
  public void EmptyId_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { Id = Guid.Empty })
      .ShouldHaveValidationErrorFor(x => x.Id);

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
}

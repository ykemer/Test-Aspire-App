using FluentValidation.TestHelper;

using Service.Enrollments.Features.Classes.CreateClass;
using Service.Enrollments.Features.Classes.UpdateClass;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Classes;

/// <summary>
/// Create and update share the same rules (ClassDetailsValidator), so they are tested once, through "create".
/// </summary>
[TestFixture]
public class ClassDetailsValidatorTests
{
  private readonly CreateClassCommandValidator _validator = new();

  private static CreateClassCommand ValidCommand() =>
    new()
    {
      Id = Guid.NewGuid(),
      CourseId = Guid.NewGuid(),
      MaxStudents = 10,
      RegistrationDeadline = TestClock.Now.AddDays(1),
      CourseStartDate = TestClock.Now.AddDays(2),
      CourseEndDate = TestClock.Now.AddDays(3)
    };

  [Test]
  public void ValidCommand_ShouldPass() =>
    _validator.TestValidate(ValidCommand()).ShouldNotHaveAnyValidationErrors();

  [Test]
  public void DatesInThePast_ShouldPass_BecauseCoursesAlreadyValidatedThem()
  {
    // A late or replayed message must still be copied, or this service drifts away from Courses.
    var lateMessage = ValidCommand() with
    {
      RegistrationDeadline = DateTime.UtcNow.AddYears(-2),
      CourseStartDate = DateTime.UtcNow.AddYears(-2).AddDays(1),
      CourseEndDate = DateTime.UtcNow.AddYears(-2).AddDays(2)
    };

    _validator.TestValidate(lateMessage).ShouldNotHaveAnyValidationErrors();
  }

  [Test]
  public void EmptyId_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { Id = Guid.Empty }).ShouldHaveValidationErrorFor(x => x.Id);

  [Test]
  public void EmptyCourseId_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { CourseId = Guid.Empty })
      .ShouldHaveValidationErrorFor(x => x.CourseId);

  [Test]
  public void RegistrationDeadline_AfterStart_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { RegistrationDeadline = TestClock.Now.AddDays(2.5) })
      .ShouldHaveValidationErrorFor(x => x.RegistrationDeadline);

  [Test]
  public void Start_AfterEnd_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { CourseStartDate = TestClock.Now.AddDays(4) })
      .ShouldHaveValidationErrorFor(x => x.CourseStartDate);

  [TestCase(0)]
  [TestCase(-1)]
  public void MaxStudents_BelowOne_ShouldFail(int maxStudents) =>
    _validator.TestValidate(ValidCommand() with { MaxStudents = maxStudents })
      .ShouldHaveValidationErrorFor(x => x.MaxStudents);

  [Test]
  public void UpdateValidator_UsesTheSameRules() =>
    new UpdateClassCommandValidator()
      .TestValidate(new UpdateClassCommand
      {
        Id = Guid.NewGuid(),
        CourseId = Guid.NewGuid(),
        MaxStudents = 0,
        RegistrationDeadline = TestClock.Now,
        CourseStartDate = TestClock.Now.AddDays(1),
        CourseEndDate = TestClock.Now.AddDays(2)
      })
      .ShouldHaveValidationErrorFor(x => x.MaxStudents);
}

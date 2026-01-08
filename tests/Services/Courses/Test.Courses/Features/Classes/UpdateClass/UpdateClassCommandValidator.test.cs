using FluentValidation.TestHelper;

using Service.Courses.Features.Classes.UpdateClass;

namespace Courses.Application.Features.Classes.UpdateClass;

[TestFixture]
public class UpdateClassCommandValidatorTests
{
  [SetUp]
  public void SetUp() => _validator = new UpdateClassCommandValidator();

  private UpdateClassCommandValidator _validator;

  [Test]
  public void ValidRequest_ShouldPass()
  {
    var now = DateTime.UtcNow.AddMinutes(1);
    var request = new UpdateClassCommand
    {
      Id = "class-1",
      CourseId = "course-1",
      RegistrationDeadline = now.AddDays(1),
      CourseStartDate = now.AddDays(2),
      CourseEndDate = now.AddDays(3),
      MaxStudents = 10
    };
    var result = _validator.TestValidate(request);
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Test]
  public void RegistrationDeadline_InPast_ShouldFail()
  {
    var now = DateTime.UtcNow;
    var request = new UpdateClassCommand
    {
      Id = "class-1",
      CourseId = "course-1",
      RegistrationDeadline = now.AddDays(-1),
      CourseStartDate = now.AddDays(2),
      CourseEndDate = now.AddDays(3),
      MaxStudents = 10
    };
    var result = _validator.TestValidate(request);
    result.ShouldHaveValidationErrorFor(x => x.RegistrationDeadline);
  }

  [Test]
  public void RegistrationDeadline_AfterCourseStartDate_ShouldFail()
  {
    var now = DateTime.UtcNow.AddMinutes(1);
    var request = new UpdateClassCommand
    {
      Id = "class-1",
      CourseId = "course-1",
      RegistrationDeadline = now.AddDays(3),
      CourseStartDate = now.AddDays(2),
      CourseEndDate = now.AddDays(4),
      MaxStudents = 10
    };
    var result = _validator.TestValidate(request);
    result.ShouldHaveValidationErrorFor(x => x.RegistrationDeadline);
  }

  [Test]
  public void RegistrationDeadline_AfterCourseEndDate_ShouldFail()
  {
    var now = DateTime.UtcNow.AddMinutes(1);
    var request = new UpdateClassCommand
    {
      Id = "class-1",
      CourseId = "course-1",
      RegistrationDeadline = now.AddDays(5),
      CourseStartDate = now.AddDays(2),
      CourseEndDate = now.AddDays(4),
      MaxStudents = 10
    };
    var result = _validator.TestValidate(request);
    result.ShouldHaveValidationErrorFor(x => x.RegistrationDeadline);
  }

  [Test]
  public void CourseStartDate_InPast_ShouldFail()
  {
    var now = DateTime.UtcNow;
    var request = new UpdateClassCommand
    {
      Id = "class-1",
      CourseId = "course-1",
      RegistrationDeadline = now.AddDays(1),
      CourseStartDate = now.AddDays(-1),
      CourseEndDate = now.AddDays(3),
      MaxStudents = 10
    };
    var result = _validator.TestValidate(request);
    result.ShouldHaveValidationErrorFor(x => x.CourseStartDate);
  }

  [Test]
  public void CourseStartDate_AfterCourseEndDate_ShouldFail()
  {
    var now = DateTime.UtcNow.AddMinutes(1);
    var request = new UpdateClassCommand
    {
      Id = "class-1",
      CourseId = "course-1",
      RegistrationDeadline = now.AddDays(1),
      CourseStartDate = now.AddDays(5),
      CourseEndDate = now.AddDays(4),
      MaxStudents = 10
    };
    var result = _validator.TestValidate(request);
    result.ShouldHaveValidationErrorFor(x => x.CourseStartDate);
  }

  [Test]
  public void CourseEndDate_InPast_ShouldFail()
  {
    var now = DateTime.UtcNow;
    var request = new UpdateClassCommand
    {
      Id = "class-1",
      CourseId = "course-1",
      RegistrationDeadline = now.AddDays(1),
      CourseStartDate = now.AddDays(2),
      CourseEndDate = now.AddDays(-1),
      MaxStudents = 10
    };
    var result = _validator.TestValidate(request);
    result.ShouldHaveValidationErrorFor(x => x.CourseEndDate);
  }

  [Test]
  public void MaxStudents_ZeroOrNegative_ShouldFail()
  {
    var now = DateTime.UtcNow.AddMinutes(1);
    var request = new UpdateClassCommand
    {
      Id = "class-1",
      CourseId = "course-1",
      RegistrationDeadline = now.AddDays(1),
      CourseStartDate = now.AddDays(2),
      CourseEndDate = now.AddDays(3),
      MaxStudents = 0
    };
    var result = _validator.TestValidate(request);
    result.ShouldHaveValidationErrorFor(x => x.MaxStudents);

    request.MaxStudents = -5;
    result = _validator.TestValidate(request);
    result.ShouldHaveValidationErrorFor(x => x.MaxStudents);
  }
}

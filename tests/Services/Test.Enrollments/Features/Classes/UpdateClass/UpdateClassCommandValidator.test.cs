using FluentValidation.TestHelper;

using Service.Enrollments.Features.Classes.UpdateClass;

namespace Test.Enrollments.Features.Classes.UpdateClass;

[TestFixture]
public class UpdateClassCommandValidatorTests
{
  private UpdateClassCommandValidator _validator;

  [SetUp]
  public void SetUp() => _validator = new UpdateClassCommandValidator();

  [Test]
  public void ValidRequest_ShouldPass()
  {
    var now = DateTime.UtcNow.AddMinutes(1);
    var request = new UpdateClassCommand
    {
      Id = Guid.NewGuid().ToString(),
      CourseId = Guid.NewGuid().ToString(),
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
      Id = Guid.NewGuid().ToString(),
      CourseId = Guid.NewGuid().ToString(),
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
      Id = Guid.NewGuid().ToString(),
      CourseId = Guid.NewGuid().ToString(),
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
      Id = Guid.NewGuid().ToString(),
      CourseId = Guid.NewGuid().ToString(),
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
      Id = Guid.NewGuid().ToString(),
      CourseId = Guid.NewGuid().ToString(),
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
      Id = Guid.NewGuid().ToString(),
      CourseId = Guid.NewGuid().ToString(),
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
      Id = Guid.NewGuid().ToString(),
      CourseId = Guid.NewGuid().ToString(),
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
      Id = Guid.NewGuid().ToString(),
      CourseId = Guid.NewGuid().ToString(),
      RegistrationDeadline = now.AddDays(1),
      CourseStartDate = now.AddDays(2),
      CourseEndDate = now.AddDays(3),
      MaxStudents = 0
    };
    var result = _validator.TestValidate(request);
    result.ShouldHaveValidationErrorFor(x => x.MaxStudents);

    var request2 = new UpdateClassCommand
    {
      Id = Guid.NewGuid().ToString(),
      CourseId = Guid.NewGuid().ToString(),
      RegistrationDeadline = now.AddDays(1),
      CourseStartDate = now.AddDays(2),
      CourseEndDate = now.AddDays(3),
      MaxStudents = -5
    };
    result = _validator.TestValidate(request2);
    result.ShouldHaveValidationErrorFor(x => x.MaxStudents);
  }
}

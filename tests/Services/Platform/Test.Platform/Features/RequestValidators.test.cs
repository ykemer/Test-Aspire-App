using Contracts.Classes.Requests;
using Contracts.Courses.Requests;

using FluentValidation.TestHelper;

using Microsoft.Extensions.Time.Testing;

using Platform.Features;
using Platform.Features.Classes.CreateClass;
using Platform.Features.Classes.UpdateClass;
using Platform.Features.Courses.CreateCourse;
using Platform.Features.Courses.UpdateCourse;

namespace Test.Platform.Features;

[TestFixture]
public class RequestValidatorsTests
{
  private static readonly DateTime s_now = new(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc);

  private static CreateClassRequest ValidClass() =>
    new()
    {
      RegistrationDeadline = s_now.AddDays(1),
      CourseStartDate = s_now.AddDays(2),
      CourseEndDate = s_now.AddDays(3),
      MaxStudents = 10
    };

  [Test]
  public void ValidClass_ShouldPass() =>
    new CreateClassCommandValidator(new FakeTimeProvider(s_now)).TestValidate(ValidClass())
      .ShouldNotHaveAnyValidationErrors();

  [Test]
  public void ClassValidator_ReadsTheClockOnEveryValidation()
  {
    // FastEndpoints creates validators once. The old rules captured "now" at that moment.
    var clock = new FakeTimeProvider(s_now);
    var validator = new CreateClassCommandValidator(clock);
    var request = ValidClass();

    clock.Advance(TimeSpan.FromDays(1.5));

    validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.RegistrationDeadline);
  }

  [Test]
  public void UpdateClass_UsesTheSameRules()
  {
    var request = new UpdateClassRequest
    {
      RegistrationDeadline = s_now.AddDays(1),
      CourseStartDate = s_now.AddDays(2),
      CourseEndDate = s_now.AddDays(3),
      MaxStudents = 1001
    };

    new UpdateClassCommandValidator(new FakeTimeProvider(s_now)).TestValidate(request)
      .ShouldHaveValidationErrorFor(x => x.MaxStudents);
  }

  [Test]
  public void ClassStartingAfterItEnds_ShouldFail()
  {
    var request = ValidClass();
    request.CourseStartDate = s_now.AddDays(5);

    new CreateClassCommandValidator(new FakeTimeProvider(s_now)).TestValidate(request)
      .ShouldHaveValidationErrorFor(x => x.CourseStartDate);
  }

  [Test]
  public void Course_DescriptionTooLong_ShouldFail_WithAHonestMessage()
  {
    var request = new CreateCourseRequest { Name = "Algebra", Description = new string('d', 501) };

    new CreateCourseCommandValidator().TestValidate(request)
      .ShouldHaveValidationErrorFor(x => x.Description)
      .WithErrorMessage("Description must be between 3 and 500 characters.");
  }

  [Test]
  public void UpdateCourse_UsesTheSameRules() =>
    new UpdateCourseCommandValidator().TestValidate(new UpdateCourseRequest { Name = "ab", Description = "desc" })
      .ShouldHaveValidationErrorFor(x => x.Name);

  [TestCase(0, 10)]
  [TestCase(1, 0)]
  public void InvalidPaging_ShouldFail(int pageNumber, int pageSize)
  {
    var request = new ListCoursesRequest { ClassId = Guid.Empty, PageNumber = pageNumber, PageSize = pageSize };

    new ListCoursesRequestValidator().TestValidate(request).ShouldHaveValidationErrors();
  }
}

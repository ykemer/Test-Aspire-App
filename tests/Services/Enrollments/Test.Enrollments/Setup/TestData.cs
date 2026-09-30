using Service.Enrollments.Common.Database.Entities;

namespace Test.Enrollments.Setup;

/// <summary>
/// Small factories for valid test entities. Tests change only the fields they care about.
/// </summary>
public static class TestData
{
  public static Class OpenClass(Guid? courseId = null, int maxStudents = 10, int enrolledCount = 0) =>
    new()
    {
      CourseId = courseId ?? Guid.NewGuid(),
      MaxStudents = maxStudents,
      EnrolledCount = enrolledCount,
      RegistrationDeadline = TestClock.Now.AddDays(1),
      CourseStartDate = TestClock.Now.AddDays(2),
      CourseEndDate = TestClock.Now.AddDays(3)
    };

  public static Enrollment EnrollmentIn(Class courseClass, Guid? studentId = null) =>
    new()
    {
      CourseId = courseClass.CourseId,
      ClassId = courseClass.Id,
      StudentId = studentId ?? Guid.NewGuid(),
      StudentFirstName = "Jane",
      StudentLastName = "Doe",
      EnrollmentDateTime = TestClock.Now
    };
}

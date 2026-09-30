namespace Service.Courses.Features.Classes;

/// <summary>
/// The editable details of a class. Shared by "create class" and "update class"
/// so both are checked by the same <see cref="ClassDetailsValidator"/>.
/// </summary>
public interface IClassDetails
{
  DateTime RegistrationDeadline { get; }
  DateTime CourseStartDate { get; }
  DateTime CourseEndDate { get; }
  int MaxStudents { get; }
}

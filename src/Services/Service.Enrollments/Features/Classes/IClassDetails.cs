namespace Service.Enrollments.Features.Classes;

/// <summary>
/// The class data copied from the Courses service. Shared by "create class" and "update class"
/// so both are checked by the same <see cref="ClassDetailsValidator"/>.
/// </summary>
public interface IClassDetails
{
  Guid Id { get; }
  Guid CourseId { get; }
  int MaxStudents { get; }
  DateTime RegistrationDeadline { get; }
  DateTime CourseStartDate { get; }
  DateTime CourseEndDate { get; }
}

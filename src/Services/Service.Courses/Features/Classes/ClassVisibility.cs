using System.Linq.Expressions;

using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes;

/// <summary>
/// The single place that decides which classes a student may see.
/// A class is visible when the student is enrolled in it,
/// or when registration is still open and the class still has free seats.
/// </summary>
public static class ClassVisibility
{
  public static IQueryable<Class> OnlyVisibleClasses(this IQueryable<Class> classes,
    IReadOnlyCollection<Guid> enrolledClassIds, DateTime now) =>
    classes.Where(IsVisible(enrolledClassIds, now));

  /// <summary>
  /// Keeps only courses that have at least one visible class.
  /// </summary>
  public static IQueryable<Course> OnlyCoursesWithVisibleClasses(this IQueryable<Course> courses,
    IReadOnlyCollection<Guid> enrolledClassIds, DateTime now)
  {
    var isVisible = IsVisible(enrolledClassIds, now);
    return courses.Where(course => course.CourseClasses.AsQueryable().Any(isVisible));
  }

  private static Expression<Func<Class, bool>> IsVisible(IReadOnlyCollection<Guid> enrolledClassIds, DateTime now) =>
    courseClass =>
      enrolledClassIds.Contains(courseClass.Id) ||
      (courseClass.RegistrationDeadline > now && courseClass.TotalStudents < courseClass.MaxStudents);
}

namespace Service.Courses.Features.Courses;

/// <summary>
/// Size limits for course text fields. Used by both the validators and the database columns,
/// so the two can never disagree.
/// </summary>
public static class CourseLimits
{
  public const int NameMinLength = 3;
  public const int NameMaxLength = 100;
  public const int DescriptionMinLength = 3;
  public const int DescriptionMaxLength = 500;
  public const int SearchQueryMaxLength = 200;
}

namespace Service.Students.Features;

/// <summary>
/// Size limits for student text fields. Used by both the validators and the database columns,
/// so the two can never disagree.
/// </summary>
public static class StudentLimits
{
  public const int NameMaxLength = 100;
  public const int EmailMaxLength = 100;
}

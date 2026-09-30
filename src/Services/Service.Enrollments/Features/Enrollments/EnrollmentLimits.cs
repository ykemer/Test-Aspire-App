namespace Service.Enrollments.Features.Enrollments;

/// <summary>
/// Size limits for enrollment text fields. Used by both the validators and the database columns,
/// so the two can never disagree.
/// </summary>
public static class EnrollmentLimits
{
  public const int NameMaxLength = 100;
}

namespace Platform.Common.StateMachines;

/// <summary>
/// The steps of the enroll and unenroll sagas, stored in the saga data so the progress is visible.
/// </summary>
public static class SagaStates
{
  public const string Initial = "Initial";
  public const string ChangingStudentEnrollmentsCount = "ChangingStudentEnrollmentsCount";
  public const string ChangingClassEnrollmentsCount = "ChangingClassEnrollmentsCount";
  public const string Completed = "Completed";
  public const string Failed = "Failed";
}

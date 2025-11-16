namespace Contracts.Courses.Hub;

public static class CourseHubMessage
{
  public static string CourseCreated = "CourseCreated";
  public static string CourseCreateRequestRejected = "CourseCreateRequestRejected";

  public static string CourseUpdated = "CourseUpdated";
  public static string CourseUpdateRequestRejected = "CourseUpdateRequestRejected";

  public static string CourseDeleteRequestRejected = "CourseDeleteRequestRejected";
  public static string CourseDeleted = "CourseDeleted";
}

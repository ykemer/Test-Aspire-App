namespace Library.Infrastructure;

/// <summary>
/// URLs of the SignalR hubs. The frontend connects to these to receive live notifications.
/// </summary>
public static class HubRoutes
{
  public const string Enrollments = "/enrollmentHub";
  public const string Courses = "/courseHub";
  public const string Classes = "/classHub";

  public static readonly string[] All = [Enrollments, Courses, Classes];
}

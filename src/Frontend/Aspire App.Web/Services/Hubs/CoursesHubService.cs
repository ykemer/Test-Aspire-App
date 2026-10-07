using Aspire_App.Web.Helpers;

using Contracts.Courses.Hub;

using Library.Infrastructure;

using Microsoft.AspNetCore.SignalR.Client;

using IAuthenticationService = Aspire_App.Web.Services.Auth.IAuthenticationService;


namespace Aspire_App.Web.Services.Hubs;

public class CoursesHubService : AbstractHubService
{
  private static readonly string[] s_courseEvents =
  [
    CourseHubMessage.CourseCreated,
    CourseHubMessage.CourseCreateRequestRejected,
    CourseHubMessage.CourseUpdated,
    CourseHubMessage.CourseUpdateRequestRejected,
    CourseHubMessage.CourseDeleted,
    CourseHubMessage.CourseDeleteRequestRejected
  ];

  public CoursesHubService(IConfiguration configuration, IAuthenticationService authService) : base(configuration,
    authService, HubRoutes.Courses)
  {
  }

  public event Action<CourseMessage>? OnCourseNotification;

  protected override void RegisterHandlers()
  {
    if (HubConnection == null)
    {
      return;
    }

    foreach (var eventName in s_courseEvents)
    {
      HubConnection.On<string>(eventName, message =>
        OnCourseNotification?.Invoke(new CourseMessage(eventName, message))
      );
    }
  }
}

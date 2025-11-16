using Aspire_App.Web.Helpers;

using Contracts.Courses.Hub;

using Microsoft.AspNetCore.SignalR.Client;

using IAuthenticationService = Aspire_App.Web.Services.Auth.IAuthenticationService;


namespace Aspire_App.Web.Services.Hubs;

public class CoursesHubService
{
  private HubConnection? _hubConnection;
  private readonly string _hubUrl;

  private readonly IAuthenticationService _authenticationService;

  public CoursesHubService(IConfiguration configuration, IAuthenticationService authService)
  {
    var platformServiceUrl = configuration["services:platformService:https:0"];
    _hubUrl = platformServiceUrl + "/courseHub";
    _authenticationService = authService;
  }

  public event Action<CourseMessage>? OnCourseNotification;

  public async Task StartConnectionAsync()
  {
    if (_hubConnection == null)
    {
      _hubConnection = new HubConnectionBuilder()
        .WithUrl(_hubUrl, options =>
        {
          options.AccessTokenProvider = () => _authenticationService.GetJwtAsync().AsTask();
        })
        .Build();

      _hubConnection.On<string>(CourseHubMessage.CourseCreated, (message) =>
      {
        OnCourseNotification?.Invoke(new CourseMessage(CourseHubMessage.CourseCreated, message));
      });
      _hubConnection.On<string>(CourseHubMessage.CourseCreateRequestRejected, (message) =>
      {
        OnCourseNotification?.Invoke(new CourseMessage(CourseHubMessage.CourseCreateRequestRejected, message));
      });
      _hubConnection.On<string>(CourseHubMessage.CourseDeleted, (message) =>
      {
        OnCourseNotification?.Invoke(new CourseMessage(CourseHubMessage.CourseDeleted, message));
      });
      _hubConnection.On<string>(CourseHubMessage.CourseDeleteRequestRejected, (message) =>
      {
        OnCourseNotification?.Invoke(new CourseMessage(CourseHubMessage.CourseDeleteRequestRejected, message));
      });
    }

    if (_hubConnection.State == HubConnectionState.Disconnected)
    {
      await _hubConnection.StartAsync();
    }
  }

  public async Task StopConnectionAsync()
  {
    if (_hubConnection != null)
    {
      await _hubConnection.StopAsync();
      await _hubConnection.DisposeAsync();
      _hubConnection = null;
    }
  }
}

using Aspire_App.Web.Helpers;

using Contracts.Hub;

using Microsoft.AspNetCore.SignalR.Client;

using IAuthenticationService = Aspire_App.Web.Services.Auth.IAuthenticationService;


namespace Aspire_App.Web.Services.Hubs;

public class EnrollmentHubService
{
  private HubConnection? _hubConnection;
  private readonly string _hubUrl;

  private readonly IAuthenticationService _authenticationService;

  public EnrollmentHubService(IConfiguration configuration, IAuthenticationService authService)
  {
    var platformServiceUrl = configuration["services:platformService:https:0"];
    _hubUrl = platformServiceUrl + "/enrollmentHub";
    _authenticationService = authService;
  }

  public event Action<EnrollmentMessage>? OnEnrollmentNotification;

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

      _hubConnection.On<string>(EnrollmentHubMessages.EnrollmentCreated, (message) =>
      {
        OnEnrollmentNotification?.Invoke(new EnrollmentMessage(EnrollmentHubMessages.EnrollmentCreated, message));
      });
      _hubConnection.On<string>(EnrollmentHubMessages.EnrollmentCreateRequestRejected, (message) =>
      {
        OnEnrollmentNotification?.Invoke(new EnrollmentMessage(EnrollmentHubMessages.EnrollmentCreateRequestRejected, message));
      });
      _hubConnection.On<string>(EnrollmentHubMessages.EnrollmentDeleted, (message) =>
      {
        OnEnrollmentNotification?.Invoke(new EnrollmentMessage(EnrollmentHubMessages.EnrollmentDeleted, message));
      });
      _hubConnection.On<string>(EnrollmentHubMessages.EnrollmentDeleteRequestRejected, (message) =>
      {
        OnEnrollmentNotification?.Invoke(new EnrollmentMessage(EnrollmentHubMessages.EnrollmentDeleteRequestRejected, message));
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

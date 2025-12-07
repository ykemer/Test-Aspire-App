using Aspire_App.Web.Services.Auth;

using Microsoft.AspNetCore.SignalR.Client;

namespace Aspire_App.Web.Services.Hubs;

public abstract class AbstractHubService
{
  protected HubConnection? HubConnection;
  private readonly string _hubUrl;
  private readonly IAuthenticationService _authenticationService;

  protected AbstractHubService(IConfiguration configuration, IAuthenticationService authenticationService,
    string hubEndpoint)
  {
    _authenticationService = authenticationService;
    var platformServiceUrl = configuration["services:platformService:https:0"];
    _hubUrl = platformServiceUrl + hubEndpoint;
  }

  public async Task StartConnectionAsync()
  {
    if (HubConnection == null)
    {
      HubConnection = new HubConnectionBuilder()
        .WithUrl(_hubUrl, options =>
        {
          options.AccessTokenProvider = () => _authenticationService.GetJwtAsync().AsTask()!;
        })
        .Build();

      RegisterHandlers();
    }

    if (HubConnection.State == HubConnectionState.Disconnected)
    {
      await HubConnection.StartAsync();
    }
  }

  public async Task StopConnectionAsync()
  {
    if (HubConnection != null)
    {
      await HubConnection.StopAsync();
      await HubConnection.DisposeAsync();
      HubConnection = null;
    }
  }

  protected abstract void RegisterHandlers();
}

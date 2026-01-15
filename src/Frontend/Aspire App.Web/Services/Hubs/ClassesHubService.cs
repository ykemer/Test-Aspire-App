using Aspire_App.Web.Helpers;

using Contracts.Classes.Hub;

using Microsoft.AspNetCore.SignalR.Client;

using IAuthenticationService = Aspire_App.Web.Services.Auth.IAuthenticationService;


namespace Aspire_App.Web.Services.Hubs;

public class ClassesHubService : AbstractHubService
{
  private static readonly string[] s_classEvents =
  [
    ClassHubMessage.ClassCreated,
    ClassHubMessage.ClassCreateRequestRejected,
    ClassHubMessage.ClassUpdated,
    ClassHubMessage.ClassUpdateRequestRejected,
    ClassHubMessage.ClassDeleted,
    ClassHubMessage.ClassDeleteRequestRejected
  ];

  public ClassesHubService(IConfiguration configuration, IAuthenticationService authenticationService) : base(
    configuration, authenticationService, "/classHub")
  {
  }

  public event Action<ClassMessage>? OnClassNotification;


  protected override void RegisterHandlers()
  {
    if (HubConnection == null)
    {
      return;
    }

    foreach (var eventName in s_classEvents)
    {
      HubConnection.On<string>(eventName, message =>
        OnClassNotification?.Invoke(new ClassMessage(eventName, message))
      );
    }
  }
}

using Aspire_App.Web.Helpers;

using Contracts.Enrollments.Hub;

using Microsoft.AspNetCore.SignalR.Client;

using IAuthenticationService = Aspire_App.Web.Services.Auth.IAuthenticationService;

namespace Aspire_App.Web.Services.Hubs;

public class EnrollmentHubService : AbstractHubService
{

  private static readonly string[] s_enrollmentEvents =
  [
    EnrollmentHubMessages.EnrollmentCreated,
    EnrollmentHubMessages.EnrollmentCreateRequestRejected,
    EnrollmentHubMessages.EnrollmentDeleted,
    EnrollmentHubMessages.EnrollmentDeleteRequestRejected,
  ];

  public EnrollmentHubService(IConfiguration configuration, IAuthenticationService authenticationService): base(configuration, authenticationService, "/enrollmentHub")
  {
  }

  public event Action<EnrollmentMessage>? OnEnrollmentNotification;


  protected override void RegisterHandlers()
  {
    if (HubConnection == null) return;

    foreach (var eventName in s_enrollmentEvents)
    {
      HubConnection.On<string>(eventName, message =>
        OnEnrollmentNotification?.Invoke(new EnrollmentMessage(eventName, message))
      );
    }
  }
}

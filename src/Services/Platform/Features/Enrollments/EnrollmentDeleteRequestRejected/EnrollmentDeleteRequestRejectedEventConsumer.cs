using Contracts.Enrollments.Events;
using Contracts.Hub;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

using Platform.Common.Hubs;

namespace Platform.Features.Enrollments.EnrollmentDeleteRequestRejected;

public class EnrollmentDeleteRequestRejectedEventConsumer: IConsumer<EnrollmentDeleteRequestRejectedEvent>
{
  private readonly IHubContext<EnrollmentHub> _hubContext;

  public EnrollmentDeleteRequestRejectedEventConsumer(IHubContext<EnrollmentHub> hubContext)
  {
    _hubContext = hubContext;
  }

  public async Task Consume(ConsumeContext<EnrollmentDeleteRequestRejectedEvent> context)
  {
    await _hubContext.Clients.User(context.Message.StudentId).SendAsync(EnrollmentHubMessages.EnrollmentDeleteRequestRejected, $"Failed to unenroll you from the course. Please contact support.");
  }
}

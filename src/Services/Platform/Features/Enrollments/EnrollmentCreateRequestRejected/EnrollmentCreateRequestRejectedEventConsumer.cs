using Contracts.Enrollments.Events;
using Contracts.Hub;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

using Platform.Common.Hubs;

namespace Platform.Features.Enrollments.EnrollmentCreateRequestRejected;

public class EnrollmentCreateRequestRejectedEventConsumer : IConsumer<EnrollmentCreateRequestRejectedEvent>
{
  private readonly IHubContext<EnrollmentHub> _hubContext;

  public EnrollmentCreateRequestRejectedEventConsumer(IHubContext<EnrollmentHub> hubContext) =>
    _hubContext = hubContext;

  public async Task Consume(ConsumeContext<EnrollmentCreateRequestRejectedEvent> context) => await _hubContext.Clients
    .User(context.Message.StudentId).SendAsync(EnrollmentHubMessages.EnrollmentCreateRequestRejected,
      "Failed to enroll you in the course. Please contact support.");
}

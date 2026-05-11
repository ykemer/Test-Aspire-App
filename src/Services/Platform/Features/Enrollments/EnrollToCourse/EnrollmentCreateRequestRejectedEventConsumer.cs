using Contracts.Enrollments.Events;
using Contracts.Enrollments.Hub;

using Microsoft.AspNetCore.SignalR;

using Rebus.Handlers;

namespace Platform.Features.Enrollments.EnrollToCourse;

public class EnrollmentCreateRequestRejectedEventConsumer : IHandleMessages<EnrollmentCreateRequestRejectedEvent>
{
  private readonly IHubContext<EnrollmentHub> _hubContext;

  public EnrollmentCreateRequestRejectedEventConsumer(IHubContext<EnrollmentHub> hubContext) =>
    _hubContext = hubContext;

  public async Task Handle(EnrollmentCreateRequestRejectedEvent message) => await _hubContext.Clients
    .User(message.StudentId.ToString()).SendAsync(EnrollmentHubMessages.EnrollmentCreateRequestRejected,
      "Failed to enroll you in the course. Please contact support.");
}

using Contracts.Enrollments.Events;
using Contracts.Enrollments.Hub;

using Microsoft.AspNetCore.SignalR;

using Rebus.Handlers;

namespace Platform.Features.Enrollments.UnenrollFromCourse;

public class EnrollmentDeleteRequestRejectedEventConsumer : IHandleMessages<EnrollmentDeleteRequestRejectedEvent>
{
  private readonly IHubContext<EnrollmentHub> _hubContext;

  public EnrollmentDeleteRequestRejectedEventConsumer(IHubContext<EnrollmentHub> hubContext) =>
    _hubContext = hubContext;

  public async Task Handle(EnrollmentDeleteRequestRejectedEvent message) => await _hubContext.Clients
    .User(message.StudentId.ToString()).SendAsync(EnrollmentHubMessages.EnrollmentDeleteRequestRejected,
      "Failed to unenroll you from the course. Please contact support.");
}

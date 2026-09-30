using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Platform.Features.Enrollments;

/// <summary>
/// Live notifications about enrollments ("you are enrolled", "enrollment failed").
/// Only the server sends messages (through IHubContext); clients only listen.
/// There are deliberately no hub methods: a client-callable "send to user X" method would let
/// anyone send fake notifications to anyone.
/// </summary>
[Authorize]
public class EnrollmentHub : Hub;

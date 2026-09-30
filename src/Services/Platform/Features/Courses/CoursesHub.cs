using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Platform.Features.Courses;

/// <summary>
/// Live notifications about courses ("course created", "update rejected").
/// Only the server sends messages (through IHubContext); clients only listen.
/// There are deliberately no hub methods: a client-callable "send to user X" method would let
/// anyone send fake notifications to anyone.
/// </summary>
[Authorize]
public class CoursesHub : Hub;

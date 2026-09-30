using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Platform.Features.Classes;

/// <summary>
/// Live notifications about classes ("class created", "delete rejected").
/// Only the server sends messages (through IHubContext); clients only listen.
/// There are deliberately no hub methods: a client-callable "send to user X" method would let
/// anyone send fake notifications to anyone.
/// </summary>
[Authorize]
public class ClassesHub : Hub;

﻿namespace Contracts.Users.Events;

public record UserCreatedEvent
{
  public string Id { get; set; }
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string Email { get; set; }
  public DateTime DateOfBirth { get; set; }
}

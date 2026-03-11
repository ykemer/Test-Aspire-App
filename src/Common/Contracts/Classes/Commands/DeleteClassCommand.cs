namespace Contracts.Classes.Commands;

public class DeleteClassCommand
{
  public required Guid CourseId { get; init; }
  public required Guid ClassId { get; init; }
  public string UserId { get; init; }
}

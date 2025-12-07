namespace Contracts.Classes.Commands;

public class DeleteClassCommand
{
  public required string CourseId { get; init; }
  public required string ClassId { get; init; }
  public string UserId { get; init; }
}

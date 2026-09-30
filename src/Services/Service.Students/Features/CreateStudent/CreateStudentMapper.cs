using Contracts.Users.Events;

using Service.Students.Common.Database.Entities;

namespace Service.Students.Features.CreateStudent;

public static class CreateStudentMapper
{
  public static CreateStudentCommand ToCreateStudentCommand(this UserCreatedEvent userCreated) =>
    new()
    {
      Id = userCreated.Id,
      FirstName = userCreated.FirstName,
      LastName = userCreated.LastName,
      Email = userCreated.Email,
      DateOfBirth = userCreated.DateOfBirth
    };

  public static Student ToStudent(this CreateStudentCommand command, DateTime now) =>
    new()
    {
      Id = command.Id,
      FirstName = command.FirstName,
      LastName = command.LastName,
      Email = command.Email,
      DateOfBirth = command.DateOfBirth.Date,
      CreatedAt = now,
      UpdatedAt = now
    };
}

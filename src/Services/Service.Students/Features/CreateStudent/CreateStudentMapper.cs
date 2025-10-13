using Service.Students.Common.Database.Entities;

namespace Service.Students.Features.CreateStudent;

public static class CreateStudentMapper
{
  public static Student MapToStudent(this CreateStudentCommand command)
  {
    return new Student
    {
      Id = command.Id,
      FirstName = command.FirstName,
      LastName = command.LastName,
      Email = command.Email,
      DateOfBirth = command.DateOfBirth
    };
  }
}

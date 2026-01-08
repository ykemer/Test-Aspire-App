using FluentValidation.TestHelper;

using Service.Students.Features.CreateStudent;

namespace Test.Students.Application.Features.CreateStudent;

[TestFixture]
public class CreateStudentCommandValidatorTests
{
  [SetUp]
  public void Setup() => _validator = new CreateStudentCommandValidator();

  private CreateStudentCommandValidator _validator;

  [Test]
  public void Should_Pass_When_All_Fields_Are_Valid()
  {
    var command = new CreateStudentCommand
    {
      Id = Guid.NewGuid().ToString(),
      FirstName = "John",
      LastName = "Doe",
      Email = "john.doe@example.com",
      DateOfBirth = new DateTime(2000, 1, 1)
    };

    var result = _validator.TestValidate(command);
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Test]
  public void Should_Fail_When_Id_Is_Empty()
  {
    var command = new CreateStudentCommand
    {
      Id = Guid.Empty.ToString(),
      FirstName = "John",
      LastName = "Doe",
      Email = "john.doe@example.com",
      DateOfBirth = new DateTime(2000, 1, 1)
    };

    var result = _validator.TestValidate(command);
    result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id cannot be the empty GUID.");
  }

  [Test]
  public void Should_Fail_When_FirstName_Is_Null()
  {
    var command = new CreateStudentCommand
    {
      Id = Guid.NewGuid().ToString(),
      FirstName = null,
      LastName = "Doe",
      Email = "john.doe@example.com",
      DateOfBirth = new DateTime(2000, 1, 1)
    };

    var result = _validator.TestValidate(command);
    result.ShouldHaveValidationErrorFor(x => x.FirstName).WithErrorMessage("First Name can not be empty");
  }

  [Test]
  public void Should_Fail_When_FirstName_Is_Empty()
  {
    var command = new CreateStudentCommand
    {
      Id = Guid.NewGuid().ToString(),
      FirstName = "",
      LastName = "Doe",
      Email = "john.doe@example.com",
      DateOfBirth = new DateTime(2000, 1, 1)
    };

    var result = _validator.TestValidate(command);
    result.ShouldHaveValidationErrorFor(x => x.FirstName).WithErrorMessage("First Name can not be empty");
  }

  [Test]
  public void Should_Fail_When_LastName_Is_Null()
  {
    var command = new CreateStudentCommand
    {
      Id = Guid.NewGuid().ToString(),
      FirstName = "John",
      LastName = null,
      Email = "john.doe@example.com",
      DateOfBirth = new DateTime(2000, 1, 1)
    };

    var result = _validator.TestValidate(command);
    result.ShouldHaveValidationErrorFor(x => x.LastName).WithErrorMessage("Last Name can not be empty");
  }

  [Test]
  public void Should_Fail_When_LastName_Is_Empty()
  {
    var command = new CreateStudentCommand
    {
      Id = Guid.NewGuid().ToString(),
      FirstName = "John",
      LastName = "",
      Email = "john.doe@example.com",
      DateOfBirth = new DateTime(2000, 1, 1)
    };

    var result = _validator.TestValidate(command);
    result.ShouldHaveValidationErrorFor(x => x.LastName).WithErrorMessage("Last Name can not be empty");
  }

  [Test]
  public void Should_Fail_When_Email_Is_Null()
  {
    var command = new CreateStudentCommand
    {
      Id = Guid.NewGuid().ToString(),
      FirstName = "John",
      LastName = "Doe",
      Email = null,
      DateOfBirth = new DateTime(2000, 1, 1)
    };

    var result = _validator.TestValidate(command);
    result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email can not be empty");
  }

  [Test]
  public void Should_Fail_When_Email_Is_Empty()
  {
    var command = new CreateStudentCommand
    {
      Id = Guid.NewGuid().ToString(),
      FirstName = "John",
      LastName = "Doe",
      Email = "",
      DateOfBirth = new DateTime(2000, 1, 1)
    };

    var result = _validator.TestValidate(command);
    result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email can not be empty");
  }

  [Test]
  public void Should_Fail_When_Email_Is_Invalid()
  {
    var command = new CreateStudentCommand
    {
      Id = Guid.NewGuid().ToString(),
      FirstName = "John",
      LastName = "Doe",
      Email = "invalid-email",
      DateOfBirth = new DateTime(2000, 1, 1)
    };

    var result = _validator.TestValidate(command);
    result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Wrong email format");
  }

  [Test]
  public void Should_Fail_When_DateOfBirth_Is_Under_18()
  {
    var command = new CreateStudentCommand
    {
      Id = Guid.NewGuid().ToString(),
      FirstName = "John",
      LastName = "Doe",
      Email = "john.doe@example.com",
      DateOfBirth = new DateTime(DateTime.Now.Year - 17, 1, 1)
    };

    var result = _validator.TestValidate(command);
    result.ShouldHaveValidationErrorFor(x => x.DateOfBirth).WithErrorMessage("Student must be at least 18 years old");
  }

  [Test]
  public void Should_Fail_When_DateOfBirth_Is_Over_100()
  {
    var command = new CreateStudentCommand
    {
      Id = Guid.NewGuid().ToString(),
      FirstName = "John",
      LastName = "Doe",
      Email = "john.doe@example.com",
      DateOfBirth = new DateTime(DateTime.Now.Year - 101, 1, 1)
    };

    var result = _validator.TestValidate(command);
    result.ShouldHaveValidationErrorFor(x => x.DateOfBirth).WithErrorMessage("Wrong date");
  }
}

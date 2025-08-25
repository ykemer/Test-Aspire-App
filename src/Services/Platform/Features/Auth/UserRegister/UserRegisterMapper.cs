using Contracts.Users.Events;
using Contracts.Users.Requests;

using Platform.Database.Entities;

namespace Platform.Features.Auth.UserRegister;

public static class UserRegisterMapper
{
  public static ApplicationUser ToApplicationUser(this UserRegisterRequest request) =>
    new()
    {
      UserName = request.Email,
      Email = request.Email,
      FirstName = request.FirstName,
      LastName = request.LastName,
      DateOfBirth = request.DateOfBirth
    };

  public static UserCreatedEvent ToUserCreatedEvent(this ApplicationUser user) =>
    new()
    {
      Id = user.Id,
      FirstName = user.FirstName,
      LastName = user.LastName,
      DateOfBirth = user.DateOfBirth,
      Email = user.Email!
    };
}

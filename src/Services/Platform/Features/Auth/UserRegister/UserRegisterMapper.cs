using Contracts.Users.Events;
using Contracts.Users.Requests;

using Microsoft.AspNetCore.Identity;

using Platform.Common.Auth;
using Platform.Common.Database.Entities;

namespace Platform.Features.Auth.UserRegister;

public static class UserRegisterMapper
{
  private static readonly string[] s_duplicateAccountErrorCodes =
  [
    nameof(IdentityErrorDescriber.DuplicateUserName), nameof(IdentityErrorDescriber.DuplicateEmail)
  ];

  public static ApplicationUser ToApplicationUser(this UserRegisterRequest request) =>
    new()
    {
      UserName = request.Email,
      Email = request.Email,
      FirstName = request.FirstName,
      LastName = request.LastName,
      DateOfBirth = request.DateOfBirth.Date
    };

  public static UserCreatedEvent ToUserCreatedEvent(this ApplicationUser user) =>
    new()
    {
      Id = Guid.Parse(user.Id),
      FirstName = user.FirstName,
      LastName = user.LastName,
      DateOfBirth = user.DateOfBirth,
      Email = user.Email!
    };

  /// <summary>
  /// Turns ASP.NET Identity errors into API errors: "email already used" becomes 409, everything else
  /// (e.g. a password rule) becomes a 400 validation error with Identity's own, user-friendly message.
  /// </summary>
  public static List<Error> ToApiErrors(this IEnumerable<IdentityError> identityErrors)
  {
    var errors = identityErrors.ToList();
    if (errors.Any(error => s_duplicateAccountErrorCodes.Contains(error.Code)))
    {
      return [AuthErrors.EmailAlreadyRegistered];
    }

    return errors.Select(error => Error.Validation($"platform.auth.{error.Code}", error.Description)).ToList();
  }
}

using Contracts.Users.Responses;

using FastEndpoints;

using Microsoft.AspNetCore.Identity;

using Platform.Common.Database.Entities;
using Platform.Common.Services.User;

namespace Platform.Features.Auth.WhoAmI;

public class WhoAmIEndpoint : EndpointWithoutRequest<ErrorOr<UserInfoResponse>>
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IUserService _userService;

  public WhoAmIEndpoint(UserManager<ApplicationUser> userManager, IUserService userService)
  {
    _userManager = userManager;
    _userService = userService;
  }

  public override void Configure()
  {
    Get("/api/auth/whoami");
    Policies("RequireUserRole");
    Description(x => x.WithTags("Auth"));
  }

  public override async Task<ErrorOr<UserInfoResponse>> ExecuteAsync(CancellationToken cancellationToken)
  {
    var id = _userService.GetUserId(User).ToString();
    if(string.IsNullOrEmpty(id))
      return Error.Unauthorized("User not found");

    var user = await _userManager.FindByIdAsync(id);

    if (user is null)
      return Error.Unauthorized("User not found");

    return new UserInfoResponse
    {
      Id = user.Id,
      FirstName = user.FirstName,
      Email = user.Email!,
      LastName = user.LastName
    };
  }
}

﻿using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

using Platform.Entities;

namespace Platform.Services.User;

public class AppClaimsFactory : IUserClaimsPrincipalFactory<ApplicationUser>
{
  public Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
  {
    var claims = new Claim[]
    {
      new(ClaimTypes.Email, user.Email ?? ""), new(ClaimTypes.Name, user.FirstName),
      new(ClaimTypes.Surname, user.LastName), new(ClaimTypes.Sid, user.Id), new(ClaimTypes.Authentication, "true")
    };
    ClaimsIdentity? claimsIdentity = new(claims, "Bearer");

    ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

    return Task.FromResult(claimsPrincipal);
  }

  // public async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
  // {
  //     var roles = await _signInManager.UserManager.GetRolesAsync(user);
  //     var claims = new Claim[] {
  //         new(ClaimTypes.Email, user.Email ?? ""),
  //         new(ClaimTypes.Name, user.FirstName),
  //         new(ClaimTypes.Surname, user.LastName),
  //         new(ClaimTypes.Sid, user.Id),
  //         new(ClaimTypes.Authentication, "true"),
  //         new(ClaimTypes.Role, roles.FirstOrDefault()!),
  //
  //     };
  //     var claimsIdentity = new ClaimsIdentity(claims, "Bearer");
  //
  //     var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
  //
  //     return claimsPrincipal;
  // }
}

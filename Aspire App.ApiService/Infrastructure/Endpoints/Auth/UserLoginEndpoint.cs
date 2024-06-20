using Aspire_App.ApiService.Domain.Models;
using Aspire_App.ApiService.Infrastructure.Services;
using FastEndpoints;
using Library.Auth;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Auth
{
    public class UserLoginEndpoint : Endpoint<LoginRequest, IResult>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtService _jwtService;

        public UserLoginEndpoint(SignInManager<ApplicationUser> signInManager, JwtService jwtService)
        {
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        public override void Configure()
        {
            Post("/api/auth/login");
            AllowAnonymous();
        }

        public override async Task<IResult> ExecuteAsync(LoginRequest req, CancellationToken ct)
        {
            var result = await _signInManager.PasswordSignInAsync(req.Email, req.Password, false, false);
            if (!result.Succeeded)
                return Results.Problem("Login failed", statusCode: StatusCodes.Status401Unauthorized);
            
            var user = await _signInManager.UserManager.FindByEmailAsync(req.Email);
                
          
            var jwtTokenResponse = await _jwtService.GenerateJwtToken(user);
            
            if(user.RefreshToken == null)
            {
                user.RefreshToken = Generators.GenerateToken(64);
                await _signInManager.UserManager.UpdateAsync(user);
            }
                
            var accessTokenResponse = new AccessTokenResponse
            {
                AccessToken = jwtTokenResponse.AccessToken,
                ExpiresIn = jwtTokenResponse.ExpiresIn,
                RefreshToken = user.RefreshToken ?? ""
            };
            return Results.Ok(accessTokenResponse);


        }
    }
}

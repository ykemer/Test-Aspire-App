using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Aspire_App.ApiService.Infrastructure.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var tokenStr = authHeader.ToString().Split(' ')[1];
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenStr);

            var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
            if (userIdClaim != null) context.Items["UserId"] = userIdClaim.Value;
        }

        await _next(context);
    }
}
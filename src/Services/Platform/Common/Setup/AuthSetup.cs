using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using Platform.Common.Auth;
using Platform.Common.Database;
using Platform.Common.Database.Entities;

namespace Platform.Common.Setup;

public static class AuthSetup
{
  public static IServiceCollection AddAuth(this IServiceCollection services)
  {
    services.AddOptions<JwtOptions>()
      .Configure<IConfiguration>((options, configuration) => options.ReadFrom(configuration))
      .ValidateDataAnnotations()
      .ValidateOnStart();

    services
      .AddIdentity<ApplicationUser, IdentityRole>(options => PasswordRules.ApplyTo(options.Password))
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders();

    services
      .AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer();

    // Configured here (not inside AddJwtBearer) so it can use the validated JwtOptions.
    services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
      .Configure<IOptions<JwtOptions>>((bearer, jwt) =>
      {
        bearer.TokenValidationParameters = jwt.Value.CreateValidationParameters();
        bearer.Events = new JwtBearerEvents { OnMessageReceived = ReadHubTokenFromQueryString };
      });

    services.AddAuthorizationBuilder()
      .AddPolicy(Policies.Administrators, policy => policy.RequireRole(Roles.Administrator))
      .AddPolicy(Policies.Users, policy => policy.RequireRole(Roles.User, Roles.Administrator));

    services.AddScoped<IAccessTokenFactory, AccessTokenFactory>();
    services.AddScoped<IAuthTokenService, AuthTokenService>();
    return services;
  }

  /// <summary>
  /// Browsers cannot send an Authorization header when opening a WebSocket, so SignalR clients may send
  /// the access token in the "access_token" query string instead. Accept it only for hub URLs.
  /// </summary>
  private static Task ReadHubTokenFromQueryString(MessageReceivedContext context)
  {
    var token = context.Request.Query["access_token"].ToString();
    var isHubRequest = HubRoutes.All.Any(route => context.HttpContext.Request.Path.StartsWithSegments(route));

    if (isHubRequest && !string.IsNullOrEmpty(token))
    {
      context.Token = token;
    }

    return Task.CompletedTask;
  }
}

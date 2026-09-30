using FastEndpoints;
using FastEndpoints.Swagger;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Platform.Common.Auth;
using Platform.Common.Database;

namespace Platform.Common.Setup;

public static class ApiSetup
{
  public static IServiceCollection AddApi(this IServiceCollection services)
  {
    services.TryAddSingleton(TimeProvider.System);
    services.AddScoped<ApplicationDbContextInitializer>();

    services
      .AddFastEndpoints()
      .SwaggerDocument(options =>
      {
        options.AutoTagPathSegmentIndex = 0;
        options.DocumentSettings = settings =>
        {
          settings.Title = "Platform API";
          settings.Version = "v1";
        };
        options.TagDescriptions = tags =>
        {
          tags["Auth"] = "Sign up, sign in and tokens";
          tags["Classes"] = "Operations on classes";
          tags["Courses"] = "Operations on courses";
          tags["Students"] = "Operations on students";
          tags["Enrollments"] = "Enrolling students in classes";
        };
      });

    services.AddSignalR();
    services.AddSingleton<IUserIdProvider, UserIdProvider>();
    return services;
  }
}

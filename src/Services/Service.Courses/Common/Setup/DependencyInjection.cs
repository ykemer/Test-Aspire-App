using Library.Middleware;

using Service.Courses.Common.Database;

namespace Service.Courses.Common.Setup;

public static class DependencyInjection
{
  public static IServiceCollection AddServices(this IServiceCollection services)
  {
    services.AddGrpc();
    services.AddScoped<ApplicationDbContextInitializer>();
    services.AddMediator(options =>
    {
      options.ServiceLifetime = ServiceLifetime.Scoped;
      options.Assemblies = [typeof(DependencyInjection).Assembly];
      options.PipelineBehaviors = [typeof(LoggingBehaviour<,>), typeof(ValidationBehavior<,>), typeof(ExceptionHandlingBehaviour<,>)];
    });

    return services;
  }
}

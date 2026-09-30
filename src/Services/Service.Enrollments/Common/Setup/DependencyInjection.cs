using FluentValidation;

using Library.GRPC.InternalApi;
using Library.Middleware;

using Microsoft.Extensions.DependencyInjection.Extensions;

using Service.Enrollments.Common.Database;

namespace Service.Enrollments.Common.Setup;

public static class DependencyInjection
{
  public static IServiceCollection AddServices(this IServiceCollection services)
  {
    services.AddGrpcForInternalCallers();
    services.TryAddSingleton(TimeProvider.System);
    services.AddScoped<ApplicationDbContextInitializer>();

    // Without this line the ValidationBehavior finds no validator and silently skips validation.
    services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, ServiceLifetime.Scoped);

    services.AddMediator(options =>
    {
      options.ServiceLifetime = ServiceLifetime.Scoped;
      // Must stay a literal typeof(...).Assembly: the Mediator source generator reads it at compile time
      // and cannot follow a variable.
      options.Assemblies = [typeof(DependencyInjection).Assembly];
      options.PipelineBehaviors =
        [typeof(LoggingBehaviour<,>), typeof(ValidationBehavior<,>), typeof(ExceptionHandlingBehaviour<,>)];
    });

    return services;
  }
}

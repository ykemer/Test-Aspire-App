﻿using Library.Middleware;

namespace Service.Enrollments;

public static class DependencyInjection
{
  public static IServiceCollection AddServices(this IServiceCollection services)
  {
    services.AddGrpc();
    services.AddScoped<ApplicationDbContextInitializer>();
    services.AddMediator(options =>
    {
      options.ServiceLifetime = ServiceLifetime.Scoped;
      options.Assemblies = [typeof(DependencyInjection)];
      options.PipelineBehaviors = [typeof(LoggingBehaviour<,>)];
    });

    return services;
  }
}

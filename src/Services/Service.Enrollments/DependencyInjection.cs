using Library.Middleware;

namespace Service.Enrollments;

public static class DependencyInjection
{
  public static IServiceCollection AddServices(this IServiceCollection services)
  {
    services.AddGrpc();
    services.AddScoped<ApplicationDbContextInitializer>();
    services.AddMediatR(options =>
    {
      options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
      options.AddOpenRequestPostProcessor(typeof(LoggingBehaviour<,>));
      options.AutoRegisterRequestProcessors = false;
    });

    return services;
  }
}

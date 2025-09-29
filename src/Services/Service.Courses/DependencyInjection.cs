using _Application._Common.Behaviours;

using Library.Middleware;

using MassTransit;

namespace Service.Courses;

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
      options.PipelineBehaviors = [
        typeof(ValidationBehavior<,>),
        typeof(LoggingBehaviour<,>)
      ];
    });

    services.Configure<MassTransitHostOptions>(options =>
    {
      options.WaitUntilStarted = true;
    });

    services.AddMassTransit(configure =>
    {
      var assembly = typeof(DependencyInjection).Assembly;
      var queue = "queue-courses";

      configure.SetKebabCaseEndpointNameFormatter();
      configure.AddConsumers(assembly);

      configure.UsingRabbitMq((context, cfg) =>
      {
        var configService = context.GetRequiredService<IConfiguration>();
        var connectionString = configService.GetConnectionString("messaging");
        cfg.Host(connectionString);
        cfg.ReceiveEndpoint(queue, e =>
        {
          e.ConfigureConsumers(context);
        });
        cfg.ConfigureEndpoints(context);
      });
    });

    return services;
  }
}

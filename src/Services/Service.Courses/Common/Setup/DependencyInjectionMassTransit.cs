using MassTransit;

using Service.Courses.Common.Database;

namespace Service.Courses.Common.Setup;

public static class DependencyInjectionMassTransit
{
  public static IServiceCollection AddMassTransitServices(this IServiceCollection services)
  {
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


      configure.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
      {
        o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
        o.UsePostgres();
        // o.UseBusOutbox();
      });

      configure.AddConfigureEndpointsCallback((context, name, cfg) =>
      {
        cfg.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
      });

      configure.UsingRabbitMq((context, cfg) =>
      {
        var configService = context.GetRequiredService<IConfiguration>();
        var connectionString = configService.GetConnectionString("messaging");
        cfg.Host(connectionString);

        cfg.ReceiveEndpoint(queue, e =>
        {
          e.ConfigureConsumers(context);
          e.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
        });
      });
    });

    return services;
  }
}

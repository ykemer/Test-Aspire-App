using System.Reflection;

using MassTransit;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Infrastructure;

public static class AsyncMessagesSetup
{
  public static IServiceCollection AddMassTransitServices(this IServiceCollection services, Assembly assembly,
    string queue)
  {
    services.Configure<MassTransitHostOptions>(options =>
    {
      options.WaitUntilStarted = true;
    });

    services.AddMassTransit(configure =>
    {
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

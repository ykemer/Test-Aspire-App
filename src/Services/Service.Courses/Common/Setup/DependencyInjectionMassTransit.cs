using Contracts.Classes.Events.DecreaseClassEnrollmentsCount;
using Contracts.Classes.Events.IncreaseClassEnrollmentsCount;

using Rebus.Config;
using Rebus.RabbitMq;
using Rebus.ServiceProvider;

namespace Service.Courses.Common.Setup;

public static class DependencyInjectionMassTransit
{
  public static IServiceCollection AddRebusServices(this IServiceCollection services, IConfiguration config)
  {
    var rabbitConn = config.GetConnectionString("messaging")!;

    services.AddRebus(
      (configure, _) => configure
        .Transport(t => t.UseRabbitMq(rabbitConn, "queue-courses"))
        .Options(o => o.SetNumberOfWorkers(1)),
      onCreated: async bus =>
      {
        await bus.Subscribe<IncreaseClassEnrollmentsCountEvent>();
        await bus.Subscribe<DecreaseClassEnrollmentsCountEvent>();
      }
    );

    services.AutoRegisterHandlersFromAssemblyOf<Program>();
    return services;
  }
}

using Contracts.Students.Events.DecreaseStudentEnrollmentCount;
using Contracts.Students.Events.IncreaseStudentEnrollmentsCount;
using Contracts.Users.Events;

using Rebus.Config;
using Rebus.RabbitMq;
using Rebus.ServiceProvider;

namespace Service.Students.Common.Setup;

public static class DependencyInjectionMassTransit
{
  public static IServiceCollection AddRebusServices(this IServiceCollection services, IConfiguration config)
  {
    var rabbitConn = config.GetConnectionString("messaging")!;

    services.AddRebus(
      (configure, _) => configure
        .Transport(t => t.UseRabbitMq(rabbitConn, "queue-students"))
        .Options(o => o.SetNumberOfWorkers(1)),
      onCreated: async bus =>
      {
        await bus.Subscribe<UserCreatedEvent>();
        await bus.Subscribe<IncreaseStudentEnrollmentsCountEvent>();
        await bus.Subscribe<DecreaseStudentEnrollmentCountEvent>();
      }
    );

    services.AutoRegisterHandlersFromAssemblyOf<Program>();
    return services;
  }
}

using Contracts.Classes.Events;
using Contracts.Courses.Events;

using Rebus.Config;
using Rebus.RabbitMq;
using Rebus.ServiceProvider;

namespace Service.Enrollments.Common.Setup;

public static class DependencyInjectionMassTransit
{
  public static IServiceCollection AddRebusServices(this IServiceCollection services, IConfiguration config)
  {
    var rabbitConn = config.GetConnectionString("messaging")!;

    services.AddRebus(
      (configure, _) => configure
        .Transport(t => t.UseRabbitMq(rabbitConn, "queue-enrollments"))
        .Options(o => o.SetNumberOfWorkers(1)),
      onCreated: async bus =>
      {
        await bus.Subscribe<ClassCreatedEvent>();
        await bus.Subscribe<ClassUpdatedEvent>();
        await bus.Subscribe<ClassDeletedEvent>();
        await bus.Subscribe<CourseDeletedEvent>();
      }
    );

    services.AutoRegisterHandlersFromAssemblyOf<Program>();
    return services;
  }
}

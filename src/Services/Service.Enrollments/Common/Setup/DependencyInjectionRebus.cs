using Contracts.Classes.Events;
using Contracts.Courses.Events;

using Rebus.Config;
using Rebus.RabbitMq;
using Rebus.ServiceProvider;

namespace Service.Enrollments.Common.Setup;

public static class DependencyInjectionRebus
{
  private const string QueueName = "queue-enrollments";
  private const string ConnectionStringName = "messaging";
  private const string NumberOfWorkersSetting = "Messaging:NumberOfWorkers";
  private const int DefaultNumberOfWorkers = 4;

  public static IServiceCollection AddRebusMessaging(this IServiceCollection services, IConfiguration configuration)
  {
    var rabbitMqConnectionString = configuration.GetConnectionString(ConnectionStringName)
                                   ?? throw new InvalidOperationException(
                                     $"Connection string '{ConnectionStringName}' is missing. " +
                                     "It is normally provided by the Aspire AppHost.");

    var numberOfWorkers = configuration.GetValue(NumberOfWorkersSetting, DefaultNumberOfWorkers);

    services.AddRebus(
      (configure, _) => configure
        .Transport(transport => transport.UseRabbitMq(rabbitMqConnectionString, QueueName))
        .Options(options => options.SetNumberOfWorkers(numberOfWorkers)),
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

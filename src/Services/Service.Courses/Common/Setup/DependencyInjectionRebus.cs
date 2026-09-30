using Contracts.Classes.Events.DecreaseClassEnrollmentsCount;
using Contracts.Classes.Events.IncreaseClassEnrollmentsCount;

using Rebus.Config;
using Rebus.RabbitMq;
using Rebus.ServiceProvider;

namespace Service.Courses.Common.Setup;

public static class DependencyInjectionRebus
{
  private const string QueueName = "queue-courses";
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
        await bus.Subscribe<IncreaseClassEnrollmentsCountEvent>();
        await bus.Subscribe<DecreaseClassEnrollmentsCountEvent>();
      }
    );

    services.AutoRegisterHandlersFromAssemblyOf<Program>();
    return services;
  }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rebus.Config;

namespace Library.Infrastructure;

public static class AsyncMessagesSetup
{
  public static IServiceCollection AddRebusServices(this IServiceCollection services, IConfiguration config,
    string queue)
  {
    var rabbitConn = config.GetConnectionString("messaging")!;

    services.AddRebus((configure, _) => configure
      .Transport(t => t.UseRabbitMq(rabbitConn, queue))
      .Options(o => o.SetNumberOfWorkers(1))
    );

    return services;
  }
}

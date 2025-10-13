using MassTransit;

using Service.Enrollments.Common.AsyncDataServices.Consumers;
using Service.Enrollments.Common.Database;

namespace Service.Enrollments.Common.Setup;

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
      var queue = "queue-enrollments";
      var deleteCommandQueue = "delete-enrollment-command";
      var createCommandQueue = "create-enrollment-command";

      configure.SetKebabCaseEndpointNameFormatter();
      configure.AddConsumers(assembly);


      configure.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
      {
        o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
        o.UsePostgres();
        o.UseBusOutbox();
      });

      configure.UsingRabbitMq((context, cfg) =>
      {
        var configService = context.GetRequiredService<IConfiguration>();
        var connectionString = configService.GetConnectionString("messaging");
        cfg.Host(connectionString);
        cfg.ReceiveEndpoint(queue, e =>
        {
          e.ConfigureConsumers(context);
        });

        cfg.ReceiveEndpoint(deleteCommandQueue, e =>
        {
          e.ConfigureConsumer<DeleteEnrollmentCommandConsumer>(context);
        });

        cfg.ReceiveEndpoint(createCommandQueue, e =>
        {
          e.ConfigureConsumer<CreateEnrollmentCommandConsumer>(context);
        });

      });
    });

    return services;
  }
}

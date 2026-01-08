using MassTransit;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Features.Enrollments.EnrollStudentToClass;
using Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

namespace Service.Enrollments.Common.Setup;

public static class DependencyInjectionMassTransit
{
  private const string queue = "queue-enrollments";
  private const string deleteEnrollmentCommandQueue = "delete-enrollment-command";
  private const string createEnrollmentCommandQueue = "create-enrollment-command";

  public static IServiceCollection AddMassTransitServices(this IServiceCollection services)
  {
    services.Configure<MassTransitHostOptions>(options =>
    {
      options.WaitUntilStarted = true;
    });

    services.AddMassTransit(configure =>
    {
      var assembly = typeof(DependencyInjection).Assembly;


      configure.SetKebabCaseEndpointNameFormatter();
      configure.AddConsumers(assembly);

      configure.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
      {
        o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
        o.UsePostgres();
        // o.UseBusOutbox(); // TODO setup the bus outbox
      });

      configure.AddConfigureEndpointsCallback((context, name, cfg) =>
      {
        cfg.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
      });

      configure.AddOptions<OutboxDeliveryServiceOptions>()
        .Configure(options =>
        {
          options.QueryDelay = TimeSpan.FromSeconds(1);
          options.QueryMessageLimit = 100;
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
          e.ConfigureDefaultDeadLetterTransport();
        });

        cfg.ReceiveEndpoint(deleteEnrollmentCommandQueue, e =>
        {
          e.ConfigureConsumer<DeleteEnrollmentCommandConsumer>(context);
          e.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
          e.ConfigureDefaultDeadLetterTransport();
        });

        cfg.ReceiveEndpoint(createEnrollmentCommandQueue, e =>
        {
          e.ConfigureConsumer<CreateEnrollmentCommandConsumer>(context);
          e.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
          e.ConfigureDefaultDeadLetterTransport();
        });
      });
    });

    return services;
  }
}

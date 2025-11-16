using MassTransit;

using Service.Courses.Common.Database;
using Service.Courses.Features.Courses.CreateCourse;
using Service.Courses.Features.Courses.DeleteCourse;
using Service.Courses.Features.Courses.UpdateCourse;

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
      var createCourseCommandQueue = "create-course-command";
      var updateCourseCommandQueue = "update-course-command";
      var deleteCourseCommandQueue = "delete-course-command";

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
        });

        cfg.ReceiveEndpoint(createCourseCommandQueue, e =>
        {
          e.ConfigureConsumer<CreateCourseCommandConsumer>(context);
          e.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
        });

        cfg.ReceiveEndpoint(updateCourseCommandQueue, e =>
        {
          e.ConfigureConsumer<UpdateCourseCommandConsumer>(context);
          e.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
        });

        cfg.ReceiveEndpoint(deleteCourseCommandQueue, e =>
        {
          e.ConfigureConsumer<DeleteCourseCommandConsumer>(context);
          e.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
        });

      });
    });

    return services;
  }
}

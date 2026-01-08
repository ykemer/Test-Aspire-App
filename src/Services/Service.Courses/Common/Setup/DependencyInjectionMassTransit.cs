using MassTransit;

using Service.Courses.Common.Database;
using Service.Courses.Features.Classes.CreateClass;
using Service.Courses.Features.Classes.DeleteClass;
using Service.Courses.Features.Classes.UpdateClass;
using Service.Courses.Features.Courses.CreateCourse;
using Service.Courses.Features.Courses.DeleteCourse;
using Service.Courses.Features.Courses.UpdateCourse;

namespace Service.Courses.Common.Setup;

public static class DependencyInjectionMassTransit
{
  private const string Queue = "queue-courses";
  private const string CreateCourseCommandQueue = "create-course-command";
  private const string UpdateCourseCommandQueue = "update-course-command";
  private const string DeleteCourseCommandQueue = "delete-course-command";


  private const string CreateClassCommandQueue = "create-class-command";
  private const string UpdateClassCommandQueue = "update-class-command";
  private const string DeleteClassCommandQueue = "delete-class-command";

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

        cfg.ReceiveEndpoint(Queue, e =>
        {
          e.ConfigureConsumers(context);
          e.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
          e.ConfigureDefaultDeadLetterTransport();
        });

        cfg.ReceiveEndpoint(CreateCourseCommandQueue, e =>
        {
          e.ConfigureConsumer<CreateCourseCommandConsumer>(context);
          e.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
          e.ConfigureDefaultDeadLetterTransport();
        });

        cfg.ReceiveEndpoint(UpdateCourseCommandQueue, e =>
        {
          e.ConfigureConsumer<UpdateCourseCommandConsumer>(context);
          e.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
          e.ConfigureDefaultDeadLetterTransport();
        });

        cfg.ReceiveEndpoint(DeleteCourseCommandQueue, e =>
        {
          e.ConfigureConsumer<DeleteCourseCommandConsumer>(context);
          e.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
          e.ConfigureDefaultDeadLetterTransport();
        });

        cfg.ReceiveEndpoint(CreateClassCommandQueue, e =>
        {
          e.ConfigureConsumer<CreateClassCommandConsumer>(context);
          e.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
          e.ConfigureDefaultDeadLetterTransport();
        });

        cfg.ReceiveEndpoint(UpdateClassCommandQueue, e =>
        {
          e.ConfigureConsumer<UpdateClassCommandConsumer>(context);
          e.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
          e.ConfigureDefaultDeadLetterTransport();
        });

        cfg.ReceiveEndpoint(DeleteClassCommandQueue, e =>
        {
          e.ConfigureConsumer<DeleteClassCommandConsumer>(context);
          e.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
          e.ConfigureDefaultDeadLetterTransport();
        });
      });
    });

    return services;
  }
}

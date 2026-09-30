using Contracts.Classes.Commands;
using Contracts.Classes.Events;
using Contracts.Classes.Events.DecreaseClassEnrollmentsCount;
using Contracts.Classes.Events.IncreaseClassEnrollmentsCount;
using Contracts.Courses.Commands;
using Contracts.Courses.Events;
using Contracts.Enrollments.Commands;
using Contracts.Enrollments.Events;
using Contracts.Students.Events.DecreaseStudentEnrollmentCount;
using Contracts.Students.Events.IncreaseStudentEnrollmentsCount;

using Rebus.Config;
using Rebus.PostgreSql.Sagas;
using Rebus.RabbitMq;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;

namespace Platform.Common.Setup;

public static class RebusSetup
{
  private const string QueueName = "queue-platform";
  private const string CoursesQueue = "queue-courses";
  private const string EnrollmentsQueue = "queue-enrollments";
  private const string NumberOfWorkersSetting = "Messaging:NumberOfWorkers";
  private const int DefaultNumberOfWorkers = 4;

  public static IServiceCollection AddRebusMessaging(this IServiceCollection services, IConfiguration configuration)
  {
    var rabbitMqConnectionString = GetRequiredConnectionString(configuration, "messaging");
    var databaseConnectionString = GetRequiredConnectionString(configuration, "mainDb");
    var numberOfWorkers = configuration.GetValue(NumberOfWorkersSetting, DefaultNumberOfWorkers);

    services.AddRebus(
      (configure, _) => configure
        .Transport(transport => transport.UseRabbitMq(rabbitMqConnectionString, QueueName))
        // Saga state (enroll / unenroll progress) lives in PostgreSQL, so it survives restarts.
        .Sagas(sagas => sagas.StoreInPostgres(databaseConnectionString, "rebus_saga_data", "rebus_saga_index"))
        // Commands go to exactly one service; events (below) are broadcast to whoever subscribes.
        .Routing(routing => routing.TypeBased()
          .Map<CreateCourseCommand>(CoursesQueue)
          .Map<UpdateCourseCommand>(CoursesQueue)
          .Map<DeleteCourseCommand>(CoursesQueue)
          .Map<CreateClassCommand>(CoursesQueue)
          .Map<UpdateClassCommand>(CoursesQueue)
          .Map<DeleteClassCommand>(CoursesQueue)
          .Map<CreateEnrollmentCommand>(EnrollmentsQueue)
          .Map<DeleteEnrollmentCommand>(EnrollmentsQueue))
        .Options(options => options.SetNumberOfWorkers(numberOfWorkers)),
      onCreated: async bus =>
      {
        // Results of course/class commands, shown to the admin as live notifications.
        await bus.Subscribe<CourseCreatedEvent>();
        await bus.Subscribe<CourseCreateRejectionEvent>();
        await bus.Subscribe<CourseUpdatedEvent>();
        await bus.Subscribe<CourseUpdateRejectionEvent>();
        await bus.Subscribe<CourseDeletedEvent>();
        await bus.Subscribe<CourseDeleteRejectionEvent>();
        await bus.Subscribe<ClassCreatedEvent>();
        await bus.Subscribe<ClassCreateRejectionEvent>();
        await bus.Subscribe<ClassUpdatedEvent>();
        await bus.Subscribe<ClassUpdateRejectionEvent>();
        await bus.Subscribe<ClassDeletedEvent>();
        await bus.Subscribe<ClassDeleteRejectionEvent>();

        // Enroll / unenroll sagas.
        await bus.Subscribe<EnrollmentCreatedEvent>();
        await bus.Subscribe<EnrollmentDeletedEvent>();
        await bus.Subscribe<EnrollmentCreateRequestRejectedEvent>();
        await bus.Subscribe<EnrollmentDeleteRequestRejectedEvent>();
        await bus.Subscribe<IncreaseStudentEnrollmentsCountSuccessEvent>();
        await bus.Subscribe<IncreaseStudentEnrollmentsCountFailedEvent>();
        await bus.Subscribe<IncreaseClassEnrollmentsCountSuccessEvent>();
        await bus.Subscribe<IncreaseClassEnrollmentsCountFailedEvent>();
        await bus.Subscribe<DecreaseStudentEnrollmentCountSuccessEvent>();
        await bus.Subscribe<DecreaseStudentEnrollmentCountFailedEvent>();
        await bus.Subscribe<DecreaseClassEnrollmentsCountSuccessEvent>();
        await bus.Subscribe<DecreaseClassEnrollmentsCountFailedEvent>();
      }
    );

    services.AutoRegisterHandlersFromAssemblyOf<Program>();
    return services;
  }

  private static string GetRequiredConnectionString(IConfiguration configuration, string name) =>
    configuration.GetConnectionString(name)
    ?? throw new InvalidOperationException(
      $"Connection string '{name}' is missing. It is normally provided by the Aspire AppHost.");
}

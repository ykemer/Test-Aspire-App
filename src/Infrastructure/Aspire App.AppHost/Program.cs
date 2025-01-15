using Projects;

IDistributedApplicationBuilder? builder = DistributedApplication.CreateBuilder(args);
IResourceBuilder<RedisResource>? cache = builder.AddRedis("cache");

IResourceBuilder<PostgresServerResource>? postgres =
  builder.AddPostgres("postgres").WithDataBindMount(@"C:\Volumes\PG").WithPgWeb();
// To avoid resource consumption, we add databases to a single postgres instance
IResourceBuilder<PostgresDatabaseResource>? mainDb = postgres.AddDatabase("mainDb");
IResourceBuilder<PostgresDatabaseResource>? coursesDb = postgres.AddDatabase("coursesDb");
IResourceBuilder<PostgresDatabaseResource>? enrollmentsDb = postgres.AddDatabase("enrollmentsDb");
IResourceBuilder<PostgresDatabaseResource>? studentsDb = postgres.AddDatabase("studentsDb");


IResourceBuilder<RabbitMQServerResource>? rabbitmq = builder
  .AddRabbitMQ("messaging")
  .WithDataBindMount(@"C:\Volumes\RabbitMQ")
  .WithManagementPlugin();

IResourceBuilder<ProjectResource>? coursesService = builder
  .AddProject<Service_Courses>("coursesService")
  .WithReference(coursesDb)
  .WaitFor(coursesDb)
  .WithReference(rabbitmq)
  .WaitFor(rabbitmq);

IResourceBuilder<ProjectResource>? enrollmentsService = builder
  .AddProject<Service_Enrollments>("enrollmentsService")
  .WithReference(enrollmentsDb)
  .WaitFor(enrollmentsDb)
  .WithReference(rabbitmq)
  .WaitFor(rabbitmq);

IResourceBuilder<ProjectResource>? studentsService = builder
  .AddProject<Service_Students>("studentsService")
  .WithReference(studentsDb)
  .WaitFor(studentsDb)
  .WithReference(rabbitmq)
  .WaitFor(rabbitmq);

IResourceBuilder<ProjectResource>? platformService = builder
  .AddProject<Platform>("platformService")
  .WithReference(mainDb)
  .WaitFor(mainDb)
  .WithReference(rabbitmq)
  .WaitFor(rabbitmq)
  .WithReference(coursesService)
  .WithReference(enrollmentsService)
  .WithReference(studentsService);

builder.AddProject<Aspire_App_Web>("webfrontend")
  .WithExternalHttpEndpoints()
  .WithReference(cache)
  .WithReference(platformService)
  .WaitFor(platformService);


await builder.Build().RunAsync();

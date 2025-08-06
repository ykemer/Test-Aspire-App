using Aspire_App.AppHost;

using Projects;

DotEnv.Load();

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

#region Databases

var postgres =
  builder.AddPostgres("postgres").WithDataBindMount($"{Environment.GetEnvironmentVariable("VOLUME_PATH")}/postgres")
    .WithPgWeb().WithPgAdmin();

// To avoid resource consumption, we add databases to a single postgres instance
var mainDb = postgres.AddDatabase("mainDb");
var coursesDb = postgres.AddDatabase("coursesDb");
var enrollmentsDb = postgres.AddDatabase("enrollmentsDb");
var studentsDb = postgres.AddDatabase("studentsDb");

#endregion

var rabbitmq = builder
  .AddRabbitMQ("messaging")
  .WithDataBindMount($"{Environment.GetEnvironmentVariable("VOLUME_PATH")}/RabbitMQ")
  .WithManagementPlugin();


#region main services

var coursesService = builder
  .AddProject<Service_Courses>("coursesService")
  .WithReference(coursesDb)
  .WaitFor(coursesDb)
  .WithReference(rabbitmq)
  .WaitFor(rabbitmq)
  .WithHttpsEndpoint();

var enrollmentsService = builder
  .AddProject<Service_Enrollments>("enrollmentsService")
  .WithReference(enrollmentsDb)
  .WaitFor(enrollmentsDb)
  .WithReference(rabbitmq)
  .WaitFor(rabbitmq)
  .WithHttpsEndpoint();

var studentsService = builder
  .AddProject<Service_Students>("studentsService")
  .WithReference(studentsDb)
  .WaitFor(studentsDb)
  .WithReference(rabbitmq)
  .WaitFor(rabbitmq)
  .WithHttpsEndpoint();

#endregion

var platformService = builder
  .AddProject<Platform>("platformService")
  .WithReference(mainDb)
  .WaitFor(mainDb)
  .WithReference(rabbitmq)
  .WaitFor(rabbitmq)
  .WithReference(cache)
  .WaitFor(cache)
  .WithReference(coursesService)
  .WithReference(enrollmentsService)
  .WithReference(studentsService)
  .WithHttpsEndpoint();

builder.AddProject<Aspire_App_Web>("webFrontend")
  .WithExternalHttpEndpoints()
  .WithReference(cache)
  .WithReference(platformService)
  .WaitFor(platformService)
  .WithHttpsEndpoint();


await builder.Build().RunAsync();

using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var cache = builder.AddRedis("cache");

var postgres =
  builder.AddPostgres("postgres").WithDataBindMount(@"C:\Volumes\PG").WithPgWeb();
// To avoid resource consumption, we add databases to a single postgres instance
var mainDb = postgres.AddDatabase("mainDb");
var coursesDb = postgres.AddDatabase("coursesDb");
var enrollmentsDb = postgres.AddDatabase("enrollmentsDb");
var studentsDb = postgres.AddDatabase("studentsDb");


var rabbitmq = builder
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

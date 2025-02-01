using Library.Infrastructure;

using Service.Enrollments;
using Service.Enrollments.Features.Enrollments;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ApplicationDbContext>("enrollmentsDb");
builder.AddRedisDistributedCache("cache");


// Add services to the container.
var assembly = typeof(Program).Assembly;

builder.Services.AddGrpc();
builder.Services.AddMassTransitServices(assembly, "queue-enrollments");
builder.Services.AddServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<EnrollmentsService>();
app.MapGet("/",
  () =>
    "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  using var scope = app.Services.CreateScope();
  var initializer =
    scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
  await initializer.InitialiseAsync();
  await initializer.SeedAsync();
}

await app.RunAsync();

// Messaging
// Dispatch: Enrollment deleted, Enrollment created,
// Receive: Student Deleted, Student Name Changed, Course Deleted

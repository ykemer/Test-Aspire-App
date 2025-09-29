using Library.Infrastructure;

using Service.Courses;
using Service.Courses.Features.Classes;
using Service.Courses.Features.Courses;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ApplicationDbContext>("coursesDb");
builder.AddRedisDistributedCache("cache");

// Add services to the container.
var assembly = typeof(Program).Assembly;
// builder.Services.AddMassTransitServices(assembly, "queue-courses");
builder.Services.AddServices();

var app = builder.Build();
// Configure the HTTP request pipeline.
app.MapGrpcService<CoursesService>();
app.MapGrpcService<ClassesService>();


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

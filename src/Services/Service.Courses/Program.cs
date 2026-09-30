using Service.Courses.Common.Database;
using Service.Courses.Common.Setup;
using Service.Courses.Features.Classes;
using Service.Courses.Features.Courses;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ApplicationDbContext>("coursesDb");

builder.Services.AddServices();
builder.Services.AddRebusMessaging(builder.Configuration);

var app = builder.Build();

app.MapGrpcService<CoursesService>();
app.MapGrpcService<ClassesService>();
app.MapDefaultEndpoints();

app.MapGet("/",
  () =>
    "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

if (app.Environment.IsDevelopment())
{
  using var scope = app.Services.CreateScope();
  var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
  await initializer.MigrateAndSeedAsync();
}

await app.RunAsync();

using Service.Students.Common.Database;
using Service.Students.Common.Setup;
using Service.Students.Features;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ApplicationDbContext>("studentsDb");

builder.Services.AddServices();
builder.Services.AddRebusMessaging(builder.Configuration);

var app = builder.Build();

app.MapGrpcService<StudentsService>();
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

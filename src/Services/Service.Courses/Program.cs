using Service.Courses;
using Service.Courses.Features.Courses;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
builder.AddNpgsqlDbContext<ApplicationDbContext>("coursesDb");
builder.AddRedisDistributedCache("cache");
builder.Services.AddServices();

WebApplication? app = builder.Build();
// Configure the HTTP request pipeline.
app.MapGrpcService<CoursesService>();


app.MapGet("/",
  () =>
    "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  using IServiceScope? scope = app.Services.CreateScope();
  ApplicationDbContextInitializer? initializer =
    scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
  await initializer.InitialiseAsync();
  await initializer.SeedAsync();
}

await app.RunAsync();

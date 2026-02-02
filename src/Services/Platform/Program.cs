using FastEndpoints;
using FastEndpoints.Swagger;

using Library.Infrastructure;

using Platform;
using Platform.Common.Database;
using Platform.Common.Middleware.Responses;
using Platform.Features.Classes;
using Platform.Features.Courses;
using Platform.Features.Enrollments;

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
DotEnv.Load(dotenv);

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
// Add PostgresSQL database.
builder.AddNpgsqlDbContext<ApplicationDbContext>("mainDb");

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddRedisDistributedCache("cache");

// Add services to the container.
builder.Services.AddGrpcServices();
builder.Services.AddAuthServices();
builder.Services.AddMassTransitServices();
builder.Services.AddApiServices();
builder.Services.AddCaching();
builder.Services.AddRateLimiting();

var app = builder.Build();

app.UseAuthentication()
  .UseAuthorization();

app.UseRateLimiter();

app.UseDefaultExceptionHandler()
  .UseOutputCache()
  .UseResponseCaching()
  .UseFastEndpoints(options =>
  {
    options.Errors.UseProblemDetails();
    options.Endpoints.Configurator =
      ep =>
      {
        if (ep.AnonymousVerbs is null)
        {
          ep.Description(b => b.Produces<ProblemDetails>(401));
        }

        if (ep.ResDtoType.IsAssignableTo(typeof(IErrorOr)))
        {
          ep.DontAutoSendResponse();
          ep.PostProcessor<ResponseMiddleware>(Order.After);
          ep.Description(b => b.ClearDefaultProduces()
            .Produces(200, ep.ResDtoType.GetGenericArguments()[0])
            .ProducesProblemDetails());
        }
      };
  })
  .UseSwaggerGen();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  using (var scope = app.Services.CreateScope())
  {
    var initializer =
      scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
    await initializer.InitialiseAsync();
    await initializer.SeedAsync();
  }
}

app.UseMiddleware<ProblemDetailsMiddleware>();
app.MapHub<EnrollmentHub>("/enrollmentHub");
app.MapHub<CoursesHub>("/courseHub");
app.MapHub<ClassesHub>("/classHub");
await app.RunAsync();

// TODO error handling
// TODO log errors
// TODO log unhandled
// TODO validation problem
// TODO password recovery




// TODO update endpoints to return 204 where applicable

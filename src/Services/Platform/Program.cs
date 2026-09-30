using FastEndpoints;
using FastEndpoints.Swagger;

using Library.Infrastructure;

using Platform.Common.Database;
using Platform.Common.Responses;
using Platform.Common.Setup;
using Platform.Features.Classes;
using Platform.Features.Courses;
using Platform.Features.Enrollments;

// Local development secrets (JWT keys, seed password) live in a .env file next to the project.
DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ApplicationDbContext>("mainDb");

builder.Services.AddApi();
builder.Services.AddAuth();
builder.Services.AddGrpcClients();
builder.Services.AddRebusMessaging(builder.Configuration);
builder.Services.AddRateLimits();

var app = builder.Build();

// First, so it also catches errors thrown by the middleware below. Returns a generic 500 and logs the details.
app.UseDefaultExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.UseFastEndpoints(options =>
{
  options.Errors.UseProblemDetails();
  options.Endpoints.Configurator = endpoint =>
  {
    var isAnonymous = endpoint.AnonymousVerbs is not null;

    // Every endpoint is rate limited: sign-in endpoints per IP address, all others per user.
    endpoint.Options(route => route.RequireRateLimiting(
      isAnonymous ? RateLimitPolicies.SignIn : RateLimitPolicies.PerUser));

    if (!isAnonymous)
    {
      endpoint.Description(route => route.Produces<ProblemDetails>(StatusCodes.Status401Unauthorized));
    }

    // Endpoints that return ErrorOr<T> are answered by ErrorOrResponseSender.
    if (endpoint.ResDtoType.IsAssignableTo(typeof(IErrorOr)))
    {
      endpoint.DontAutoSendResponse();
      endpoint.PostProcessor<ErrorOrResponseSender>(Order.After);
      endpoint.Description(route => route.ClearDefaultProduces()
        .Produces(StatusCodes.Status200OK, endpoint.ResDtoType.GetGenericArguments()[0])
        .ProducesProblemDetails());
    }
  };
});

app.MapHub<EnrollmentHub>(HubRoutes.Enrollments);
app.MapHub<CoursesHub>(HubRoutes.Courses);
app.MapHub<ClassesHub>(HubRoutes.Classes);
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
  // API documentation is only published in development.
  app.UseSwaggerGen();

  using var scope = app.Services.CreateScope();
  var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
  await initializer.MigrateAndSeedAsync();
}

await app.RunAsync();

using FastEndpoints;
using FastEndpoints.Swagger;

using Library.Infrastructure;

using Platform;
using Platform.Database;
using Platform.Middleware.Responses;

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
DotEnv.Load(dotenv);

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
// Add PostgresSQL database.
builder.AddNpgsqlDbContext<ApplicationDbContext>("mainDb");

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddRabbitMQClient("messaging");
builder.AddRedisDistributedCache("cache");

// Add services to the container.
builder.Services.AddGrpcServices();
builder.Services.AddAuthServices();
builder.Services.AddMassTransitServices();
builder.Services.AddApiServices();
builder.Services.AddCaching();

var app = builder.Build();

app.UseAuthentication()
  .UseAuthorization();

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
          ep.Description(
            b => b.ClearDefaultProduces()
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
await app.RunAsync();

// TODO error handling
// TODO log errors
// TODO log unhandled
// TODO validation problem
// TODO password recovery
// TODO RPC errors handling
// TODO remove mediators

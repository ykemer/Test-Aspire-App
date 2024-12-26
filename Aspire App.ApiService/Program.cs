using Aspire_App.ApiService;
using Aspire_App.ApiService.Features.Auth;
using Aspire_App.ApiService.Middleware;
using Aspire_App.ApiService.Persistence;
using Aspire_App.ApiService.Services.MailService;
using FastEndpoints;
using FastEndpoints.Swagger;
using Library.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Namotion.Reflection;

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
DotEnv.Load(dotenv);

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
// Add PostgresSQL database.
builder.AddNpgsqlDbContext<ApplicationDbContext>("postgresdb");
// Add MS SQL DB
//builder.AddSqlServerDbContext<ApplicationDbContext>("sqlDb");

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddApiServices();

var app = builder.Build();

app.UseAuthentication()
    .UseAuthorization();

// app.MapGroup("/api/identity")
//     .WithTags("Identity")
//     .MapCustomIdentityApi<ApplicationUser>();

app.UseDefaultExceptionHandler() 
    .UseFastEndpoints(options =>
    {
        options.Errors.UseProblemDetails();
        options.Endpoints.Configurator =
            ep =>
            {
                if (ep.AnonymousVerbs is null)
                    ep.Description(b => b.Produces<ProblemDetails>(401));
                
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
    .UseResponseCaching()
    .UseSwaggerGen();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseMiddleware<ProblemDetailsMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}


app.Run();

// TODO error handling
// TODO log errors
// TODO log unhandled
// TODO validation problem
// TODO password recovery
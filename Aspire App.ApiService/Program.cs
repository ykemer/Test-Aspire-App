using System.Reflection;
using System.Text;
using Aspire_App.ApiService;
using Aspire_App.ApiService.Domain.Models;
using Aspire_App.ApiService.Domain.Persistence;
using Aspire_App.ApiService.Infrastructure.Persistence;
using Aspire_App.ApiService.Infrastructure.Persistence.Repositories;
using Aspire_App.ApiService.Infrastructure.Services;
using FastEndpoints;
using FastEndpoints.Swagger;
using Library.Infrastructure;
using Library.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql.Internal;

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
DotEnv.Load(dotenv);

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
// Add PostgresSQL database.
//builder.AddNpgsqlDbContext<ApplicationDbContext>("postgresdb");
// Add MS SQL DB
builder.AddSqlServerDbContext<ApplicationDbContext>("sqlDb");

// Add service defaults & Aspire components.
builder.AddServiceDefaults();



// Add services to the container.
builder.Services.AddApiServices();


var app = builder.Build();

app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints()
    .UseSwaggerGen();


using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
    await Waiter.Wait(10_000); // TODO wait for db to be ready
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}


app.Run();
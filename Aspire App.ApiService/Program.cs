using System.Reflection;
using Aspire_App.ApiService.Domain.Models;
using Aspire_App.ApiService.Domain.Persistence;
using Aspire_App.ApiService.Infrastructure.Persistence;
using Aspire_App.ApiService.Infrastructure.Persistence.Repositories;
using Aspire_App.ApiService.Infrastructure.Services;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Library.Infrastructure;
using Library.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using NSwag;


var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
DotEnv.Load(dotenv);

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add PostgresSQL database.
builder.AddNpgsqlDbContext<ApplicationDbContext>("postgresdb");

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddSignInManager<SignInManager<ApplicationUser>>();

builder.Services
    //.AddAuthenticationCookie(validFor: TimeSpan.FromMinutes(10))
    .AddAuthorization(options =>
    {
        options.AddPolicy("RequireAdministratorRole",
            policy => policy.RequireRole("Administrator"));
        options.AddPolicy("RequireUserRole",
            policy =>   policy.RequireRole("User", "Administrator"));
    })
    .AddAuthenticationJwtBearer(options =>
    {
        options.SigningKey = Environment.GetEnvironmentVariable("JWT_SIGN_KEY");
        
    })
    .AddFastEndpoints()
    .AddSwaggerDocument(options =>
    {
        options.AddSecurity("Bearer", new OpenApiSecurityScheme
        {
            Description = "Please enter 'Bearer [jwt]'",
            Name = "Authorization",
        });
        
        options.Title = "Aspire App API";
        options.Version = "v1";
    });



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<ApplicationDbContextInitialiser>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();




var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseDefaultExceptionHandler()
    .UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints();
app.UseSwaggerGen();


using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
    await Waiter.Wait(5000);
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}


app.Run();
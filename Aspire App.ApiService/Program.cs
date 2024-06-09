using System.Reflection;
using Aspire_App.ApiService.Domain.Persistence;
using Aspire_App.ApiService.Infrastructure.Persistence;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.AddNpgsqlDbContext<ApplicationDbContext>("postgresdb");

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddFastEndpoints().AddSwaggerDocument();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddScoped<IStudentRepository, StudentRepository>();



var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseFastEndpoints().UseSwaggerGen();


app.Run();


using System.Text;

using CoursesGRPCClient;

using EnrollmentsGRPCClient;

using FastEndpoints;
using FastEndpoints.Swagger;

using MassTransit;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;

using Platform.AsyncDataServices.StateMachines;
using Platform.Database;
using Platform.Entities;
using Platform.Middleware.Grpc;
using Platform.Services.JWT;
using Platform.Services.MailService;
using Platform.Services.User;

using StudentsGRPCClient;

namespace Platform;

public static class DependencyInjection
{
  public static IServiceCollection AddMassTransitServices(this IServiceCollection services)
  {
    var assembly = typeof(Program).Assembly;

    services.Configure<MassTransitHostOptions>(options =>
    {
      options.WaitUntilStarted = true;
    });


    services.AddMassTransit(configure =>
    {
      configure.SetKebabCaseEndpointNameFormatter();
      configure.AddConsumers(assembly);

      configure
        .AddSagaStateMachine<StudentEnrollmentsStateMachine, StudentEnrollmentsState>()
        .EntityFrameworkRepository(r =>
        {
          r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
          r.ExistingDbContext<ApplicationDbContext>();
          r.UsePostgres();
        });

      configure.UsingRabbitMq((context, cfg) =>
      {
        var configService = context.GetRequiredService<IConfiguration>();
        var connectionString = configService.GetConnectionString("messaging");

        cfg.Host(connectionString);
        cfg.UseInMemoryOutbox(context);

        cfg.ReceiveEndpoint("queue-platform", e =>
        {
          e.ConfigureConsumers(context);
        });

        cfg.ConfigureEndpoints(context);
      });


    });

    return services;
  }

  public static IServiceCollection AddGrpcServices(this IServiceCollection services)
  {
    services.AddScoped<IGrpcRequestMiddleware, GrpcRequestMiddleware>();

    services.AddGrpcClient<GrpcCoursesService.GrpcCoursesServiceClient>(options =>
    {
      options.Address = new Uri("https://coursesService");
    });
    services.AddGrpcClient<GrpcEnrollmentsService.GrpcEnrollmentsServiceClient>(options =>
    {
      options.Address = new Uri("https://enrollmentsService");
    });
    services.AddGrpcClient<GrpcStudentsService.GrpcStudentsServiceClient>(options =>
    {
      options.Address = new Uri("https://studentsService");
    });
    return services;
  }

  public static IServiceCollection AddAuthServices(this IServiceCollection services)
  {
    services
      .AddIdentity<ApplicationUser, IdentityRole>()
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders()
      .AddSignInManager<SignInManager<ApplicationUser>>();


    services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(
        o =>
        {
          o.TokenValidationParameters = new TokenValidationParameters
          {
            IssuerSigningKey =
              new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SIGN_KEY")!)),
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_KEY_ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_KEY_AUDIENCE"),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
          };
        });

    services.AddAuthorization(options =>
    {
      options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
      options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Administrator"));
    });
    return services;
  }

  public static IServiceCollection AddApiServices(this IServiceCollection services)
  {
    services.AddTransient<IEmailSender<ApplicationUser>, UserMailService>();
    services.AddTransient<IEmailSender, EmailSender>();


    services.AddScoped<ApplicationDbContextInitializer>();
    services.AddScoped<IJwtService, JwtService>();
    services.AddScoped<IUserService, UserService>();


    services
      .AddFastEndpoints()
      .AddResponseCaching()
      .SwaggerDocument(o =>
      {
        o.DocumentSettings = s =>
        {
          s.Title = "My API";
          s.Version = "v1";
        };
      });

    return services;
  }
}

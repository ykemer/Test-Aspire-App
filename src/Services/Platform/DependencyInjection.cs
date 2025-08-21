using System.Text;
using System.Threading.RateLimiting;

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

using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

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
      configure.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
      {
        o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
        o.UsePostgres();
      });
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
      .AddJwtBearer(o =>
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
          s.Title = "Students API";
          s.Version = "v1";
        };
      });

    return services;
  }

  public static IServiceCollection AddCaching(this IServiceCollection services)
  {
    services.AddResponseCaching(); // client side caching
    services.AddOutputCache(x =>
    {
      x.AddBasePolicy(c => c.Cache());
      x.AddPolicy("CoursesCache", c =>
        c.Cache()
          .Expire(TimeSpan.FromMinutes(5))
          .SetVaryByQuery(["pageNumber", "pageSize"])
          .Tag("courses")
      );
    });
    return services;
  }

  public static IServiceCollection AddRateLimiting(this IServiceCollection services)
  {
    services.AddRateLimiter(options =>
    {
      options.AddPolicy("fixed-per-user", context =>
        RateLimitPartition.GetFixedWindowLimiter(
          partitionKey: context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
          factory: _ => new FixedWindowRateLimiterOptions
          {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
          }
        )
      );

      // Custom rejection response
      options.OnRejected = async (context, cancellationToken) =>
      {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/problem+json";
        context.HttpContext.Response.Headers.Add("Retry-After", "60");

        // Add remaining requests header (0 when rate limited)
        context.HttpContext.Response.Headers.Add("X-RateLimit-Remaining", "0");
        context.HttpContext.Response.Headers.Add("X-RateLimit-Limit", "10");
        context.HttpContext.Response.Headers.Add("X-RateLimit-Reset", DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds().ToString());

        var problemDetails = new ProblemDetails
        {
          Status = 429,
          Title = "Too Many Requests",
          Detail = "Rate limit exceeded. Please try again later.",
          Type = "https://tools.ietf.org/html/rfc6585#section-4"
        };

        await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
      };
    });

    return services;
  }
}

using System.Text;
using System.Threading.RateLimiting;

using ClassesGRPCClient;

using Contracts.Classes.Commands;
using Contracts.Classes.Events;
using Contracts.Classes.Events.DecreaseClassEnrollmentsCount;
using Contracts.Classes.Events.IncreaseClassEnrollmentsCount;
using Contracts.Courses.Commands;
using Contracts.Courses.Events;
using Contracts.Enrollments.Commands;
using Contracts.Enrollments.Events;
using Contracts.Students.Events.DecreaseStudentEnrollmentCount;
using Contracts.Students.Events.IncreaseStudentEnrollmentsCount;

using CoursesGRPCClient;

using EnrollmentsGRPCClient;

using FastEndpoints;
using FastEndpoints.Swagger;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

using Platform.Common.Database;
using Platform.Common.Database.Entities;
using Platform.Common.Middleware.Grpc;
using Platform.Common.Providers;
using Platform.Common.Services.JWT;
using Platform.Common.Services.MailService;
using Platform.Common.Services.User;

using Rebus.Config;
using Rebus.PostgreSql.Sagas;
using Rebus.RabbitMq;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;

using StudentsGRPCClient;

using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Platform;

public static class DependencyInjection
{
  public static IServiceCollection AddRebusServices(this IServiceCollection services, IConfiguration config)
  {
    var rabbitConn = config.GetConnectionString("messaging")!;
    var dbConn = config.GetConnectionString("mainDb")!;

    services.AddRebus(
      (configure, _) => configure
        .Transport(t => t.UseRabbitMq(rabbitConn, "queue-platform"))
        .Sagas(s => s.StoreInPostgres(dbConn, "rebus_saga_data", "rebus_saga_index"))
        .Routing(r => r.TypeBased()
          .Map<CreateCourseCommand>("queue-courses")
          .Map<UpdateCourseCommand>("queue-courses")
          .Map<DeleteCourseCommand>("queue-courses")
          .Map<CreateClassCommand>("queue-courses")
          .Map<UpdateClassCommand>("queue-courses")
          .Map<DeleteClassCommand>("queue-courses")
          .Map<CreateEnrollmentCommand>("queue-enrollments")
          .Map<DeleteEnrollmentCommand>("queue-enrollments"))
        .Options(o => o.SetNumberOfWorkers(1)),
      onCreated: async bus =>
      {
        await bus.Subscribe<CourseCreatedEvent>();
        await bus.Subscribe<CourseCreateRejectionEvent>();
        await bus.Subscribe<CourseUpdatedEvent>();
        await bus.Subscribe<CourseUpdateRejectionEvent>();
        await bus.Subscribe<CourseDeletedEvent>();
        await bus.Subscribe<CourseDeleteRejectionEvent>();
        await bus.Subscribe<ClassCreatedEvent>();
        await bus.Subscribe<ClassCreateRejectionEvent>();
        await bus.Subscribe<ClassUpdatedEvent>();
        await bus.Subscribe<ClassUpdateRejectionEvent>();
        await bus.Subscribe<ClassDeletedEvent>();
        await bus.Subscribe<ClassDeleteRejectionEvent>();
        await bus.Subscribe<EnrollmentCreatedEvent>();
        await bus.Subscribe<EnrollmentDeletedEvent>();
        await bus.Subscribe<EnrollmentCreateRequestRejectedEvent>();
        await bus.Subscribe<EnrollmentDeleteRequestRejectedEvent>();
        await bus.Subscribe<IncreaseStudentEnrollmentsCountSuccessEvent>();
        await bus.Subscribe<IncreaseStudentEnrollmentsCountFailedEvent>();
        await bus.Subscribe<IncreaseClassEnrollmentsCountSuccessEvent>();
        await bus.Subscribe<IncreaseClassEnrollmentsCountFailedEvent>();
        await bus.Subscribe<DecreaseStudentEnrollmentCountSuccessEvent>();
        await bus.Subscribe<DecreaseStudentEnrollmentCountFailedEvent>();
        await bus.Subscribe<DecreaseClassEnrollmentsCountSuccessEvent>();
        await bus.Subscribe<DecreaseClassEnrollmentsCountFailedEvent>();
      }
    );

    services.AutoRegisterHandlersFromAssemblyOf<Program>();
    return services;
  }

  public static IServiceCollection AddGrpcServices(this IServiceCollection services)
  {
    services.AddScoped<IGrpcRequestMiddleware, GrpcRequestMiddleware>();

    services.AddGrpcClient<GrpcCoursesService.GrpcCoursesServiceClient>(options =>
    {
      options.Address = new Uri("https://coursesService");
    });
    services.AddGrpcClient<GrpcClassService.GrpcClassServiceClient>(options =>
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
          ValidateLifetime = true
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
        o.AutoTagPathSegmentIndex = 0;
        o.DocumentSettings = s =>
        {
          s.Title = "Students API";
          s.Version = "v1";
        };

        o.TagDescriptions = t =>
        {
          t["Auth"] = "Authorization endpoints for user management";
          t["Classes"] = "Possible operations related to Classes";
          t["Courses"] = "Possible operations related to Courses";
          t["Students"] = "Possible operations related to Students";
          t["Enrollments"] = "Possible operations related to Enrollments";
        };
      });

    services.AddSignalR();
    services.AddSingleton<IUserIdProvider, UserIdProvider>();

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
          context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
          _ => new FixedWindowRateLimiterOptions { PermitLimit = 60, Window = TimeSpan.FromMinutes(1) }
        )
      );

      // Custom rejection response
      options.OnRejected = async (context, cancellationToken) =>
      {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/problem+json";
        context.HttpContext.Response.Headers.Append("Retry-After", "60");

        // Add remaining requests header (0 when rate limited)
        context.HttpContext.Response.Headers.Append("X-RateLimit-Remaining", "0");
        context.HttpContext.Response.Headers.Append("X-RateLimit-Limit", "10");
        context.HttpContext.Response.Headers.Append("X-RateLimit-Reset",
          DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds().ToString());

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

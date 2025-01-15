using System.Text;

using CoursesGRPCClient;

using EnrollmentsGRPCClient;

using FastEndpoints;
using FastEndpoints.Swagger;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;

using Platform.AsyncDataServices;
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
  public static IServiceCollection AddApiServices(this IServiceCollection services)
  {
    services.AddTransient<IEmailSender<ApplicationUser>, UserMailService>();
    services.AddTransient<IEmailSender, EmailSender>();
    services.AddTransient<IMessageBusClient, MessageBusClient>();


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
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
              new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SIGN_KEY")!))
          };
        });

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddBearerToken(IdentityConstants.BearerScheme);
    services.AddScoped<ApplicationDbContextInitializer>();
    services.AddScoped<IJwtService, JwtService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IGrpcRequestMiddleware, GrpcRequestMiddleware>();

    services.AddGrpcClient<GrpcCoursesService.GrpcCoursesServiceClient>(options =>
    {
      options.Address = new Uri("http://coursesService");
    });
    services.AddGrpcClient<GrpcEnrollmentsService.GrpcEnrollmentsServiceClient>(options =>
    {
      options.Address = new Uri("http://enrollmentsService");
    });
    services.AddGrpcClient<GrpcStudentsService.GrpcStudentsServiceClient>(options =>
    {
      options.Address = new Uri("http://studentsService");
    });

    services.AddAuthorization(options =>
      {
        options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
        options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Administrator"));
      })
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

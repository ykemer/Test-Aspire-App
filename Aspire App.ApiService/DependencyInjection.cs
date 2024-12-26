using System.Reflection;
using System.Text;
using Aspire_App.ApiService.Features.Auth;
using Aspire_App.ApiService.Persistence;
using Aspire_App.ApiService.Services.JWT;
using Aspire_App.ApiService.Services.MailService;
using Aspire_App.ApiService.Services.User;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;

namespace Aspire_App.ApiService;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        services.AddTransient<IEmailSender<ApplicationUser>, UserMailService>();
        services.AddTransient<IEmailSender, EmailSender>();

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

        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IUserService, UserService>();
        


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

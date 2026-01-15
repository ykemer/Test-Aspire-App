using Aspire_App.Web.Handlers;
using Aspire_App.Web.Middleware;
using Aspire_App.Web.Services.CookiesServices;
using Aspire_App.Web.Services.Courses;
using Aspire_App.Web.Services.Hubs;
using Aspire_App.Web.Services.Students;
using Aspire_App.Web.Services.TokenServices;
using Aspire_App.Web.Services.UserService;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Http.Resilience;

using Polly;

using AuthenticationService = Aspire_App.Web.Services.Auth.AuthenticationService;
using IAuthenticationService = Aspire_App.Web.Services.Auth.IAuthenticationService;

namespace Aspire_App.Web;

public static class DependencyInjection
{
  public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
  {
    var platformServiceUrl = configuration["services:platformService:https:0"];

    if (string.IsNullOrEmpty(platformServiceUrl))
    {
      throw new InvalidOperationException("Platform service URL is not configured.");
    }

    // Add services to the container.
    services
      .AddRazorComponents()
      .AddInteractiveServerComponents();

    services.AddHttpContextAccessor();
    services.AddScoped<ICookiesService, CookiesService>();
    services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
    services.AddScoped<IAuthenticationService, AuthenticationService>();
    services.AddScoped<IUserService, UserService>();

    services.AddScoped<ITokenService, RedisTokenService>();
    services.AddScoped<BearerTokenInterceptor>();


    services.AddHttpClient("ServerApi")
      .ConfigureHttpClient(c => c.BaseAddress = new Uri(platformServiceUrl));


    services.AddHttpClient<IAuthenticationService, AuthenticationService>(client =>
      {
        client.BaseAddress = new Uri(platformServiceUrl);
      })
      .AddResilienceHandler("AuthenticationHandler", builder =>
      {
        builder.AddRetry(new HttpRetryStrategyOptions
        {
          MaxRetryAttempts = 3, Delay = TimeSpan.FromSeconds(2), BackoffType = DelayBackoffType.Exponential
        });
        builder.AddTimeout(TimeSpan.FromSeconds(30));
      });

    services.AddHttpClient<IStudentApiService, StudentApiService>(client =>
    {
      client.BaseAddress = new Uri(platformServiceUrl);
    }).AddHttpMessageHandler<BearerTokenInterceptor>();

    services.AddHttpClient<IUserService, UserService>(client =>
    {
      client.BaseAddress = new Uri(platformServiceUrl);
    }).AddHttpMessageHandler<BearerTokenInterceptor>();

    services.AddHttpClient<IClassesApiService, ClassesApiApiService>(client =>
    {
      client.BaseAddress = new Uri(platformServiceUrl);
    }).AddHttpMessageHandler<BearerTokenInterceptor>();

    services.AddHttpClient<IEnrollmentApiService, EnrollmentApiService>(client =>
    {
      client.BaseAddress = new Uri(platformServiceUrl);
    }).AddHttpMessageHandler<BearerTokenInterceptor>();

    services.AddHttpClient<ICoursesApiService, CoursesApiService>(client =>
    {
      client.BaseAddress = new Uri(platformServiceUrl);
    }).AddHttpMessageHandler<BearerTokenInterceptor>();


    services.AddCascadingAuthenticationState();
    services.AddAuthorizationCore();


    services.AddAuthentication("Custom")
      .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("Custom", options => { });

    services.AddScoped<EnrollmentHubService>();
    services.AddScoped<CoursesHubService>();
    services.AddScoped<ClassesHubService>();
    return services;
  }
}

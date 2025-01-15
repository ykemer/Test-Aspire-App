using Aspire_App.Web.Handlers;
using Aspire_App.Web.Middleware;
using Aspire_App.Web.Services.CookiesServices;
using Aspire_App.Web.Services.Courses;
using Aspire_App.Web.Services.Students;
using Aspire_App.Web.Services.TokenServices;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace Aspire_App.Web;

public static class DependencyInjection
{
  public static IServiceCollection AddWebServices(this IServiceCollection services)
  {
    // Add services to the container.
    services
      .AddRazorComponents()
      .AddInteractiveServerComponents();

    services.AddHttpContextAccessor();
    services.AddScoped<ICookiesService, CookiesService>();
    services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

    services.AddScoped<ITokenService, RedisTokenService>();
    services.AddScoped<BearerTokenInterceptor>();

    services.AddHttpClient("ServerApi")
      .ConfigureHttpClient(c => c.BaseAddress = new Uri("https+http://platformService"));

    services.AddHttpClient<IStudentApiService, StudentApiService>(client =>
    {
      // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
      // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
      client.BaseAddress = new Uri("https+http://platformService");
    }).AddHttpMessageHandler<BearerTokenInterceptor>();

    services.AddHttpClient<ICoursesApiService, CoursesApiService>(client =>
    {
      // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
      // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
      client.BaseAddress = new Uri("https+http://platformService");
    }).AddHttpMessageHandler<BearerTokenInterceptor>();


    services.AddCascadingAuthenticationState();
    services.AddAuthorizationCore();


    services.AddAuthentication("Custom")
      .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("Custom", options => { });

    return services;
  }
}

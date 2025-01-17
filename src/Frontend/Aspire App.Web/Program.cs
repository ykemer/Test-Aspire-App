using Aspire_App.Web;
using Aspire_App.Web.Components;

using AuthenticationService = Aspire_App.Web.Services.Auth.AuthenticationService;
using IAuthenticationService = Aspire_App.Web.Services.Auth.IAuthenticationService;


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");
builder.AddRedisDistributedCache("cache");

// Add services to the container.
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddWebServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error", true);
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
  .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();

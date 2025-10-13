using Aspire_App.Web;
using Aspire_App.Web.Components;


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");
builder.AddRedisDistributedCache("cache");

// Add services to the container.
builder.Services.AddWebServices(builder.Configuration);

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

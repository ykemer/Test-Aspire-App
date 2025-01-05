namespace Aspire_App.Web.Services.CookiesServices;

public interface ICookiesService
{
    string GenerateUserIdCookie();
    string GetUserId();
}
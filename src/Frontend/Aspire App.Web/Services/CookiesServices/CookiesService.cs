namespace Aspire_App.Web.Services.CookiesServices;

public class CookiesService : ICookiesService
{
  private const string UserIdCookieName = "UserId";
  private readonly IHttpContextAccessor _httpContextAccessor;

  public CookiesService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

  public string GenerateUserIdCookie()
  {
    var userId = Guid.CreateVersion7().ToString();

    // Set the cookie options
    var cookieOptions = new CookieOptions()
    {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict,
      Expires = DateTimeOffset.UtcNow.AddYears(1) // Set expiration as needed
    };

    // Add the cookie to the response
    _httpContextAccessor.HttpContext?.Response.Cookies.Append(UserIdCookieName, userId, cookieOptions);
    return userId;
  }

  public string GetUserId()
  {
    var userId = _httpContextAccessor.HttpContext?.Request.Cookies[UserIdCookieName];
    if (string.IsNullOrEmpty(userId))
    {
      userId = GenerateUserIdCookie();
    }

    return userId;
  }
}

namespace  Microsoft.AspNetCore.Http;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext httpContext)
    {
        httpContext.Items.TryGetValue("UserId", out var userIdObj);
        return Guid.Parse(userIdObj.ToString());
    }
}
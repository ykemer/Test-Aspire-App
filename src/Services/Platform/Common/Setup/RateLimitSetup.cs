using System.Globalization;
using System.Threading.RateLimiting;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using Platform.Common.Auth;

namespace Platform.Common.Setup;

/// <summary>
/// Names of the rate-limit policies. Program.cs applies them to every endpoint automatically:
/// signed-in endpoints get <see cref="PerUser"/>, anonymous (sign-in) endpoints get <see cref="SignIn"/>.
/// </summary>
public static class RateLimitPolicies
{
  public const string PerUser = "per-user";
  public const string SignIn = "sign-in-per-ip";

  public const int PerUserRequestsPerMinute = 60;

  /// <summary>Low on purpose: slows down password guessing and account spam.</summary>
  public const int SignInRequestsPerMinute = 10;
}

public static class RateLimitSetup
{
  private static readonly TimeSpan s_window = TimeSpan.FromMinutes(1);

  public static IServiceCollection AddRateLimits(this IServiceCollection services)
  {
    services.AddRateLimiter(options =>
    {
      options.AddPolicy(RateLimitPolicies.PerUser, context =>
        RateLimitPartition.GetFixedWindowLimiter(
          GetUserPartitionKey(context),
          _ => new FixedWindowRateLimiterOptions
          {
            PermitLimit = RateLimitPolicies.PerUserRequestsPerMinute, Window = s_window
          }));

      options.AddPolicy(RateLimitPolicies.SignIn, context =>
        RateLimitPartition.GetFixedWindowLimiter(
          GetIpPartitionKey(context),
          _ => new FixedWindowRateLimiterOptions
          {
            PermitLimit = RateLimitPolicies.SignInRequestsPerMinute, Window = s_window
          }));

      options.OnRejected = WriteTooManyRequestsResponse;
    });

    return services;
  }

  /// <summary>Each signed-in user gets their own budget. Anonymous callers share one per IP address.</summary>
  public static string GetUserPartitionKey(HttpContext context) =>
    context.User.FindFirst(ClaimsPrincipalExtensions.UserIdClaim)?.Value is { } userId
      ? $"user:{userId}"
      : GetIpPartitionKey(context);

  private static string GetIpPartitionKey(HttpContext context) =>
    $"ip:{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";

  private static async ValueTask WriteTooManyRequestsResponse(OnRejectedContext context,
    CancellationToken cancellationToken)
  {
    var response = context.HttpContext.Response;
    response.StatusCode = StatusCodes.Status429TooManyRequests;
    response.Headers.RetryAfter = ((int)s_window.TotalSeconds).ToString(CultureInfo.InvariantCulture);

    await response.WriteAsJsonAsync(new ProblemDetails
    {
      Status = StatusCodes.Status429TooManyRequests,
      Title = "Too Many Requests",
      Detail = "Rate limit exceeded. Please try again later.",
      Type = "https://tools.ietf.org/html/rfc6585#section-4"
    }, cancellationToken);
  }
}

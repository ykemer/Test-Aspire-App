using System.Net;
using System.Text.Json;

using FastEndpoints;

namespace Platform.Common.Middleware.Responses;

public class ProblemDetailsMiddleware
{
  private readonly ILogger<ProblemDetailsMiddleware> _logger;
  private readonly RequestDelegate _next;

  public ProblemDetailsMiddleware(RequestDelegate next, ILogger<ProblemDetailsMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An unhandled exception has occurred.");
      await HandleExceptionAsync(context, ex);
    }
  }

  private static Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    if (context.Response.HasStarted)
    {
      // Log the exception and return
      Console.WriteLine("The response has already started, the error handling middleware will not be executed.");
      return Task.CompletedTask;
    }

    var problemDetails = new ProblemDetails
    {
      Status = (int)HttpStatusCode.BadRequest,
      //Title = "An error occurred while processing your request.",
      Detail = exception.Message
    };

    context.Response.ContentType = "application/json";
    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

    var json = JsonSerializer.Serialize(problemDetails);
    return context.Response.WriteAsync(json);
  }
}

using System.Net;
using System.Text.Json;

using Aspire_App.Web.Exceptions;

using FastEndpoints;

namespace Aspire_App.Web.Helpers;

public static class FrontendHelper
{
  private static readonly JsonSerializerOptions s_jsonOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true
  };

  public static string ToPascalCase(string input)
  {
    if (string.IsNullOrEmpty(input))
    {
      return input;
    }

    return char.ToUpper(input[0]) + input.Substring(1);
  }


  public static async Task<T?> ReadJsonOrThrowForErrors<T>(HttpResponseMessage response,
    string notFoundMessage = "Record not found")
  {
    if (response.IsSuccessStatusCode)
    {
      return await response.Content.ReadFromJsonAsync<T>(s_jsonOptions);
    }

    await ProcessResponseErrors(response, notFoundMessage);
    return default;
  }

  public static async Task ProcessResponseErrors(HttpResponseMessage response, string notFoundMessage)
  {
    if (response.StatusCode.Equals(HttpStatusCode.NotFound))
    {
      throw new NotFoundException(notFoundMessage);
    }

    if (response.StatusCode.Equals(HttpStatusCode.Unauthorized) || response.StatusCode.Equals(HttpStatusCode.Forbidden))
    {
      throw new UnauthorizedAccessException("You are not authorized to access this resource.");
    }

    if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
    {
      await ProcessValidationDetails(response);
    }

    throw new ArgumentException($"Error fetching resource: {notFoundMessage}");
  }


  private static async Task ProcessValidationDetails(HttpResponseMessage response)
  {
    if (response.Content.Headers.ContentType?.MediaType != "application/problem+json")
    {
      throw new Exception("Unexpected problem details format.");
    }

    var statusCode = (int)response.StatusCode;

    if (statusCode.Equals(HttpStatusCode.BadRequest))
    {
      var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(s_jsonOptions);
      throw new ValidationException(problemDetails?.Errors.ToDictionary(e => e.Name, e => new[] { e.Reason }) ??
                                    new Dictionary<string, string[]>());
    }
  }
}

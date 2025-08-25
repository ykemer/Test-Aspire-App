using System.Text.Json;

using Aspire_App.Web.Exceptions;

using FastEndpoints;

namespace Aspire_App.Web.Helpers;

public static class FrontendHelper
{
  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
  };

  public static string ToPascalCase(string input)
  {
    if (string.IsNullOrEmpty(input))
    {
      return input;
    }

    return char.ToUpper(input[0]) + input.Substring(1);
  }

  public static async Task ProcessValidationDetails(HttpResponseMessage response)
  {
    if (response.Content.Headers.ContentType?.MediaType == "application/problem+json")
    {
      var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(JsonOptions);
      throw new ValidationException(problemDetails?.Errors.ToDictionary(e => e.Name, e => new[] { e.Reason }) ??
                                    new Dictionary<string, string[]>());
    }
  }

  public static async Task<T?> ReadJsonOrThrowForErrors<T>(HttpResponseMessage response, string notFoundMessage)
  {
    if (!response.IsSuccessStatusCode)
    {
      if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
      {
        throw new NotFoundException(notFoundMessage);
      }

      await ProcessValidationDetails(response);
      throw new ArgumentException($"Error fetching resource: {notFoundMessage}");
    }

    return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
  }
}

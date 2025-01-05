using System.Text.Json;
using Aspire_App.Web.Exceptions;
using FastEndpoints;

namespace Aspire_App.Web.Helpers;

public static class FrontendHelper
{
    public static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1);
    }

    public static async Task ProcessValidationDetails(HttpResponseMessage response)
    {
        if(response.Content.Headers.ContentType?.MediaType == "application/problem+json")
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(options);
            throw new ValidationException(problemDetails?.Errors.ToDictionary(e => e.Name, e => new [] {e.Reason}) ?? new Dictionary<string, string[]>());
        }  
    }
}
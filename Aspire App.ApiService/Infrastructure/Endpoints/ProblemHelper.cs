using ErrorOr;

namespace Aspire_App.ApiService.Infrastructure.Endpoints;

public static class ProblemHelper
{
    
    public static IResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
        {
            return Results.Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0]);
    }
    
    public static IResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError,
        };

        return Results.Problem(statusCode: statusCode, detail: error.Description);
    }

    private static IResult ValidationProblem(List<Error> errors)
    {
        var modelStateDictionary = new Dictionary<string, string[]>();

        foreach (var error in errors)
        {
            if(!modelStateDictionary.ContainsKey(error.Code))
            {
                modelStateDictionary.Add(
                    error.Code,
                    new string[] { error.Description });
                continue;                
            }
            
            modelStateDictionary[error.Code] = 
                modelStateDictionary[error.Code].Append(error.Description).ToArray();
        }

        return Results.ValidationProblem(modelStateDictionary);
    }
}
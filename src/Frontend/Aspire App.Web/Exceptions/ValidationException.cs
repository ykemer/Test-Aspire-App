namespace Aspire_App.Web.Exceptions;

public class ValidationException : Exception
{
  public ValidationException(IDictionary<string, string[]> errors)
    : base("One or more validation errors occurred.") =>
    Errors = errors;

  public IDictionary<string, string[]> Errors { get; }
}

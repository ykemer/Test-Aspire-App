namespace Aspire_App.Web.Exceptions;

public class NotFoundException : Exception
{
  public NotFoundException(string? message = null) : base(message ?? "The requested resource was not found.")
  {
  }
}

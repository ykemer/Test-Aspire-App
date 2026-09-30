using ErrorOr;

namespace Library.Messaging;

public static class ErrorOrMessagingExtensions
{
  /// <summary>
  /// Use in message consumers that must not lose data (e.g. copying classes from the Courses service).
  /// Throwing makes Rebus retry the message a few times and then move it to the "error" queue,
  /// where someone can see it, instead of the failure being silently ignored.
  /// </summary>
  public static void ThrowIfFailed<T>(this ErrorOr<T> result, string whatWasAttempted)
  {
    if (result.IsError)
    {
      var errorCodes = string.Join(", ", result.Errors.Select(error => error.Code));
      throw new InvalidOperationException($"{whatWasAttempted} failed: {errorCodes}");
    }
  }
}

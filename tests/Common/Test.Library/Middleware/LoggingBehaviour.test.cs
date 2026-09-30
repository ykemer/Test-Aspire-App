using ErrorOr;

using Library.Middleware;

using Mediator;

using Microsoft.Extensions.Logging;

namespace Test.Library.Middleware;

[TestFixture]
public class LoggingBehaviourTests
{
  private const string SecretEmail = "jane.doe@example.com";

  public record RegisterStudentRequest(string Email) : IRequest<ErrorOr<Success>>;

  private ListLogger _logger = null!;
  private IPipelineBehavior<RegisterStudentRequest, ErrorOr<Success>> _behaviour = null!;

  [SetUp]
  public void SetUp()
  {
    _logger = new ListLogger();
    _behaviour = new LoggingBehaviour<RegisterStudentRequest, ErrorOr<Success>>(_logger);
  }

  private ValueTask<ErrorOr<Success>> Run(ErrorOr<Success> handlerResult) =>
    _behaviour.Handle(new RegisterStudentRequest(SecretEmail), (_, _) => ValueTask.FromResult(handlerResult),
      CancellationToken.None);

  [Test]
  public async Task Success_LogsRequestName_ButNotItsContent()
  {
    await Run(Result.Success);

    Assert.That(_logger.Entries, Has.Some.Contains(nameof(RegisterStudentRequest)));
    Assert.That(_logger.Entries, Has.None.Contains(SecretEmail));
  }

  [Test]
  public async Task BusinessError_IsLoggedAsWarning_WithCode()
  {
    await Run(Error.Conflict("students.already_exists", $"Student {SecretEmail} already exists"));

    Assert.That(_logger.Levels, Is.EqualTo(new[] { LogLevel.Warning }));
    Assert.That(_logger.Entries.Single(), Does.Contain("students.already_exists"));
    Assert.That(_logger.Entries, Has.None.Contains(SecretEmail), "descriptions can contain personal data");
  }

  [Test]
  public async Task UnexpectedError_IsLoggedAsError()
  {
    await Run(Error.Unexpected("boom"));

    Assert.That(_logger.Levels, Is.EqualTo(new[] { LogLevel.Error }));
  }

  private class ListLogger : ILogger<LoggingBehaviour<RegisterStudentRequest, ErrorOr<Success>>>
  {
    public List<string> Entries { get; } = [];
    public List<LogLevel> Levels { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
      Func<TState, Exception?, string> formatter)
    {
      Levels.Add(logLevel);
      Entries.Add(formatter(state, exception));
    }
  }
}

using ErrorOr;

using Grpc.Core;

using Library.GRPC;

using Microsoft.Extensions.Logging.Abstractions;

namespace Test.Library.GRPC;

[TestFixture]
public class GrpcErrorHandlerTests
{
  private static RpcException Convert(Error error) =>
    GrpcErrorHandler.ThrowAndLogRpcException([error], NullLogger.Instance);

  [Test]
  public void NotFound_MapsToNotFound_WithDescription()
  {
    var exception = Convert(Error.NotFound("code", "Course 1 was not found."));

    Assert.That(exception.StatusCode, Is.EqualTo(StatusCode.NotFound));
    Assert.That(exception.Status.Detail, Is.EqualTo("Course 1 was not found."));
  }

  [Test]
  public void Validation_MapsToInvalidArgument() =>
    Assert.That(Convert(Error.Validation("code", "bad")).StatusCode, Is.EqualTo(StatusCode.InvalidArgument));

  [Test]
  public void PlainConflict_MapsToFailedPrecondition() =>
    Assert.That(Convert(Error.Conflict("code", "busy")).StatusCode, Is.EqualTo(StatusCode.FailedPrecondition));

  [Test]
  public void AlreadyExistsConflict_MapsToAlreadyExists() =>
    Assert.That(Convert(ConflictErrors.AlreadyExists("code", "dup")).StatusCode,
      Is.EqualTo(StatusCode.AlreadyExists));

  [Test]
  public void ConcurrentUpdateConflict_MapsToAborted() =>
    Assert.That(Convert(ConflictErrors.ConcurrentUpdate("code", "retry")).StatusCode,
      Is.EqualTo(StatusCode.Aborted));

  [Test]
  public void StatePreventsActionConflict_MapsToFailedPrecondition() =>
    Assert.That(Convert(ConflictErrors.StatePreventsAction("code", "full")).StatusCode,
      Is.EqualTo(StatusCode.FailedPrecondition));

  [Test]
  public void ConflictHelpers_StillHaveConflictType() =>
    Assert.That(ConflictErrors.AlreadyExists("code", "dup").Type, Is.EqualTo(ErrorType.Conflict));

  [TestCase(ErrorType.Failure)]
  [TestCase(ErrorType.Unexpected)]
  public void UnexpectedErrors_MapToInternal_AndHideDetails(ErrorType type)
  {
    var error = Error.Custom((int)type, "code", "Npgsql connection to 10.0.0.5 failed");

    var exception = Convert(error);

    Assert.That(exception.StatusCode, Is.EqualTo(StatusCode.Internal));
    Assert.That(exception.Status.Detail, Is.EqualTo(GrpcErrorHandler.InternalErrorMessage));
  }
}

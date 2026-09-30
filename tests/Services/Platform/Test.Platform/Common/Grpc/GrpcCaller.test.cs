using Grpc.Core;

using Platform.Common.Grpc;
using Platform.Common.Responses;

namespace Test.Platform.Common.Grpc;

[TestFixture]
public class GrpcCallerTests
{
  private static int HttpStatusFor(StatusCode grpcStatus) =>
    ErrorOrResponseSender.ToStatusCode(GrpcCaller.ToError(new Status(grpcStatus, "detail")).Type);

  [TestCase(StatusCode.NotFound, 404)]
  [TestCase(StatusCode.InvalidArgument, 400)]
  [TestCase(StatusCode.AlreadyExists, 409)]
  [TestCase(StatusCode.FailedPrecondition, 409)]
  [TestCase(StatusCode.Aborted, 409)]
  [TestCase(StatusCode.PermissionDenied, 403)]
  [TestCase(StatusCode.Unavailable, 503)]
  [TestCase(StatusCode.DeadlineExceeded, 503)]
  [TestCase(StatusCode.Internal, 500)]
  public void GrpcStatus_BecomesTheRightHttpStatus(StatusCode grpcStatus, int httpStatus) =>
    Assert.That(HttpStatusFor(grpcStatus), Is.EqualTo(httpStatus));

  [Test]
  public void Unauthenticated_FromAnInternalService_IsA500_NotA401()
  {
    // It means OUR internal API key was rejected. A 401 would make the frontend sign the user out.
    Assert.That(HttpStatusFor(StatusCode.Unauthenticated), Is.EqualTo(500));
  }

  [Test]
  public void BusinessErrors_KeepTheDownstreamMessage() =>
    Assert.That(GrpcCaller.ToError(new Status(StatusCode.NotFound, "Course 1 was not found.")).Description,
      Is.EqualTo("Course 1 was not found."));

  [Test]
  public void UnexpectedErrors_DoNotLeakDetails() =>
    Assert.That(GrpcCaller.ToError(new Status(StatusCode.Internal, "NullReferenceException at X.cs:42")).Description,
      Does.Not.Contain("X.cs"));
}

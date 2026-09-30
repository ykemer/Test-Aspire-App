using Grpc.Core;

using Service.Students.Features.DeleteStudent;
using Service.Students.Features.GetStudent;

using StudentsGRPC;

namespace Test.Students.Features;

/// <summary>
/// Every gRPC mapper must turn bad ids into "InvalidArgument" instead of crashing.
/// </summary>
[TestFixture]
public class GrpcMappersRejectBadInputTests
{
  private static void AssertInvalidArgument(TestDelegate action)
  {
    var exception = Assert.Throws<RpcException>(action);
    Assert.That(exception!.StatusCode, Is.EqualTo(StatusCode.InvalidArgument));
  }

  [Test]
  public void GetStudent_RejectsBadId() =>
    AssertInvalidArgument(() => new GrpcGetStudentByIdRequest { Id = "bad" }.ToGetStudentQuery());

  [Test]
  public void DeleteStudent_RejectsBadId() =>
    AssertInvalidArgument(() => new GrpcDeleteStudentRequest { Id = "" }.ToDeleteStudentCommand());
}

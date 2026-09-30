using EnrollmentsGRPC;

using Grpc.Core;

using Service.Enrollments.Features.Enrollments.GetClassEnrollments;
using Service.Enrollments.Features.Enrollments.GetCourseEnrollments;
using Service.Enrollments.Features.Enrollments.GetStudentEnrollments;

namespace Test.Enrollments.Features.Enrollments;

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
  public void GetClassEnrollments_RejectsBadClassId() =>
    AssertInvalidArgument(() => new GrpcGetClassEnrollmentsRequest
    {
      CourseId = Guid.NewGuid().ToString(), ClassId = "bad"
    }.ToGetClassEnrollmentsQuery());

  [Test]
  public void GetCourseEnrollments_RejectsBadCourseId() =>
    AssertInvalidArgument(() => new GrpcGetCourseEnrollmentsRequest { CourseId = "" }.ToGetCourseEnrollmentsQuery());

  [Test]
  public void GetStudentEnrollments_RejectsBadStudentId() =>
    AssertInvalidArgument(() => new GrpcGetStudentEnrollmentsRequest { StudentId = "bad" }
      .ToGetStudentEnrollmentsQuery());

  [Test]
  public void GetStudentEnrollments_RejectsBadOptionalCourseId() =>
    AssertInvalidArgument(() => new GrpcGetStudentEnrollmentsRequest
    {
      StudentId = Guid.NewGuid().ToString(), CourseId = "bad"
    }.ToGetStudentEnrollmentsQuery());

  [Test]
  public void GetStudentEnrollments_TreatsMissingCourseIdAsAllCourses()
  {
    var query = new GrpcGetStudentEnrollmentsRequest { StudentId = Guid.NewGuid().ToString() }
      .ToGetStudentEnrollmentsQuery();

    Assert.That(query.CourseId, Is.Null);
  }
}

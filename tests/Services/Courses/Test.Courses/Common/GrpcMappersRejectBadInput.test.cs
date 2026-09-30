using ClassesGRPC;

using CoursesGRPC;

using Grpc.Core;

using Service.Courses.Features.Classes.GetClass;
using Service.Courses.Features.Classes.ListClasses;
using Service.Courses.Features.Courses.GetCourse;
using Service.Courses.Features.Courses.ListCourses;

namespace Test.Courses.Common;

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
  public void GetCourseMapper_RejectsBadId() =>
    AssertInvalidArgument(() => new GrpcGetCourseRequest { Id = "bad" }.ToGetCourseQuery());

  [Test]
  public void GetClassMapper_RejectsBadCourseId() =>
    AssertInvalidArgument(() =>
      new GrpcGetClassRequest { Id = Guid.NewGuid().ToString(), CourseId = "bad" }.ToGetClassQuery());

  [Test]
  public void ListClassesMapper_RejectsBadCourseId() =>
    AssertInvalidArgument(() => new GrpcListClassRequest { CourseId = "bad" }.ToListClassesQuery());

  [Test]
  public void ListCoursesMapper_RejectsBadEnrolledClassId() =>
    AssertInvalidArgument(() =>
      new GrpcListCoursesRequest { EnrolledClasses = { "bad" } }.ToListCoursesQuery());
}

using EnrollmentsGRPC;

using Service.Enrollments.Features.Enrollments.GetCourseEnrollments;

namespace Test.Enrollments.Features.Enrollments.GetCourseEnrollments;

[TestFixture]
public class GetCourseEnrollmentsMapperTests
{
  [Test]
  public void MapToGetCourseEnrollmentsQuery_MapsFields()
  {
    var req = new GrpcGetCourseEnrollmentsRequest { CourseId = "course-1" };
    var query = req.MapToGetCourseEnrollmentsQuery();
    Assert.That(query.CourseId, Is.EqualTo("course-1"));
  }
}

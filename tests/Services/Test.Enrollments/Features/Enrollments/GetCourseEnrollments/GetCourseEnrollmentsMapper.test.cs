using EnrollmentsGRPC;

using Service.Enrollments.Features.Enrollments.GetCourseEnrollments;

namespace Test.Enrollments.Features.Enrollments.GetCourseEnrollments;

[TestFixture]
public class GetCourseEnrollmentsMapperTests
{
  [Test]
  public void MapToGetCourseEnrollmentsQuery_MapsFields()
  {
    var courseId = Guid.NewGuid();
    var req = new GrpcGetCourseEnrollmentsRequest { CourseId = courseId.ToString() };
    var query = req.MapToGetCourseEnrollmentsQuery();
    Assert.That(query.CourseId, Is.EqualTo(courseId));
  }
}

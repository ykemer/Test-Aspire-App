using EnrollmentsGRPC;

using Service.Enrollments.Features.Enrollments.GetCourseEnrollments;

namespace Test.Enrollments.Features.Enrollments.GetCourseEnrollments;

[TestFixture]
public class GetCourseEnrollmentsMapperTests
{
  [Test]
  public void ToGetCourseEnrollmentsQuery_MapsFields()
  {
    var courseId = Guid.NewGuid();
    var req = new GrpcGetCourseEnrollmentsRequest { CourseId = courseId.ToString() };
    var query = req.ToGetCourseEnrollmentsQuery();
    Assert.That(query.CourseId, Is.EqualTo(courseId));
  }
}

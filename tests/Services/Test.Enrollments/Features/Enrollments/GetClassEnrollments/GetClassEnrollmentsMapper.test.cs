using EnrollmentsGRPC;

using Service.Enrollments.Features.Enrollments.GetClassEnrollments;

namespace Test.Enrollments.Features.Enrollments.GetClassEnrollments;

[TestFixture]
public class GetClassEnrollmentsMapperTests
{
  [Test]
  public void MapToGetClassEnrollmentsQuery_MapsFields()
  {
    var courseId = Guid.NewGuid();
    var classId = Guid.NewGuid();
    var req = new GrpcGetClassEnrollmentsRequest { CourseId = courseId.ToString(), ClassId = classId.ToString() };
    var query = req.MapToGetClassEnrollmentsQuery();
    Assert.That(query.CourseId, Is.EqualTo(courseId));
    Assert.That(query.ClassId, Is.EqualTo(classId));
  }
}

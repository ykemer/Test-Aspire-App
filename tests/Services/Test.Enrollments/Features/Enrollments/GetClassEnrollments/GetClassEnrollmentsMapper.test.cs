using EnrollmentsGRPC;

using Service.Enrollments.Features.Enrollments.GetClassEnrollments;

namespace Test.Enrollments.Features.Enrollments.GetClassEnrollments;

[TestFixture]
public class GetClassEnrollmentsMapperTests
{
  [Test]
  public void MapToGetClassEnrollmentsQuery_MapsFields()
  {
    var req = new GrpcGetClassEnrollmentsRequest { CourseId = "course-1", ClassId = "class-1" };
    var query = req.MapToGetClassEnrollmentsQuery();
    Assert.That(query.CourseId, Is.EqualTo("course-1"));
    Assert.That(query.ClassId, Is.EqualTo("class-1"));
  }
}

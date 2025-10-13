using FizzWare.NBuilder;

using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Courses;

namespace Courses.Application.Features.Courses;

[TestFixture]
public class CourseExtensionMethodsTest
{
  [Test]
  public void MapToGrpcCourseResponse_MapsCorrectly()
  {
    var course = Builder<Course>.CreateNew()
      .With(c => c.Id = "test-id")
      .With(c => c.Name = "Test Course")
      .With(c => c.Description = "Desc")
      .With(c => c.TotalStudents = 42)
      .Build();

    var grpcResponse = course.MapToGrpcCourseResponse();

    Assert.That(grpcResponse.Id, Is.EqualTo(course.Id));
    Assert.That(grpcResponse.Name, Is.EqualTo(course.Name));
    Assert.That(grpcResponse.Description, Is.EqualTo(course.Description));
    Assert.That(grpcResponse.TotalStudents, Is.EqualTo(course.TotalStudents));
  }

  [Test]
  public void MapToGrpcClassResponse_MapsCorrectly()
  {
    var courseClass = Builder<Class>.CreateNew()
      .With(c => c.Id = "class-id")
      .With(c => c.CourseId = "course-id")
      .With(c => c.RegistrationDeadline = DateTime.UtcNow.AddDays(1))
      .With(c => c.CourseStartDate = DateTime.UtcNow.AddDays(2))
      .With(c => c.CourseEndDate = DateTime.UtcNow.AddDays(3))
      .With(c => c.MaxStudents = 30)
      .With(c => c.TotalStudents = 10)
      .Build();

    var grpcResponse = courseClass.MapToGrpcClassResponse();

    Assert.That(grpcResponse.Id, Is.EqualTo(courseClass.Id));
    Assert.That(grpcResponse.CourseId, Is.EqualTo(courseClass.CourseId));
    Assert.That(grpcResponse.MaxStudents, Is.EqualTo(courseClass.MaxStudents));
    Assert.That(grpcResponse.TotalStudents, Is.EqualTo(courseClass.TotalStudents));
    Assert.That(grpcResponse.RegistrationDeadline.ToDateTime(), Is.EqualTo(courseClass.RegistrationDeadline.ToUniversalTime()));
    Assert.That(grpcResponse.CourseStartDate.ToDateTime(), Is.EqualTo(courseClass.CourseStartDate.ToUniversalTime()));
    Assert.That(grpcResponse.CourseEndDate.ToDateTime(), Is.EqualTo(courseClass.CourseEndDate.ToUniversalTime()));
  }
}

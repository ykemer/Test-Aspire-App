using Contracts.Common;

using CoursesGRPC;

using ErrorOr;

using FizzWare.NBuilder;

using Grpc.Core;

using Mediator;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Database.Entities;
using Service.Courses.Features.Courses;
using Service.Courses.Features.Courses.CreateCourse;
using Service.Courses.Features.Courses.DeleteCourse;
using Service.Courses.Features.Courses.GetCourse;
using Service.Courses.Features.Courses.ListCourses;
using Service.Courses.Features.Courses.UpdateCourse;

namespace Courses.Application.Features.Courses;

[TestFixture]
public class CoursesServiceTests
{
  [SetUp]
  public void Setup()
  {
    _loggerMock = Substitute.For<ILogger<CoursesService>>();
    _mediatorMock = Substitute.For<IMediator>();
    _coursesService = new CoursesService(_loggerMock, _mediatorMock);
    _context = Substitute.For<ServerCallContext>();
    _testError = Error.Unexpected("Test error message");
  }

  private ILogger<CoursesService> _loggerMock;
  private IMediator _mediatorMock;
  private CoursesService _coursesService;
  private ServerCallContext _context;
  private Error _testError;

  [Test]
  public async Task CreateCourse_Success_ReturnsCourseResponse()
  {
    // Arrange
    var request = Builder<GrpcCreateCourseRequest>
      .CreateNew()
      .With(x => x.Name = "Test Course")
      .With(x => x.Description = "Test Description")
      .Build();

    var course = Builder<Course>
      .CreateNew()
      .With(x => x.Name, request.Name)
      .With(x => x.Description, request.Description)
      .Build();

    _mediatorMock
      .Send(Arg.Any<CreateCourseCommand>(), Arg.Any<CancellationToken>())
      .Returns(course);

    // Act
    var response = await _coursesService.CreateCourse(request, _context);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(response, Is.Not.Null);
      Assert.That(response.Id, Is.EqualTo(course.Id));
      Assert.That(response.Name, Is.EqualTo(course.Name));
      Assert.That(response.Description, Is.EqualTo(course.Description));
    });
  }

  [Test]
  public void CreateCourse_Error_ThrowsRpcException()
  {
    // Arrange
    var request = Builder<GrpcCreateCourseRequest>
      .CreateNew()
      .With(x => x.Name, "Test Course")
      .Build();

    _mediatorMock
      .Send(Arg.Any<CreateCourseCommand>(), Arg.Any<CancellationToken>())
      .Returns(_testError);

    // Act & Assert
    var ex = Assert.ThrowsAsync<RpcException>(async () =>
      await _coursesService.CreateCourse(request, _context));

    Assert.That(ex.Status.Detail, Does.Contain(_testError.Description));
  }

  [Test]
  public async Task UpdateCourse_Success_ReturnsSuccessResponse()
  {
    // Arrange
    var request = Builder<GrpcUpdateCourseRequest>
      .CreateNew()
      .With(x => x.Id, "course-id")
      .With(x => x.Name = "Updated Course")
      .Build();

    _mediatorMock
      .Send(Arg.Any<UpdateCourseCommand>(), Arg.Any<CancellationToken>())
      .Returns(Result.Updated);

    // Act
    var response = await _coursesService.UpdateCourse(request, _context);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(response.Success, Is.True);
      Assert.That(response.Message, Is.EqualTo("Course updated successfully"));
    });
  }

  [Test]
  public void UpdateCourse_Error_ThrowsRpcException()
  {
    // Arrange
    var request = Builder<GrpcUpdateCourseRequest>
      .CreateNew()
      .With(x => x.Id, "course-id")
      .Build();

    _mediatorMock
      .Send(Arg.Any<UpdateCourseCommand>(), Arg.Any<CancellationToken>())
      .Returns(_testError);

    // Act & Assert
    var ex = Assert.ThrowsAsync<RpcException>(async () =>
      await _coursesService.UpdateCourse(request, _context));

    Assert.That(ex.Status.Detail, Does.Contain(_testError.Description));
  }


  [Test]
  public async Task DeleteCourse_Success_ReturnsSuccessResponse()
  {
    // Arrange
    var request = Builder<GrpcDeleteCourseRequest>
      .CreateNew()
      .With(x => x.Id, "course-id")
      .Build();

    _mediatorMock
      .Send(Arg.Any<DeleteCourseCommand>(), Arg.Any<CancellationToken>())
      .Returns(Result.Deleted);

    // Act
    var response = await _coursesService.DeleteCourse(request, _context);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(response.Success, Is.True);
      Assert.That(response.Message, Is.EqualTo("Course deleted successfully"));
    });
  }

  [Test]
  public void DeleteCourse_Error_ThrowsRpcException()
  {
    // Arrange
    var request = Builder<GrpcDeleteCourseRequest>
      .CreateNew()
      .With(x => x.Id, "course-id")
      .Build();

    _mediatorMock
      .Send(Arg.Any<DeleteCourseCommand>(), Arg.Any<CancellationToken>())
      .Returns(_testError);

    // Act & Assert
    var ex = Assert.ThrowsAsync<RpcException>(async () =>
      await _coursesService.DeleteCourse(request, _context));

    Assert.That(ex.Status.Detail, Does.Contain(_testError.Description));
  }

  [Test]
  public async Task GetCourse_Success_ReturnsCourseResponse()
  {
    // Arrange
    var request = Builder<GrpcGetCourseRequest>
      .CreateNew()
      .With(x => x.Id, "course-id")
      .Build();

    var course = Builder<Course>
      .CreateNew()
      .With(x => x.Id, "course-id")
      .Build();

    _mediatorMock
      .Send(Arg.Any<GetCourseQuery>(), Arg.Any<CancellationToken>())
      .Returns(course);

    // Act
    var response = await _coursesService.GetCourse(request, _context);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(response, Is.Not.Null);
      Assert.That(response.Id, Is.EqualTo(course.Id));
      Assert.That(response.Name, Is.EqualTo(course.Name));
      Assert.That(response.Description, Is.EqualTo(course.Description));
      Assert.That(response.TotalStudents, Is.EqualTo(course.TotalStudents));
    });
  }

  [Test]
  public void GetCourse_Error_ThrowsRpcException()
  {
    // Arrange
    var request = Builder<GrpcGetCourseRequest>
      .CreateNew()
      .With(x => x.Id, "course-id")
      .Build();

    _mediatorMock
      .Send(Arg.Any<GetCourseQuery>(), Arg.Any<CancellationToken>())
      .Returns(_testError);

    // Act & Assert
    var ex = Assert.ThrowsAsync<RpcException>(async () =>
      await _coursesService.GetCourse(request, _context));

    Assert.That(ex.Status.Detail, Does.Contain(_testError.Description));
  }

  [Test]
  public async Task ListCourses_Success_ReturnsListResponse()
  {
    // Arrange
    GrpcListCoursesRequest? request = new();
    var courses = Builder<Course>
      .CreateListOfSize(2)
      .All()
      .With(x => x.Name = $"Course {x.Id}")
      .Build();

    _mediatorMock
      .Send(Arg.Any<ListCoursesRequest>(), Arg.Any<CancellationToken>())
      .Returns(PagedList<Course>.Create(courses, 1, 10));

    // Act
    var response = await _coursesService.ListCourses(request, _context);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(response, Is.Not.Null);
      Assert.That(response.Items, Has.Count.EqualTo(2));

      Assert.That(response.Items[0].Id, Is.EqualTo(courses[0].Id));
      Assert.That(response.Items[0].Name, Is.EqualTo(courses[0].Name));
      Assert.That(response.Items[0].Description, Is.EqualTo(courses[0].Description));
      Assert.That(response.Items[0].TotalStudents, Is.EqualTo(courses[0].TotalStudents));

      Assert.That(response.Items[1].Id, Is.EqualTo(courses[1].Id));
      Assert.That(response.Items[1].Name, Is.EqualTo(courses[1].Name));
      Assert.That(response.Items[1].Description, Is.EqualTo(courses[1].Description));
      Assert.That(response.Items[1].TotalStudents, Is.EqualTo(courses[1].TotalStudents));

    });
  }

  [Test]
  public void ListCourses_Error_ThrowsRpcException()
  {
    // Arrange
    GrpcListCoursesRequest? request = new();

    _mediatorMock
      .Send(Arg.Any<ListCoursesRequest>(), Arg.Any<CancellationToken>())
      .Returns(_testError);

    // Act & Assert
    var ex = Assert.ThrowsAsync<RpcException>(async () =>
      await _coursesService.ListCourses(request, _context));

    Assert.That(ex.Status.Detail, Does.Contain(_testError.Description));
  }
}

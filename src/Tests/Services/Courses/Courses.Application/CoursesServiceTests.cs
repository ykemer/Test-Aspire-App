using Contracts.Common;

using CoursesGRPC;

using ErrorOr;

using FizzWare.NBuilder;

using Grpc.Core;

using MediatR;

using Microsoft.Extensions.Logging;

using Moq;

using Service.Courses.Entities;
using Service.Courses.Features.Courses;
using Service.Courses.Features.Courses.CreateCourse;
using Service.Courses.Features.Courses.DeleteCourse;
using Service.Courses.Features.Courses.GetCourse;
using Service.Courses.Features.Courses.ListCourses;
using Service.Courses.Features.Courses.UpdateCourse;

namespace Courses.Application;

[TestFixture]
public class CoursesServiceTests
{
  [SetUp]
  public void Setup()
  {
    _loggerMock = new Mock<ILogger<CoursesService>>();
    _mediatorMock = new Mock<IMediator>();
    _coursesService = new CoursesService(_loggerMock.Object, _mediatorMock.Object);
    _context = new Mock<ServerCallContext>().Object;
    _testError = Error.Unexpected("Test error message");
  }

  private Mock<ILogger<CoursesService>> _loggerMock;
  private Mock<IMediator> _mediatorMock;
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
      .Setup(m => m.Send(It.IsAny<CreateCourseCommand>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(course);

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
      .Setup(m => m.Send(It.IsAny<CreateCourseCommand>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(_testError);

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
      .Setup(m => m.Send(It.IsAny<UpdateCourseCommand>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(Result.Updated);

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
      .Setup(m => m.Send(It.IsAny<UpdateCourseCommand>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(_testError);

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
      .Setup(m => m.Send(It.IsAny<DeleteCourseCommand>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(Result.Deleted);

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
      .Setup(m => m.Send(It.IsAny<DeleteCourseCommand>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(_testError);

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
      .Setup(m => m.Send(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(course);

    // Act
    var response = await _coursesService.GetCourse(request, _context);

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
  public void GetCourse_Error_ThrowsRpcException()
  {
    // Arrange
    var request = Builder<GrpcGetCourseRequest>
      .CreateNew()
      .With(x => x.Id, "course-id")
      .Build();

    _mediatorMock
      .Setup(m => m.Send(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(_testError);

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
    IList<Course>? courses = Builder<Course>
      .CreateListOfSize(2)
      .All()
      .With(x => x.Name = $"Course {x.Id}")
      .Build();

    _mediatorMock
      .Setup(m => m.Send(It.IsAny<ListCoursesRequest>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(PagedList<Course>.Create(courses, 1, 10));

    // Act
    var response = await _coursesService.ListCourses(request, _context);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(response, Is.Not.Null);
      Assert.That(response.Items, Has.Count.EqualTo(2));
      Assert.That(response.Items[0].Id, Is.EqualTo(courses[0].Id));
      Assert.That(response.Items[1].Id, Is.EqualTo(courses[1].Id));
    });
  }

  [Test]
  public void ListCourses_Error_ThrowsRpcException()
  {
    // Arrange
    GrpcListCoursesRequest? request = new();

    _mediatorMock
      .Setup(m => m.Send(It.IsAny<ListCoursesRequest>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(_testError);

    // Act & Assert
    var ex = Assert.ThrowsAsync<RpcException>(async () =>
      await _coursesService.ListCourses(request, _context));

    Assert.That(ex.Status.Detail, Does.Contain(_testError.Description));
  }
}

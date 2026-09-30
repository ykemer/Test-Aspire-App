using Contracts.Common;
using Contracts.Students.Events;

using ErrorOr;

using FizzWare.NBuilder;

using Grpc.Core;

using Mediator;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Rebus.Bus;

using Service.Students.Common.Database.Entities;
using Service.Students.Features;
using Service.Students.Features.DeleteStudent;
using Service.Students.Features.GetStudent;
using Service.Students.Features.ListStudents;

using StudentsGRPC;

namespace Test.Students.Features;

[TestFixture]
public class StudentsServiceTests
{
  [SetUp]
  public void Setup()
  {
    _mediatorMock = Substitute.For<IMediator>();
    _loggerMock = Substitute.For<ILogger<StudentsService>>();
    _busMock = Substitute.For<IBus>();
    _studentsService = new StudentsService(_loggerMock, _mediatorMock, _busMock);
    _context = Substitute.For<ServerCallContext>();
  }

  [TearDown]
  public void TearDown() => _busMock.Dispose();

  private IMediator _mediatorMock = null!;
  private ILogger<StudentsService> _loggerMock = null!;
  private IBus _busMock = null!;
  private StudentsService _studentsService = null!;
  private ServerCallContext _context = null!;

  [Test]
  public async Task GetStudentById_ValidId_ReturnsStudent()
  {
    // Arrange
    var student = Builder<Student>.CreateNew()
      .With(s => s.EnrollmentsCount, 3)
      .Build();

    var request = new GrpcGetStudentByIdRequest { Id = student.Id.ToString() };

    _mediatorMock
      .Send(Arg.Any<GetStudentQuery>(), Arg.Any<CancellationToken>())
      .Returns(student);

    // Act
    var response = await _studentsService.GetStudentById(request, _context);

    // Assert
    Assert.That(response.Id, Is.EqualTo(student.Id.ToString()));
    Assert.That(response.EnrolledCourses, Is.EqualTo(3), "Platform shows this number to the user");
  }

  [Test]
  public void GetStudentById_ErrorCase_ThrowsRpcException()
  {
    // Arrange
    var error = Error.NotFound(description: "Student not found");
    var request = new GrpcGetStudentByIdRequest { Id = Guid.NewGuid().ToString() };

    _mediatorMock
      .Send(Arg.Any<GetStudentQuery>(), Arg.Any<CancellationToken>())
      .Returns(error);

    // Act & Assert
    Assert.ThrowsAsync<RpcException>(() => _studentsService.GetStudentById(request, _context));
  }

  [Test]
  public async Task ListStudents_ValidRequest_ReturnsStudentsList()
  {
    // Arrange
    var students = Builder<Student>.CreateListOfSize(5)
      .All()
      .Build()
      .ToList();

    var pagedList = new PagedList<Student>(students, students.Count, 1, 5);
    var request = new GrpcListStudentsRequest { Page = 1, PageSize = 5 };

    _mediatorMock
      .Send(Arg.Any<ListStudentsQuery>(), Arg.Any<CancellationToken>())
      .Returns(pagedList);

    // Act
    var response = await _studentsService.ListStudents(request, _context);

    // Assert
    Assert.That(response.Items, Has.Count.EqualTo(5));
  }

  [Test]
  public void ListStudents_ErrorCase_ThrowsRpcException()
  {
    // Arrange
    var error = Error.Failure(description: "Failed to retrieve students");
    var request = new GrpcListStudentsRequest { Page = 1, PageSize = 5 };

    _mediatorMock
      .Send(Arg.Any<ListStudentsQuery>(), Arg.Any<CancellationToken>())
      .Returns(error);

    // Act & Assert
    Assert.ThrowsAsync<RpcException>(() => _studentsService.ListStudents(request, _context));
  }

  [Test]
  public async Task DeleteStudent_ValidRequest_ReturnsSuccessResponse()
  {
    // Arrange
    var studentId = Guid.NewGuid();
    var request = new GrpcDeleteStudentRequest { Id = studentId.ToString() };

    _mediatorMock
      .Send(Arg.Any<DeleteStudentCommand>(), Arg.Any<CancellationToken>())
      .Returns(Result.Deleted);

    // Act
    var response = await _studentsService.DeleteStudent(request, _context);

    // Assert
    Assert.That(response.Updated, Is.True);
    Assert.That(response.Message, Is.EqualTo("Student deleted successfully"));
    await _busMock.Received(1).Publish(Arg.Is<StudentDeletedEvent>(e => e.StudentId == studentId));
  }

  [Test]
  public void DeleteStudent_ErrorCase_ThrowsRpcException()
  {
    // Arrange
    var error = Error.NotFound(description: "Student not found");
    var request = new GrpcDeleteStudentRequest { Id = Guid.NewGuid().ToString() };

    _mediatorMock
      .Send(Arg.Any<DeleteStudentCommand>(), Arg.Any<CancellationToken>())
      .Returns(error);

    // Act & Assert
    Assert.ThrowsAsync<RpcException>(() => _studentsService.DeleteStudent(request, _context));
    _busMock.DidNotReceiveWithAnyArgs().Publish(default!);
  }
}

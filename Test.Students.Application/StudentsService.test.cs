using Contracts.Common;

using ErrorOr;

using FizzWare.NBuilder;

using Grpc.Core;

using MediatR;

using Microsoft.Extensions.Logging;

using Moq;

using Service.Students.Entities;
using Service.Students.Features;
using Service.Students.Features.DeleteStudent;
using Service.Students.Features.GetStudent;
using Service.Students.Features.ListStudent;

using StudentsGRPC;

namespace Test.Students.Application;

[TestFixture]
public class StudentsServiceTests
{
  private Mock<IMediator> _mediatorMock;
  private Mock<ILogger<StudentsService>> _loggerMock;
  private StudentsService _studentsService;

  [SetUp]
  public void Setup()
  {
    _mediatorMock = new Mock<IMediator>();
    _loggerMock = new Mock<ILogger<StudentsService>>();
    _studentsService = new StudentsService(_loggerMock.Object, _mediatorMock.Object);
  }

  [Test]
  public async Task GetStudentById_ValidId_ReturnsStudent()
  {
    // Arrange
    var student = Builder<Student>.CreateNew()
      .Build();

    var request = new GrpcGetStudentByIdRequest { Id = student.Id.ToString() };
    var serverCallContext = Mock.Of<ServerCallContext>();

    _mediatorMock
      .Setup(m => m.Send(It.IsAny<GetStudentQuery>(), default))
      .ReturnsAsync(student);

    // Act
    var response = await _studentsService.GetStudentById(request, serverCallContext);

    // Assert
    Assert.That(response.Id, Is.EqualTo(student.Id.ToString()));
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
    var serverCallContext = Mock.Of<ServerCallContext>();

    _mediatorMock
      .Setup(m => m.Send(It.IsAny<ListStudentsQuery>(), default))
      .ReturnsAsync(pagedList);

    // Act
    var response = await _studentsService.ListStudents(request, serverCallContext);

    // Assert
    Assert.That(response.Items, Has.Count.EqualTo(5));
  }

  [Test]
  public async Task DeleteStudent_ValidRequest_ReturnsSuccessResponse()
  {
    // Arrange
    var studentId = Guid.NewGuid();
    var request = new GrpcDeleteStudentRequest { Id = studentId.ToString() };
    var serverCallContext = Mock.Of<ServerCallContext>();

    _mediatorMock
      .Setup(m => m.Send(It.IsAny<DeleteStudentCommand>(), default))
      .ReturnsAsync(Result.Deleted);

    // Act
    var response = await _studentsService.DeleteStudent(request, serverCallContext);

    // Assert
    Assert.That(response.Updated, Is.True);
    Assert.That(response.Message, Is.EqualTo("Student deleted successfully"));
  }

  [Test]
  public void GetStudentById_ErrorCase_ThrowsRpcException()
  {
    // Arrange
    var error = Error.NotFound(description: "Student not found");
    var request = new GrpcGetStudentByIdRequest { Id = Guid.NewGuid().ToString() };
    var serverCallContext = Mock.Of<ServerCallContext>();

    _mediatorMock
      .Setup(m => m.Send(It.IsAny<GetStudentQuery>(), default))
      .ReturnsAsync(error);

    // Act & Assert
    Assert.ThrowsAsync<RpcException>(() => _studentsService.GetStudentById(request, serverCallContext));
  }
}

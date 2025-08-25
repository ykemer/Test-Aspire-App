using ClassesGRPC;

using ErrorOr;

using FizzWare.NBuilder;

using Grpc.Core;

using Mediator;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Database.Entities;
using Service.Courses.Features.Classes;
using Service.Courses.Features.Classes.CreateClass;
using Service.Courses.Features.Classes.DeleteClass;
using Service.Courses.Features.Classes.GetClass;
using Service.Courses.Features.Classes.ListClasses;
using Service.Courses.Features.Classes.UpdateClass;

namespace Courses.Application.Features.Classes;

[TestFixture]
public class ClassesServiceTests
{
  private ILogger<ClassesService> _loggerMock = null!;
  private IMediator _mediatorMock = null!;
  private ClassesService _service = null!;
  private ServerCallContext _context = null!;
  private Error _testError;

  [SetUp]
  public void Setup()
  {
    _loggerMock = Substitute.For<ILogger<ClassesService>>();
    _mediatorMock = Substitute.For<IMediator>();
    _service = new ClassesService(_loggerMock, _mediatorMock);
    _context = Substitute.For<ServerCallContext>();
    _testError = Error.Unexpected("Test error");
  }

  [Test]
  public async Task GetClass_Success_ReturnsResponse()
  {
    var cls = Builder<Class>.CreateNew().Build();
    _mediatorMock.Send(Arg.Any<GetClassQuery>(), Arg.Any<CancellationToken>()).Returns(cls);

    var grpcReq = new GrpcGetClassRequest { Id = cls.Id, CourseId = cls.CourseId };
    var resp = await _service.GetClass(grpcReq, _context);

    Assert.That(resp.Id, Is.EqualTo(cls.Id));
    Assert.That(resp.CourseId, Is.EqualTo(cls.CourseId));
  }

  [Test]
  public void GetClass_Error_ThrowsRpcException()
  {
    _mediatorMock.Send(Arg.Any<GetClassQuery>(), Arg.Any<CancellationToken>()).Returns(_testError);

    var grpcReq = new GrpcGetClassRequest { Id = "x", CourseId = "y" };
    Assert.ThrowsAsync<RpcException>(async () => await _service.GetClass(grpcReq, _context));
  }

  [Test]
  public async Task ListClasses_Success_ReturnsResponse()
  {
    var cls = Builder<Class>.CreateNew().Build();
    var paged = Contracts.Common.PagedList<Class>.Create(new List<Class> { cls }.AsQueryable(), 1, 10);
    _mediatorMock.Send(Arg.Any<ListClassesQuery>(), Arg.Any<CancellationToken>()).Returns(paged);

    var grpcReq = new GrpcListClassRequest { CourseId = cls.CourseId, Page = 1, PageSize = 10 };
    var resp = await _service.ListClasses(grpcReq, _context);

    Assert.That(resp.Items.Count, Is.EqualTo(1));
    Assert.That(resp.Items[0].Id, Is.EqualTo(cls.Id));
  }

  [Test]
  public void ListClasses_Error_ThrowsRpcException()
  {
    _mediatorMock.Send(Arg.Any<ListClassesQuery>(), Arg.Any<CancellationToken>()).Returns(_testError);

    var grpcReq = new GrpcListClassRequest { CourseId = "c1" };
    Assert.ThrowsAsync<RpcException>(async () => await _service.ListClasses(grpcReq, _context));
  }

  [Test]
  public async Task CreateClass_Success_ReturnsClassResponse()
  {
    var cls = Builder<Class>.CreateNew().Build();
    _mediatorMock.Send(Arg.Any<CreateClassCommand>(), Arg.Any<CancellationToken>()).Returns(cls);

    var now = DateTime.UtcNow;
    var grpcReq = new GrpcCreateClassRequest
    {
      CourseId = "course-1",
      RegistrationDeadline = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(now.AddDays(-5), DateTimeKind.Utc)),
      CourseStartDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(now.AddDays(-4), DateTimeKind.Utc)),
      CourseEndDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(now.AddDays(-3), DateTimeKind.Utc)),
      MaxStudents = 10
    };
    var resp = await _service.CreateClass(grpcReq, _context);

    Assert.That(resp.Id, Is.EqualTo(cls.Id));
  }

  [Test]
  public void CreateClass_Error_ThrowsRpcException()
  {
    _mediatorMock.Send(Arg.Any<CreateClassCommand>(), Arg.Any<CancellationToken>()).Returns(_testError);

    var now = DateTime.UtcNow;
    var grpcReq = new GrpcCreateClassRequest
    {
      CourseId = "course-1",
      RegistrationDeadline = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(now.AddDays(-5), DateTimeKind.Utc)),
      CourseStartDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(now.AddDays(-4), DateTimeKind.Utc)),
      CourseEndDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(now.AddDays(-3), DateTimeKind.Utc)),
      MaxStudents = 10
    };
    Assert.ThrowsAsync<RpcException>(async () => await _service.CreateClass(grpcReq, _context));
  }

  [Test]
  public async Task UpdateClass_Success_ReturnsUpdatedResponse()
  {
    _mediatorMock.Send(Arg.Any<UpdateClassCommand>(), Arg.Any<CancellationToken>()).Returns(new Updated());

    var now = DateTime.UtcNow;
    var grpcReq = new GrpcUpdateClassRequest
    {
      Id = "class-1",
      CourseId = "course-1",
      RegistrationDeadline = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(now.AddDays(-5), DateTimeKind.Utc)),
      CourseStartDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(now.AddDays(-4), DateTimeKind.Utc)),
      CourseEndDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(now.AddDays(-3), DateTimeKind.Utc)),
      MaxStudents = 10
    };
    var resp = await _service.UpdateClass(grpcReq, _context);

    Assert.That(resp.Success, Is.True);
  }

  [Test]
  public void UpdateClass_Error_ThrowsRpcException()
  {
    _mediatorMock.Send(Arg.Any<UpdateClassCommand>(), Arg.Any<CancellationToken>()).Returns(_testError);

    var now = DateTime.UtcNow;
    var grpcReq = new GrpcUpdateClassRequest
    {
      Id = "class-1",
      CourseId = "course-1",
      RegistrationDeadline = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(now.AddDays(-5), DateTimeKind.Utc)),
      CourseStartDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(now.AddDays(-4), DateTimeKind.Utc)),
      CourseEndDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(now.AddDays(-3), DateTimeKind.Utc)),
      MaxStudents = 10
    };
    Assert.ThrowsAsync<RpcException>(async () => await _service.UpdateClass(grpcReq, _context));
  }

  [Test]
  public async Task DeleteClass_Success_ReturnsUpdatedResponse()
  {
    _mediatorMock.Send(Arg.Any<DeleteClassCommand>(), Arg.Any<CancellationToken>()).Returns(new Deleted());

    var grpcReq = Builder<GrpcDeleteClassRequest>.CreateNew().Build();
    var resp = await _service.DeleteClass(grpcReq, _context);

    Assert.That(resp.Success, Is.True);
  }

  [Test]
  public void DeleteClass_Error_ThrowsRpcException()
  {
    _mediatorMock.Send(Arg.Any<DeleteClassCommand>(), Arg.Any<CancellationToken>()).Returns(_testError);

    var grpcReq = Builder<GrpcDeleteClassRequest>.CreateNew().Build();
    Assert.ThrowsAsync<RpcException>(async () => await _service.DeleteClass(grpcReq, _context));
  }
}

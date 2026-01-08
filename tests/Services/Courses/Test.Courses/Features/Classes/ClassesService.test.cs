using ClassesGRPC;

using Contracts.Common;

using ErrorOr;

using FizzWare.NBuilder;

using Grpc.Core;

using Mediator;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Classes;
using Service.Courses.Features.Classes.GetClass;
using Service.Courses.Features.Classes.ListClasses;

namespace Courses.Application.Features.Classes;

[TestFixture]
public class ClassesServiceTests
{
  [SetUp]
  public void Setup()
  {
    _loggerMock = Substitute.For<ILogger<ClassesService>>();
    _mediatorMock = Substitute.For<IMediator>();
    _service = new ClassesService(_loggerMock, _mediatorMock);
    _context = Substitute.For<ServerCallContext>();
    _testError = Error.Unexpected("Test error");
  }

  private ILogger<ClassesService> _loggerMock = null!;
  private IMediator _mediatorMock = null!;
  private ClassesService _service = null!;
  private ServerCallContext _context = null!;
  private Error _testError;

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
    var paged = PagedList<Class>.Create(new List<Class> { cls }.AsQueryable(), 1, 10);
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
}

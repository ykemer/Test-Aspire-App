using EnrollmentsGRPCClient;

using Grpc.Core;

using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

using Platform.Common.Grpc;

using Test.Platform.Setup;

namespace Test.Platform.Common.Grpc;

[TestFixture]
public class CurrentStudentEnrollmentsTests
{
  private GrpcEnrollmentsService.GrpcEnrollmentsServiceClient _client = null!;
  private CurrentStudentEnrollments _enrollments = null!;

  [SetUp]
  public void SetUp()
  {
    _client = Substitute.For<GrpcEnrollmentsService.GrpcEnrollmentsServiceClient>();
    _enrollments = new CurrentStudentEnrollments(_client, new GrpcCaller(NullLogger<GrpcCaller>.Instance));
  }

  private static AsyncUnaryCall<T> Reply<T>(T response) =>
    new(Task.FromResult(response), Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => [], () => { });

  [Test]
  public async Task Administrator_GetsNoEnrollments_AndNoCallIsMade()
  {
    var result = await _enrollments.GetAsync(TestUsers.Administrator(), CancellationToken.None);

    Assert.That(result.Value, Is.SameAs(StudentEnrollmentIds.None));
    _ = _client.DidNotReceiveWithAnyArgs().GetStudentEnrollmentsAsync(default!, cancellationToken: default);
  }

  [Test]
  public async Task Student_GetsClassIdsAndCourseIds_OfTheirOwnEnrollments()
  {
    var studentId = Guid.NewGuid();
    _client.GetStudentEnrollmentsAsync(
        Arg.Is<GrpcGetStudentEnrollmentsRequest>(r => r.StudentId == studentId.ToString()),
        cancellationToken: Arg.Any<CancellationToken>())
      .Returns(Reply(new GrpcListEnrollmentsResponse
      {
        Items =
        {
          new GrpcEnrollmentResponse { ClassId = "class-1", CourseId = "course-1" },
          new GrpcEnrollmentResponse { ClassId = "class-2", CourseId = "course-1" }
        }
      }));

    var result = await _enrollments.GetAsync(TestUsers.Student(studentId), CancellationToken.None);

    Assert.That(result.Value.ClassIds, Is.EqualTo(new[] { "class-1", "class-2" }));
    Assert.That(result.Value.CourseIds, Is.EqualTo(new[] { "course-1" }), "each course only once");
  }
}

using Grpc.Core;

using Library.GRPC;

namespace Test.Library.GRPC;

[TestFixture]
public class GrpcInputTests
{
  private static void AssertInvalidArgument(TestDelegate action)
  {
    var exception = Assert.Throws<RpcException>(action);
    Assert.That(exception!.StatusCode, Is.EqualTo(StatusCode.InvalidArgument));
  }

  [Test]
  public void ParseGuid_ReturnsGuid_WhenValid()
  {
    var id = Guid.NewGuid();

    Assert.That(GrpcInput.ParseGuid(id.ToString(), "Id"), Is.EqualTo(id));
  }

  [TestCase("")]
  [TestCase("not-a-guid")]
  public void ParseGuid_ThrowsInvalidArgument_WhenNotAGuid(string value) =>
    AssertInvalidArgument(() => GrpcInput.ParseGuid(value, "Id"));

  [Test]
  public void ParseGuidList_ThrowsInvalidArgument_WhenTooManyItems()
  {
    var tooMany = Enumerable.Range(0, GrpcInput.MaxIdsPerRequest + 1).Select(_ => Guid.NewGuid().ToString()).ToList();

    AssertInvalidArgument(() => GrpcInput.ParseGuidList(tooMany, "EnrolledClasses"));
  }

  [Test]
  public void ParseGuidList_ThrowsInvalidArgument_WhenOneItemIsNotAGuid() =>
    AssertInvalidArgument(() => GrpcInput.ParseGuidList([Guid.NewGuid().ToString(), "oops"], "EnrolledClasses"));
}

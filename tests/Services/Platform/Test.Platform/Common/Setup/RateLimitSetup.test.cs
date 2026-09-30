using System.Net;

using Microsoft.AspNetCore.Http;

using Platform.Common.Setup;

using Test.Platform.Setup;

namespace Test.Platform.Common.Setup;

[TestFixture]
public class RateLimitSetupTests
{
  [Test]
  public void SignedInUsers_EachGetTheirOwnBudget()
  {
    var userId = Guid.NewGuid();
    var context = new DefaultHttpContext { User = TestUsers.Student(userId) };

    Assert.That(RateLimitSetup.GetUserPartitionKey(context), Is.EqualTo($"user:{userId}"));
  }

  [Test]
  public void AnonymousCallers_AreGroupedByIpAddress()
  {
    var context = new DefaultHttpContext();
    context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.7");

    Assert.That(RateLimitSetup.GetUserPartitionKey(context), Is.EqualTo("ip:10.0.0.7"));
  }
}

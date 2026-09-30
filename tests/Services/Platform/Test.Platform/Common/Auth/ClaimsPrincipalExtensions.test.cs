using System.Security.Claims;

using Platform.Common.Auth;

using Test.Platform.Setup;

namespace Test.Platform.Common.Auth;

[TestFixture]
public class ClaimsPrincipalExtensionsTests
{
  [Test]
  public void Administrator_IsDetected_EvenWhenAdminIsNotTheFirstRole()
  {
    // The old code only looked at the FIRST role claim, so [User, Administrator] was not an admin.
    var user = TestUsers.WithRoles(Guid.NewGuid(), Roles.User, Roles.Administrator);

    Assert.That(user.IsAdministrator(), Is.True);
  }

  [Test]
  public void Student_IsNotAdministrator() => Assert.That(TestUsers.Student().IsAdministrator(), Is.False);

  [Test]
  public void GetUserId_ReturnsTheIdFromTheToken()
  {
    var id = Guid.NewGuid();

    Assert.That(TestUsers.Student(id).GetUserId(), Is.EqualTo(id));
  }

  [Test]
  public void GetUserId_Throws_WhenTheTokenHasNoUserId() =>
    Assert.Throws<InvalidOperationException>(() => new ClaimsPrincipal(new ClaimsIdentity()).GetUserId());
}

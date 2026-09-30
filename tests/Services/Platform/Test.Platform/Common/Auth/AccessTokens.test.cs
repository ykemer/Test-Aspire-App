using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Microsoft.IdentityModel.JsonWebTokens;

using NSubstitute;

using Platform.Common.Auth;
using Platform.Common.Database.Entities;

namespace Test.Platform.Common.Auth;

[TestFixture]
public class AccessTokensTests
{
  private static readonly JwtOptions s_options = new()
  {
    SignKey = "a-test-signing-key-that-is-long-enough-0123456789", Issuer = "test-issuer", Audience = "test-audience"
  };

  private static UserManager<ApplicationUser> UserManagerWithRoles(params string[] roles)
  {
    var userManager = Substitute.For<UserManager<ApplicationUser>>(
      Substitute.For<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
    userManager.GetRolesAsync(Arg.Any<ApplicationUser>()).Returns(roles.ToList());
    return userManager;
  }

  private static ApplicationUser User() =>
    new() { Id = Guid.NewGuid().ToString(), FirstName = "Jane", LastName = "Doe" };

  [Test]
  public async Task CreatedToken_IsAccepted_ByTheSameValidationRules_AndCarriesIdAndRoles()
  {
    var user = User();
    var factory = new AccessTokenFactory(UserManagerWithRoles(Roles.User, Roles.Administrator),
      Options.Create(s_options), TimeProvider.System);

    var token = await factory.CreateAsync(user);
    var validation = await new JsonWebTokenHandler().ValidateTokenAsync(token.Token,
      s_options.CreateValidationParameters());

    Assert.That(validation.IsValid, Is.True, validation.Exception?.Message);
    var principal = new ClaimsPrincipal(validation.ClaimsIdentity);
    Assert.Multiple(() =>
    {
      Assert.That(principal.GetUserId().ToString(), Is.EqualTo(user.Id));
      Assert.That(principal.IsAdministrator(), Is.True);
      Assert.That(token.ExpiresInSeconds, Is.EqualTo(15 * 60));
    });
  }

  [Test]
  public async Task TokenSignedWithAnotherKey_IsRejected()
  {
    var otherKeyOptions = new JwtOptions
    {
      SignKey = "some-other-signing-key-that-is-long-enough-987654", Issuer = s_options.Issuer,
      Audience = s_options.Audience
    };
    var token = await new AccessTokenFactory(UserManagerWithRoles(), Options.Create(otherKeyOptions),
      TimeProvider.System).CreateAsync(User());

    var validation = await new JsonWebTokenHandler().ValidateTokenAsync(token.Token,
      s_options.CreateValidationParameters());

    Assert.That(validation.IsValid, Is.False);
  }

  [Test]
  public async Task ExpiredToken_IsRejected()
  {
    var clock = new FakeTimeProvider(DateTimeOffset.UtcNow.AddHours(-1));
    var token = await new AccessTokenFactory(UserManagerWithRoles(), Options.Create(s_options), clock)
      .CreateAsync(User());

    var validation = await new JsonWebTokenHandler().ValidateTokenAsync(token.Token,
      s_options.CreateValidationParameters());

    Assert.That(validation.IsValid, Is.False, "a token that expired 45 minutes ago must not work");
  }

  [TestCase("")]
  [TestCase("JWT_SIGN_KEY")]
  public void ShortSigningKey_FailsValidation(string signKey)
  {
    var options = new JwtOptions { SignKey = signKey, Issuer = "issuer", Audience = "audience" };

    var isValid = Validator.TryValidateObject(options, new ValidationContext(options), [], validateAllProperties: true);

    Assert.That(isValid, Is.False, "the service must refuse to start with a missing or weak key");
  }

  [Test]
  public void RefreshTokenHash_IsStable_AndDoesNotContainTheToken()
  {
    const string refreshToken = "my-secret-refresh-token";

    var hash = RefreshTokenHasher.Hash(refreshToken);

    Assert.That(RefreshTokenHasher.Hash(refreshToken), Is.EqualTo(hash));
    Assert.That(hash, Does.Not.Contain(refreshToken));
    Assert.That(hash, Has.Length.EqualTo(44));
  }
}

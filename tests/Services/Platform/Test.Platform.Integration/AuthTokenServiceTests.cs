using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

using Platform.Common.Auth;
using Platform.Common.Database;
using Platform.Common.Database.Entities;

namespace Test.Platform.Integration;

/// <summary>
/// Refresh tokens against a real database: stored only as a hash, usable once (even in parallel),
/// revocable, and useless after they expire.
/// </summary>
public class AuthTokenServiceTests
{
  private static readonly DateTime s_now = new(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc);

  private ServiceProvider _services = null!;
  private FakeTimeProvider _clock = null!;
  private ApplicationUser _user = null!;

  [OneTimeSetUp]
  public async Task MigrateDatabase()
  {
    await using var dbContext = PostgresDatabase.CreateDbContext();
    await dbContext.Database.MigrateAsync();
  }

  [SetUp]
  public async Task SetUp()
  {
    _clock = new FakeTimeProvider(s_now);

    var services = new ServiceCollection();
    services.AddLogging();
    services.AddSingleton<TimeProvider>(_clock);
    services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(PostgresDatabase.ConnectionString));
    services.AddIdentityCore<ApplicationUser>().AddRoles<IdentityRole>()
      .AddEntityFrameworkStores<ApplicationDbContext>();
    services.AddSingleton(Options.Create(new JwtOptions
    {
      SignKey = "a-test-signing-key-that-is-long-enough-0123456789", Issuer = "test", Audience = "test"
    }));
    services.AddScoped<IAccessTokenFactory, AccessTokenFactory>();
    services.AddScoped<IAuthTokenService, AuthTokenService>();
    _services = services.BuildServiceProvider();

    await using var scope = _services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.RefreshTokens.ExecuteDeleteAsync();

    _user = new ApplicationUser
    {
      UserName = $"{Guid.NewGuid()}@example.com", Email = "user@example.com", FirstName = "Jane", LastName = "Doe"
    };
    var created = await scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>().CreateAsync(_user);
    Assert.That(created.Succeeded, Is.True);
  }

  [TearDown]
  public async Task TearDown() => await _services.DisposeAsync();

  /// <summary>Runs one call with its own scope (own DbContext), like one HTTP request.</summary>
  private async Task<T> InNewRequest<T>(Func<IAuthTokenService, Task<T>> action)
  {
    await using var scope = _services.CreateAsyncScope();
    return await action(scope.ServiceProvider.GetRequiredService<IAuthTokenService>());
  }

  private Task<string> SignIn() =>
    InNewRequest(async tokens => (await tokens.IssueTokensAsync(_user, CancellationToken.None)).RefreshToken);

  [Test]
  public async Task OnlyTheHashIsStored_NeverTheToken()
  {
    var refreshToken = await SignIn();

    await using var dbContext = PostgresDatabase.CreateDbContext();
    var stored = await dbContext.RefreshTokens.SingleAsync();
    Assert.That(stored.TokenHash, Is.EqualTo(RefreshTokenHasher.Hash(refreshToken)));
    Assert.That(stored.TokenHash, Is.Not.EqualTo(refreshToken));
  }

  [Test]
  public async Task Refresh_GivesANewToken_AndTheOldOneStopsWorking()
  {
    var firstToken = await SignIn();

    var refreshed = await InNewRequest(tokens => tokens.RefreshAsync(firstToken, CancellationToken.None));
    var reused = await InNewRequest(tokens => tokens.RefreshAsync(firstToken, CancellationToken.None));

    Assert.That(refreshed.IsError, Is.False);
    Assert.That(refreshed.Value.RefreshToken, Is.Not.EqualTo(firstToken));
    Assert.That(reused.FirstError, Is.EqualTo(AuthErrors.InvalidRefreshToken));
  }

  [Test]
  public async Task SameTokenUsedInParallel_OnlyOneRequestSucceeds()
  {
    var refreshToken = await SignIn();

    var results = await Task.WhenAll(Enumerable.Range(0, 5)
      .Select(_ => InNewRequest(tokens => tokens.RefreshAsync(refreshToken, CancellationToken.None))));

    Assert.That(results.Count(result => !result.IsError), Is.EqualTo(1),
      "a stolen token used at the same time as the real one must not give two sessions");
  }

  [Test]
  public async Task RevokedToken_CannotBeUsed()
  {
    var refreshToken = await SignIn();

    var revoked = await InNewRequest(tokens => tokens.RevokeAsync(refreshToken, CancellationToken.None));
    var refreshed = await InNewRequest(tokens => tokens.RefreshAsync(refreshToken, CancellationToken.None));

    Assert.That(revoked.IsError, Is.False);
    Assert.That(refreshed.FirstError, Is.EqualTo(AuthErrors.InvalidRefreshToken));
  }

  [Test]
  public async Task ExpiredToken_CannotBeUsed()
  {
    var refreshToken = await SignIn();
    _clock.Advance(JwtOptions.RefreshTokenLifetime + TimeSpan.FromMinutes(1));

    var refreshed = await InNewRequest(tokens => tokens.RefreshAsync(refreshToken, CancellationToken.None));

    Assert.That(refreshed.FirstError, Is.EqualTo(AuthErrors.InvalidRefreshToken));
  }

  [Test]
  public async Task UnknownToken_CannotBeUsed()
  {
    var refreshed = await InNewRequest(tokens => tokens.RefreshAsync("made-up-token", CancellationToken.None));

    Assert.That(refreshed.FirstError, Is.EqualTo(AuthErrors.InvalidRefreshToken));
  }

  [Test]
  public async Task SigningIn_RemovesTheUsersExpiredTokens()
  {
    await SignIn();
    _clock.Advance(JwtOptions.RefreshTokenLifetime + TimeSpan.FromMinutes(1));

    await SignIn();

    await using var dbContext = PostgresDatabase.CreateDbContext();
    Assert.That(await dbContext.RefreshTokens.CountAsync(), Is.EqualTo(1), "only the new token is left");
  }
}

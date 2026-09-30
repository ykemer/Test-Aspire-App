using System.ComponentModel.DataAnnotations;

using Grpc.Core;

using Library.GRPC.InternalApi;

using Microsoft.Extensions.Options;

namespace Test.Library.GRPC;

[TestFixture]
public class InternalApiKeyInterceptorTests
{
  private const string CorrectKey = "a-very-long-and-random-internal-api-key-0123456789";

  private InternalApiKeyInterceptor _interceptor = null!;

  [SetUp]
  public void SetUp() =>
    _interceptor = new InternalApiKeyInterceptor(Options.Create(new InternalApiOptions { Key = CorrectKey }));

  private Task<string> CallWithHeaders(Metadata headers) =>
    _interceptor.UnaryServerHandler<string, string>(
      "request",
      new FakeServerCallContext(headers),
      (_, _) => Task.FromResult("handler was called"));

  [Test]
  public async Task CorrectKey_LetsTheCallThrough()
  {
    var response = await CallWithHeaders(new Metadata { { InternalApiOptions.HeaderName, CorrectKey } });

    Assert.That(response, Is.EqualTo("handler was called"));
  }

  [Test]
  public void MissingKey_IsRejectedAsUnauthenticated()
  {
    var exception = Assert.ThrowsAsync<RpcException>(() => CallWithHeaders([]));

    Assert.That(exception!.StatusCode, Is.EqualTo(StatusCode.Unauthenticated));
  }

  [TestCase("wrong")]
  [TestCase(CorrectKey + "x")]
  [TestCase("")]
  public void WrongKey_IsRejectedAsUnauthenticated(string sentKey)
  {
    var exception = Assert.ThrowsAsync<RpcException>(() =>
      CallWithHeaders(new Metadata { { InternalApiOptions.HeaderName, sentKey } }));

    Assert.That(exception!.StatusCode, Is.EqualTo(StatusCode.Unauthenticated));
  }

  [TestCase("")]
  [TestCase("too-short")]
  public void ShortOrMissingConfiguredKey_FailsValidation(string configuredKey)
  {
    var options = new InternalApiOptions { Key = configuredKey };

    var isValid = Validator.TryValidateObject(options, new ValidationContext(options), [], validateAllProperties: true);

    Assert.That(isValid, Is.False);
  }
}

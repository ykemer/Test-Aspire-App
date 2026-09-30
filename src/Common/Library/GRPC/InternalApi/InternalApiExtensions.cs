using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Library.GRPC.InternalApi;

public static class InternalApiExtensions
{
  /// <summary>
  /// Reads the internal API key from configuration ("InternalApi:Key").
  /// The service refuses to start if the key is missing or too short.
  /// Call once per service. <see cref="AddGrpcForInternalCallers"/> already calls it.
  /// </summary>
  public static IServiceCollection AddInternalApiKey(this IServiceCollection services)
  {
    services.AddOptions<InternalApiOptions>()
      .BindConfiguration(InternalApiOptions.SectionName)
      .ValidateDataAnnotations()
      .ValidateOnStart();
    return services;
  }

  /// <summary>
  /// Server side: adds gRPC and requires the internal API key on every call.
  /// </summary>
  public static IServiceCollection AddGrpcForInternalCallers(this IServiceCollection services)
  {
    services.AddInternalApiKey();
    services.AddSingleton<InternalApiKeyInterceptor>();
    services.AddGrpc(options => options.Interceptors.Add<InternalApiKeyInterceptor>());
    return services;
  }

  /// <summary>
  /// Client side: registers a gRPC client for one of our own services (found by its Aspire name, over HTTPS)
  /// that sends the internal API key with every call. Requires <see cref="AddInternalApiKey"/>.
  /// Use this instead of a plain AddGrpcClient, so the key can never be forgotten.
  /// </summary>
  public static IHttpClientBuilder AddInternalGrpcClient<TClient>(this IServiceCollection services,
    string serviceName) where TClient : class =>
    services
      .AddGrpcClient<TClient>(options => options.Address = new Uri($"https://{serviceName}"))
      .WithInternalApiKey();

  /// <summary>
  /// Client side: sends the internal API key with every call made by this gRPC client.
  /// Call credentials are only sent over HTTPS, so the key never travels in plain text.
  /// </summary>
  public static IHttpClientBuilder WithInternalApiKey(this IHttpClientBuilder clientBuilder) =>
    clientBuilder.AddCallCredentials((_, metadata, serviceProvider) =>
    {
      var key = serviceProvider.GetRequiredService<IOptions<InternalApiOptions>>().Value.Key;
      metadata.Add(InternalApiOptions.HeaderName, key);
      return Task.CompletedTask;
    });
}

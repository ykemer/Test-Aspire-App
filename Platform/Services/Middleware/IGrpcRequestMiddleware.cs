using Grpc.Core;

namespace Platform.Services.Middleware;

public interface IGrpcRequestMiddleware
{
    Task<ErrorOr<TResponse>> SendGrpcRequestAsync<TResponse>(AsyncUnaryCall<TResponse> grpcCall, CancellationToken ct);
}
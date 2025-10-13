using Grpc.Core;

namespace Platform.Common.Middleware.Grpc;

public interface IGrpcRequestMiddleware
{
  Task<ErrorOr<TResponse>> SendGrpcRequestAsync<TResponse>(AsyncUnaryCall<TResponse> grpcCall, CancellationToken ct);
}

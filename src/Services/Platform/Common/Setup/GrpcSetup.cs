using ClassesGRPCClient;

using CoursesGRPCClient;

using EnrollmentsGRPCClient;

using Library.GRPC.InternalApi;

using Platform.Common.Grpc;

using StudentsGRPCClient;

namespace Platform.Common.Setup;

public static class GrpcSetup
{
  public static IServiceCollection AddGrpcClients(this IServiceCollection services)
  {
    services.AddScoped<IGrpcCaller, GrpcCaller>();
    services.AddScoped<ICurrentStudentEnrollments, CurrentStudentEnrollments>();

    // Our gRPC services reject callers without the internal API key (see AppHost "internalApiKey").
    // AddInternalGrpcClient attaches that key to every call.
    services.AddInternalApiKey();
    services.AddInternalGrpcClient<GrpcCoursesService.GrpcCoursesServiceClient>("coursesService");
    services.AddInternalGrpcClient<GrpcClassService.GrpcClassServiceClient>("coursesService");
    services.AddInternalGrpcClient<GrpcEnrollmentsService.GrpcEnrollmentsServiceClient>("enrollmentsService");
    services.AddInternalGrpcClient<GrpcStudentsService.GrpcStudentsServiceClient>("studentsService");
    return services;
  }
}

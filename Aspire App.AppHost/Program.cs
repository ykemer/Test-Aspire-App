var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");
var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

var apiService = builder
    .AddProject<Projects.Aspire_App_ApiService>("apiservice")
    .WithReference(postgresdb);

builder.AddProject<Projects.Aspire_App_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);




builder.Build().Run();

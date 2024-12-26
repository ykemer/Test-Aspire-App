using Aspire.Hosting;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);
var cache = builder.AddRedis("cache");


// var sqlPassword = builder.AddParameter("sql-password", secret: true);
//
// var sqldb = builder
//     .AddSqlServer("sql", sqlPassword)
//     .WithDataVolume("ms-sql-aspire-volume-2")
//     .AddDatabase("sqlDb");



var postgres = builder.AddPostgres("postgres").WithPgAdmin();
var postgresdb = postgres.AddDatabase("postgresdb");


var apiService = builder
    .AddProject<Projects.Aspire_App_ApiService>("apiservice")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.Aspire_App_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);




builder.Build().Run();

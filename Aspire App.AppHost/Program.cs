using Aspire.Hosting;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);



var sqlPassword = builder.AddParameter("sql-password", secret: true);
var cache = builder.AddRedis("cache");
var sql = builder
    .AddSqlServer("sql", sqlPassword)
    .WithDataVolume()
    .AddDatabase("sqlDb");

//var sql = builder.AddSqlServer("database");

//var postgres = builder.AddPostgres("postgres").WithPgAdmin();
//var postgresdb = postgres.AddDatabase("postgresdb");


var apiService = builder
    .AddProject<Projects.Aspire_App_ApiService>("apiservice")
    .WithReference(sql)
    .WaitFor(sql);

builder.AddProject<Projects.Aspire_App_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);




builder.Build().Run();

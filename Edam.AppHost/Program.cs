var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Edam_ApiService>("apiservice");

var redis = builder.AddRedis("redis");

builder.AddProject<Projects.Edam_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();

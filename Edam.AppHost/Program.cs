var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Edam_ApiService>("apiservice");

var sql = builder.AddSqlServer("edam");
var sqldb = sql.AddDatabase("edamdb");

var redis = builder.AddRedis("redis");

var maildev = builder.AddMailDev("maildev");

builder.AddProject<Projects.Edam_Web>("edamStudioFrontEnd")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(maildev);

builder.Build().Run();

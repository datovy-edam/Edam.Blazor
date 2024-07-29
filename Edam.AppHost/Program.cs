var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Edam_ApiService>("apiservice");
var fileSystemService = builder.AddProject<
   Projects.Edam_Data_FileSystemService>("apifilesystemservice");

var sql = builder.AddSqlServer("edam");
var edamDb = sql.AddDatabase("edamdb");
var fileSystemDb = sql.AddDatabase("fileSystemDb");

var redis = builder.AddRedis("redis");

var maildev = builder.AddMailDev("maildev");

builder.AddProject<Projects.Edam_Web>("edamStudioFrontEnd")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(fileSystemService)
    .WithReference(maildev);

builder.Build().Run();

using Edam.Data.FileSystemDb;
using Edam.Data.FileSystemService;
using Edam.Data.FileSystemModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddProblemDetails();

builder.AddSqlServerDbContext<FileSystemContext>("fileSystemDb");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// setup service container 
WebAppService appService = new WebAppService(app);

#region -- 1.50 - Initialization and Session Management

// this should be called first...
app.MapGet("/filesystemservice/session/info", (
   string sessionId, string containerId) =>
{
   var container = appService.FileSystem.SetContainer(sessionId, containerId);
   return container;
});

#endregion
#region -- 4.00 - Container Support

// get container info
app.MapGet("/filesystemservice/container/info", (
   string sessionId, string containerId) =>
{
   var container = appService.FileSystem.SetContainer(sessionId, containerId);
   return container;
});

// get container list
app.MapGet("/filesystemservice/container/list", (string sessionId) =>
{
   WebAppService.SetupSession(sessionId);
   if (sessionId != WebAppService.SessionId)
   {
      return new List<ContainerInfo>();
   }
   return appService.FileSystem.GetContainerList();
});

// get container enlisted details
app.MapGet("/filesystemservice/container/enlist", (
   string sessionId, string containerId, string description) =>
{
   WebAppService.SetupSession(sessionId);
   if (sessionId != WebAppService.SessionId)
   {
      return new ContainerInfo();
   }
   return appService.FileSystem.EnlistContainer(containerId, description);
});

// get container delisted details
app.MapGet("/filesystemservice/container/delist", (
   string sessionId, string containerId, string description, 
   string? statusCode = null) =>
{
   WebAppService.SetupSession(sessionId);
   if (sessionId != WebAppService.SessionId)
   {
      return new ContainerInfo();
   }
   return appService.FileSystem.DelistContainer(containerId);
});

#endregion

// setup services
//app.MapGet("/filesystemservice", () =>
//{
//   ///appService.SetSession(sesssionId);
//   return "active";
//});

app.MapDefaultEndpoints();

app.Run();

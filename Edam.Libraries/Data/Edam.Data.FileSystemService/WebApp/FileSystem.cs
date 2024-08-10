using Edam.Application;
using Edam.Data.FileSystemDb;
using Edam.Data.FileSystemModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Edam.Data.FileSystemService;

public class FileSystem : FileSystemInstance, IDisposable
{

   public override ContainerInfo? SetContainer(
      string containerId, string sessionId)
   {
      WebAppService.SetupSession(String.IsNullOrWhiteSpace(sessionId) ?
         new Guid().ToString() : sessionId);
      return base.SetContainer(containerId, sessionId);
   }
}

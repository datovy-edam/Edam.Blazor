using Edam.Application;
using Edam.Data.FileSystemDb;
using Edam.Data.FileSystemModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Edam.Data.FileSystemService;

public class FileSystem : IDisposable
{
   public const string DELETED = "Deleted";

   private FileSystemContext? _dbContext { get; set; } = null;
   private ContainerInfo _DefaultContainer { get; set; }
   private ContainerInfo _CurrentContainer { get; set; }

   private void InitializeDbContext()
   {
      var connectionString = AppSettings.GetConnectionString("fileSystemDb");
      _dbContext = new FileSystemContext(connectionString);
      _dbContext.Database.EnsureCreated();

      if (!_dbContext.ContentTypes.Any())
      {
         var types = new ContentTypeInfo[]
         {
         new ContentTypeInfo(
            "application/ld+json", "json-ld document"),
         new ContentTypeInfo("application/json", "json document"),
         new ContentTypeInfo("application/xml", "xml document"),
         new ContentTypeInfo("text/plain", "text document"),
         new ContentTypeInfo(
            "text/javascript", "javascript document")
         };
         foreach (var type in types)
         {
            _dbContext.ContentTypes.Add(type);
         }
         _dbContext.SaveChanges();
      }

      if (!_dbContext.Containers.Any())
      {
         _DefaultContainer = new ContainerInfo();
         _dbContext.Containers.Add(_DefaultContainer);
         _dbContext.SaveChanges();
      }
      else
      {
         GetContainer(null);
      }
      _CurrentContainer = _DefaultContainer;

   }

   /// <summary>
   /// Get Container ID.
   /// </summary>
   /// <param name="containerId"></param>
   /// <returns></returns>
   private string GetContainerId(string containerId)
   {
      if (String.IsNullOrWhiteSpace(containerId))
      {
         containerId = ContainerInfo.CONTAINER_ID_DEFAULT;
      }
      return containerId;
   }

   /// <summary>
   /// Get Container.
   /// </summary>
   /// <param name="containerId"></param>
   /// <returns></returns>
   public ContainerInfo? GetContainer(string? containerId)
   {
      var cid = GetContainerId(containerId);
      var container = _dbContext.Containers.Where(
         x => x.ContainerId == cid).FirstOrDefault();
      return container;
   }

   /// <summary>
   /// Set Session and current container (by id).
   /// </summary>
   /// <param name="containerId">(optional) container id</param>
   /// <param name="sessionId">(optional) session id</param>
   /// <returns>found container is is returned</returns>
   public ContainerInfo? SetContainer(
      string containerId, string sessionId)
   {
      var cid = GetContainerId(containerId);

      ContainerInfo container = new();
      WebAppService.SetupSession(String.IsNullOrWhiteSpace(sessionId) ?
         new Guid().ToString() : sessionId);

      if (_dbContext == null)
      {
         InitializeDbContext();
         container = _CurrentContainer;
      }

      if (container == null || container.ContainerId != cid)
      {
         container = GetContainer(cid);
      }
      return container;
   }

   /// <summary>
   /// Enlist (Add or Update) container info.
   /// </summary>
   /// <param name="containerId">container id</param>
   /// <param name="description">description</param>
   /// <returns>container info is returned</returns>
   public ContainerInfo EnlistContainer(string containerId, string description)
   {
      var container = GetContainer(containerId);
      if (container == null || container.ContainerId != containerId)
      {
         container = new();
         container.ContainerId = containerId;
         container.Description = description;
         _dbContext.Containers.Add(container);
         _dbContext.SaveChanges();
      }
      else if (container.Description != description)
      {
         container.Description += description;
         _dbContext.SaveChanges();
      }
      return container;
   }

   /// <summary>
   /// Delist (Delete) container info.
   /// </summary>
   /// <param name="containerId">container id</param>
   /// <returns>container info is returned</returns>
   public ContainerInfo DelistContainer(string containerId)
   {
      var container = GetContainer(containerId);
      if (container != null || container.ContainerId == containerId)
      {
         container.StatusCode = DELETED;
         _dbContext.Remove(container);
         _dbContext.SaveChanges();
      }
      return container;
   }

   /// <summary>
   /// Get the containers list.
   /// </summary>
   /// <returns>list is returned</returns>
   public List<ContainerInfo> GetContainerList()
   {
      return _dbContext.Containers.ToList<ContainerInfo>();
   }

   /// <summary>
   /// Release resources.
   /// </summary>
   public void Dispose()
   {
      if (_dbContext != null)
      {
         _dbContext.Dispose();
         _dbContext = null;
      }
   }
}

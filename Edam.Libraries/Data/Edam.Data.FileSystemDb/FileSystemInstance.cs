using Edam.Application;
using Edam.Data.FileSystemModel;
using Edam.DataObjects.Medias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.FileSystemDb;

/// <summary>
/// Support for File System repository inqueries and requests.
/// </summary>
public class FileSystemInstance : ICatalogService, IDisposable
{

   #region -- 1.00 - Fields and Properties

   public const string DELETED = "Deleted";
   public const string ROOT_ID = "[root]";
   public const string ROOT_PATH = "/";

   private FileSystemContext? DbContext { get; set; } = null;

   public ContainerInfo? DefaultContainer { get; set; } = null;
   public ContainerInfo? CurrentContainer { get; set; } = null;

   #endregion
   #region -- 4.00 - Initialization and Disposition

   /// <summary>
   /// Initialize Repository
   /// </summary>
   private void InitializeDbContext()
   {
      var connectionString = AppSettings.GetConnectionString("fileSystemDb");
      DbContext = new FileSystemContext(connectionString);
      if (!DbContext.Database.CanConnect())
      {
         DbContext.Database.EnsureCreated();
      }

      if (!DbContext.ContentTypes.Any())
      {
         var types = new ContentTypeInfo[]
         {
         new ContentTypeInfo(MediaContentTypeHelper.JSONLD, "json-ld document"),
         new ContentTypeInfo(MediaContentTypeHelper.JsonDocument,
            "json document"),
         new ContentTypeInfo(MediaContentTypeHelper.XmlDocument, 
            "xml document"),
         new ContentTypeInfo(MediaContentTypeHelper.XmlDocument, "xml text"),
         new ContentTypeInfo(MediaContentTypeHelper.TextFile, "text document"),
         new ContentTypeInfo(MediaContentTypeHelper.OfficeExcelXmlFile,
            "excel open xml document"),
         new ContentTypeInfo(MediaContentTypeHelper.JAVASCRIPT,
            "javascript document")
         };
         foreach (var type in types)
         {
            DbContext.ContentTypes.Add(type);
         }
         DbContext.SaveChanges();
      }

      // define default container
      if (!DbContext.Containers.Any())
      {
         DefaultContainer = new ContainerInfo();
         DbContext.Containers.Add(DefaultContainer);
         DbContext.SaveChanges();
      }
      else
      {
         DefaultContainer = GetContainer(null);
      }
      CurrentContainer = DefaultContainer;

      // define default container root item
      if (!DbContext.FileItems.Any())
      {
         CreateRootItem();
      }

   }

   /// <summary>
   /// Release resources.
   /// </summary>
   public void Dispose()
   {
      if (DbContext != null)
      {
         DbContext.Dispose();
         DbContext = null;
      }
   }

   #endregion
   #region -- 4.00 - Find Containers, Items, or Item-Data Methods

   /// <summary>
   /// Get Root Item of given container.
   /// </summary>
   /// <param name="containerId">container id</param>
   /// <returns>instance of File-Item is returned</returns>
   public FileItemInfo GetContainerRootItem(Guid id)
   {
      var items = from item in DbContext.FileItems
                  where item.Name == ROOT_ID &&
                        item.FullPath == ROOT_PATH
                  select item;
      var l = items.ToList();
      return l.Count > 0 ? l[0] : null;
   }

   /// <summary>
   /// Get Containers.
   /// </summary>
   /// <returns>list of containers is returned</returns>
   public List<ContainerInfo> GetContainers()
   {
      return DbContext.Containers.ToList<ContainerInfo>();
   }

   /// <summary>
   /// Get Container Items.
   /// </summary>
   /// <param name="id">Container ID</param>
   /// <returns></returns>
   public List<FileItemInfo> GetContainerItems(Guid id)
   {
      var items = from x in DbContext.FileItems
                  where x.ContainerId == id
                  select x;
      return items.ToList<FileItemInfo>();
   }

   /// <summary>
   /// Get Item by its unique full path.
   /// </summary>
   /// <param name="path">unique full path</param>
   /// <returns>Get list of data items for given file-item id</returns>
   public FileItemInfo GetItemByPath(string path)
   {
      var ditems = from x in DbContext.FileItems
                   where x.FullPath == path
                   select x;
      if (ditems.Any())
      {
         return ditems.ToList<FileItemInfo>()[0];
      }
      return null;
   }

   /// <summary>
   /// Get Item Data entries.
   /// </summary>
   /// <param name="id">File-Item ID</param>
   /// <returns>Get list of data items for given file-item id</returns>
   public List<FileItemDataInfo> GetItemData(Guid id)
   {
      var ditems = from x in DbContext.DataItems
                   where x.FileItemId == id
                   select x;
      return ditems.ToList<FileItemDataInfo>();
   }

   #endregion
   #region -- 4.00 - Container Support

   /// <summary>
   /// Get Container ID.
   /// </summary>
   /// <param name="containerId"></param>
   /// <returns></returns>
   private string GetContainerId(string? containerId)
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
      var container = DbContext.Containers.Where(
         x => x.ContainerId == cid).FirstOrDefault();
      return container;
   }

   /// <summary>
   /// Set Session and current container (by id).
   /// </summary>
   /// <param name="containerId">(optional) container id</param>
   /// <param name="sessionId">(optional) session id</param>
   /// <returns>found container is is returned</returns>
   public virtual ContainerInfo? SetContainer(
      string containerId, string sessionId)
   {
      var cid = GetContainerId(containerId);

      ContainerInfo container = CurrentContainer;

      if (DbContext == null)
      {
         InitializeDbContext();
         container = CurrentContainer;
      }

      if (container == null || container.ContainerId != cid)
      {
         container = GetContainer(cid);
      }

      CurrentContainer = container;
      if (DefaultContainer == null)
      {
         DefaultContainer = container;
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
         // prepare container
         container = new();
         container.ContainerId = containerId;
         container.Description = description;
         DbContext.Containers.Add(container);

         // new container is the current container...
         CurrentContainer = container;

         // add root item
         CreateRootItem();
      }
      else if (container.Description != description)
      {
         container.Description += description;
         DbContext.SaveChanges();
      }
      return container;
   }

   /// <summary>
   /// Delete Container Item-Data and Items.
   /// </summary>
   /// <param name="id">Container ID</param>
   private void DeleteContainerData(Guid id)
   {
      var fitems = GetContainerItems(id);
      foreach(var item in fitems)
      { 
         var ditems = GetItemData(item.Id);
         foreach(var ditem in ditems)
         {
            DbContext.DataItems.Remove(ditem);
         }
         DbContext.FileItems.Remove(item);
      }
      DbContext.SaveChanges();
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
         // delete all container items...
         DeleteContainerData(container.Id);

         // finally... delete container
         container.StatusCode = DELETED;
         DbContext.Remove(container);
         DbContext.SaveChanges();
      }
      return container;
   }

   /// <summary>
   /// Get Current Container.
   /// </summary>
   /// <returns>instance of container is returned</returns>
   public ContainerInfo GetCurrentContainer()
   {
      if (CurrentContainer == null)
      {
         if (DefaultContainer == null)
         {
            GetContainer(null);
         }
      }

      return CurrentContainer;
   }

   #endregion
   #region -- 4.00 - File Item Branch - Leaf Support

   /// <summary>
   /// Add item (or update) in repository based on its full path.
   /// </summary>
   /// <param name="item">add item</param>
   public FileItemInfo AddItem(FileItemInfo item)
   {
      FileItemInfo ritem;
      var iitem = GetItemByPath(item.FullPath);
      if (iitem == null)
      {
         ritem = item;
         DbContext.FileItems.Add(item);
      }
      else if (item.Id != iitem.Id)
      {
         item.Id = iitem.Id;
         ritem = iitem;
         DbContext.FileItems.Update(iitem);
      }
      else
      {
         ritem = iitem;
         return ritem;
      }
      DbContext.SaveChanges();
      return ritem;
   }

   /// <summary>
   /// Create container root item.
   /// </summary>
   /// <param name="containerId">(optional) container id, else the container Id
   /// of the current container is used</param>
   public FileItemInfo CreateRootItem(Guid? containerId = null)
   {
      Guid cid = containerId == null ? CurrentContainer.Id : containerId.Value;
      var root = GetContainerRootItem(cid);

      FileItemInfo item = null;
      if (root == null)
      {
         item = CreateBranch(ROOT_ID, "root");
         item.FullPath = ROOT_PATH;

         DbContext.FileItems.Add(item);
         DbContext.SaveChanges();
      }
      else
      {
         item = root;
      }
      return item;
   }

   /// <summary>
   /// Create Branch.
   /// </summary>
   /// <param name="name">name of branch</param>
   /// <param name="description">description</param>
   /// <returns>file item instance is returned</returns>
   public FileItemInfo CreateBranch(
      string name, string? description = null)
   {
      var desc = description ?? name;
      var item = new FileItemInfo();

      item.ContainerId = CurrentContainer.Id;
      item.Container = CurrentContainer;

      item.ItemType = DataObjects.Trees.TreeItemType.Branch;
      item.Name = name;
      item.Description = desc;
      item.FullPath = String.Empty;
      return item;
   }

   /// <summary>
   /// Create/Update Leaf.
   /// </summary>
   /// <param name="path">full path</param>
   /// <param name="name">name of branch</param>
   /// <param name="description">description</param>
   /// <returns>file item instance is returned</returns>
   public FileItemInfo CreateLeaf(string path, string name,
      Guid? id = null, string? description = null, string? dataValue = null)
   {
      if (String.IsNullOrWhiteSpace(path))
      {
         path = "//Archive";
      }

      var desc = description ?? name;
      var item = new FileItemInfo();

      item.Id = id ?? item.Id;
      item.ContainerId = CurrentContainer.Id;
      item.Container = CurrentContainer;

      item.ItemType = DataObjects.Trees.TreeItemType.Leaf;
      item.FullPath = path;
      item.Name = name;
      item.Description = desc;
      item.FullPath = String.Empty;

      if (dataValue != null)
      {
         CreateDataLeaf(item, name, null, dataValue);
      }

      return item;
   }

   /// <summary>
   /// Create/Update file item data.
   /// </summary>
   /// <param name="item">parent file item</param>
   /// <param name="name">name of branch</param>
   /// <param name="dataId">data id</param>
   /// <param name="description">description</param>
   /// <returns>file item instance is returned</returns>
   public FileItemDataInfo CreateDataLeaf(FileItemInfo item, string name,
      Guid? dataId = null, string dataValue = null)
   {
      var data = new FileItemDataInfo();

      data.Id = dataId ?? data.Id;
      data.FileItem = item;
      data.FileItemId = item.Id;
      data.Name = name;
      data.Data = dataValue;

      return data;
   }

   #endregion

}

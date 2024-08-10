using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.FileSystemModel;


public interface ICatalogService
{

   ContainerInfo DefaultContainer { get; set; }
   ContainerInfo CurrentContainer { get; set; }

   ContainerInfo GetContainer(string? containerId);
   ContainerInfo SetContainer(string containerId, string sessionId);
   ContainerInfo EnlistContainer(string containerId, string description);
   ContainerInfo DelistContainer(string containerId);

   FileItemInfo GetContainerRootItem(Guid id);
   List<ContainerInfo> GetContainers();
   List<FileItemInfo> GetContainerItems(Guid id);
   FileItemInfo GetItemByPath(string name);
   List<FileItemDataInfo> GetItemData(Guid id);

   FileItemInfo CreateRootItem(Guid? containerId = null);
   FileItemInfo CreateBranch(string name, string? description = null);
   FileItemInfo CreateLeaf(string path, string name,
      Guid? id = null, string? description = null, string? dataValue = null);

   FileItemDataInfo CreateDataLeaf(FileItemInfo item, string name,
      Guid? dataId = null, string dataValue = null);

   FileItemInfo AddItem(FileItemInfo item);
}

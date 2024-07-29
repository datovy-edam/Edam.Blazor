//using Edam.DataObjects.Trees;
using KristofferStrube.Blazor.FileSystem;
using MudBlazor;

using Edam.DataObjects.Trees;

namespace Edam.Web.FileSystemHelper
{

   public class FileSystemTree
   {
      private IStorageManagerService? _StorageManagerService;

      /// <summary>
      /// Origin Private File System.
      /// </summary>
      public IFileSystemHandle? OriginPrivateFileSystemHandle { get; set; } =
         null;

      public FileSystemItemInfo Root { get; set; } = new FileSystemItemInfo();

      public FileSystemTree(IStorageManagerService StorageManagerService)
      {
         _StorageManagerService = StorageManagerService;
      }

      /// <summary>
      /// Get Item information.
      /// </summary>
      /// <param name="handle">file handle</param>
      /// <returns>instance of FileSystemItemInfo is returned</returns>
      public async Task<FileSystemItemInfo> GetItemAsync(
         IFileSystemHandle handle)
      {
         var info = new FileSystemItemInfo();

         var name = await handle.GetNameAsync();
         var kind = await handle.GetKindAsync();

         info.Name = name;
         info.Title = name;
         info.Type = kind == FileSystemHandleKind.File ?
            TreeItemType.Leaf : TreeItemType.Branch;
         info.Children = new HashSet<FileSystemItemInfo>();
         info.Tag = null;
         info.Handle = handle;

         switch (info.Type)
         {
            case TreeItemType.Leaf:
               info.Icon = Icons.Material.Outlined.ListAlt;
               break;
            case TreeItemType.Branch:
               info.Icon = Icons.Material.Outlined.Folder;
               break;
         }

         return info;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="StorageManagerService"></param>
      /// <returns></returns>
      public async Task<IFileSystemHandle> GetRootElementAsync(
         IStorageManagerService? StorageManagerService)
      {
         OriginPrivateFileSystemHandle = null;
         if (StorageManagerService != null)
         {
            OriginPrivateFileSystemHandle = 
               await StorageManagerService.GetOriginPrivateDirectoryAsync();
            return OriginPrivateFileSystemHandle;
         }
         return OriginPrivateFileSystemHandle;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="StorageManagerService"></param>
      /// <returns></returns>
      public async Task BuildTreeAsync(
         IStorageManagerService? StorageManagerService)
      {
         var root = await GetRootElementAsync(StorageManagerService);
         Root = await GetItemAsync(root);
         await BuildTreeAsync(root, Root);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="handle"></param>
      /// <returns></returns>
      public async Task<FileSystemItemInfo> BuildTreeAsync(
         IFileSystemHandle handle, FileSystemItemInfo parent)
      {
         if (handle is FileSystemFileHandle)
         {
            return await GetItemAsync(handle);
         }
         else if (handle is FileSystemDirectoryHandle)
         {
            var directoryHandle = handle as FileSystemDirectoryHandle;
            if (directoryHandle != null)
            {
               var children = (await directoryHandle.ValuesAsync()).ToList();
               foreach (var child in children)
               {
                  var item = await GetItemAsync(child);
                  await BuildTreeAsync(child, item);
                  item.Parent = parent;
                  parent.Children.Add(item);
                  parent.InstanceChildren.Add(item.Handle);
               }
            }
         }
         return parent;
      }

   }

}

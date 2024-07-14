using Edam.DataObjects.Trees;
using KristofferStrube.Blazor.FileSystem;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Edam.Web.FileSystemHelper;

public class FileSystemDirectory
{

   #region -- 1.00 - Declare - define fields and properties

   public const string CATALOG_FILE_NAME = "catalog.json";
   private static IStorageManagerService? _StorageService;
   private static HashSet<FileSystemItemInfo?>? _Directory;

   private static Dictionary<string, TreeItem>? _Catalog;
   private static FileSystemItemInfo? _CatalogFileHandle = null;
   private static int _CatalogChangeCount = 0;

   public FileSystemTree? FileSystem
   {
      get { return _Tree; }
   }

   public HashSet<FileSystemItemInfo?>? Instance
   {
      get { return _Directory; }
   }

   /// <summary>
   /// Tree Items built initializing the component, just for reference.
   /// Note that the "root" item name is set to "Projects" as default.
   /// </summary>
   private static FileSystemTree? _Tree;

   public FileSystemItemInfo? RootItem { get; set; }

   #endregion
   #region -- 1.50 - Initialization

   /// <summary>
   /// Initialize Directory...
   /// </summary>
   /// <param name="service"></param>
   /// <returns>Task is returned</returns>
   public static async Task<FileSystemDirectory> InitializeDirectory(
      IStorageManagerService service)
   {
      FileSystemDirectory directory = new FileSystemDirectory();

      _StorageService = service;
      var tree = new FileSystemTree(service);
      await tree.BuildTreeAsync(service);
      _Tree = tree;

      directory.RootItem = tree.Root;
      directory.RootItem.Name = "Projects";
      directory.RootItem.Title = directory.RootItem.Name;

      _Directory = new HashSet<FileSystemItemInfo>();
      _Directory.Add(directory.RootItem);

      await directory.GetCatalog();

      return directory;
   }

   #endregion
   #region -- 4.00 - Catalog Management

   /// <summary>
   /// Read Catalog, if any is available...
   /// </summary>
   public async Task GetCatalog()
   {
      _Catalog = null;

      if (_Directory == null)
      {
         return;
      }
      foreach (var item in _Directory)
      {
         foreach (var child in item.Children)
         {
            if (child == null && child.Type == TreeItemType.Branch)
            {
               continue;
            }
            if (child.Name == CATALOG_FILE_NAME)
            {
               // Catalog was find, read it from file
               var jsonData = await ReadItemTextAsync(child);
               _Catalog = TreeItem.FromDirectoryJsonText<
                  Dictionary<string, TreeItem>>(jsonData);
               _CatalogFileHandle = child;

               // set directory entries Catalog-Entry for further reference
               _Catalog = GetCatalog(_Directory, _Catalog);
               break;
            }
         }
         break;
      }

      // if Catalog is null, set it base on the current directory and persist it
      if (_Catalog == null)
      {
         _Catalog = GetCatalog(_Directory, null);
         await SaveCatalogAsync();
      }
   }

   /// <summary>
   /// Get Tree Item (recursively)
   /// </summary>
   /// <param name="items">file system items</param>
   /// <param name="dictionary">catalog dictionary</param>
   /// <returns>Catalog dictionary</returns>
   private Dictionary<string, TreeItem> GetCatalog(
      HashSet<FileSystemItemInfo?> items, 
      Dictionary<string, TreeItem>? dictionary)
   {
      if (dictionary == null)
      {
         dictionary = new Dictionary<string, TreeItem>();
      }
      foreach(var item in items)
      {
         if (dictionary.TryGetValue(item.Name, out TreeItem treeItem))
         {
            treeItem.Visited = true;
            item.Title = treeItem.Title;
            item.CatalogEntry = treeItem;
         }
         else
         {
            treeItem = item.Duplicate();
            dictionary.Add(item.Name, treeItem);
            item.CatalogEntry = treeItem;
         }
         
         if (item.Type == TreeItemType.Branch)
         {
            GetCatalog(item.Children, dictionary);
         }
      }
      return dictionary;
   }

   /// <summary>
   /// Save Catalog...
   /// </summary>
   /// <returns></returns>
   private async Task SaveCatalogAsync()
   {
      if (_Catalog == null)
      {
         return;
      }

      // remove items that are not used
      foreach (var item in _Catalog)
      {
         if (!item.Value.Visited)
         {
            _Catalog.Remove(item.Value.Name);
         }
      }

      // get related JSON text
      string jsonText = TreeItem.ToDirectoryJsonText(_Catalog);
      if (_CatalogFileHandle == null)
      {
         // get root item
         var rootItem = _Directory.ElementAt(0);

         // create a new file and write item
         _CatalogFileHandle = await AddItemAsync(rootItem, CATALOG_FILE_NAME);
      }

      // finally safe data
      await WriteItemAsync(_CatalogFileHandle, jsonText);
   }

   /// <summary>
   /// Rename Catalog Item.
   /// </summary>
   /// <param name="item">tree item whose name will be updated</param>
   /// <param name="newName">new name</param>
   public async Task RenameCatalogItemAsync(
      FileSystemItemInfo item, string newName)
   {
      // if item is not found in the catalog add it...
      if (!_Catalog.TryGetValue(item.Name, out TreeItem treeItem))
      {
         treeItem = item.Duplicate();
         _Catalog.Add(item.Name, treeItem);
      }

      // assign new name
      treeItem.Title = newName;

      await SaveCatalogAsync();
   }

   #endregion
   #region -- 4.00 - File System Support

   /// <summary>
   /// Add a child directory in the current selected directory.
   /// </summary>
   /// <param name="item"></param>
   /// <returns>Task is returned</returns>
   public async Task AddBranchAsync(FileSystemItemInfo? item)
   {
      var directoryHandle = item == null ? null :
         item.Handle as FileSystemDirectoryHandle;
      if (directoryHandle == null)
      {
         return;
      }
      IFileSystemHandle itemHandle =
         await directoryHandle.GetDirectoryHandleAsync(
            $"{Guid.NewGuid().ToString()}", new()
            { Create = true }
         );
      var fitem = await FileSystem.GetItemAsync(itemHandle);
      item.InstanceChildren.Add(itemHandle);
      item.Children.Add(fitem);

      _Catalog.TryAdd(fitem.Name, fitem.Duplicate());
      _CatalogChangeCount++;

   }

   /// <summary>
   /// Add a child file in the current selected directory.
   /// </summary>
   /// <param name="item"></param>
   /// <param name="itemName">optional item title</param>
   /// <returns>Task is returned</returns>
   public async Task<FileSystemItemInfo?> AddItemAsync(
      FileSystemItemInfo? item, string itemName = null)
   {
      var directoryHandle = item == null ? null :
         item.Handle as FileSystemDirectoryHandle;

      if (directoryHandle == null)
      {
         return null;
      }

      var newId = $"{Guid.NewGuid().ToString()}";
      var name = itemName ?? newId;

      IFileSystemHandle fileHandle = null;
      try
      {
         fileHandle = await directoryHandle.GetFileHandleAsync(
            name, new() { Create = true }
         );
      }
      catch ( Exception ex)
      {

      }

      var fitem = await FileSystem.GetItemAsync(fileHandle);
      fitem.Title = name;

      item.InstanceChildren.Add(fileHandle);
      item.Children.Add(fitem);

      _Catalog.TryAdd(fitem.Name, fitem.Duplicate());
      _CatalogChangeCount++;

      return fitem;
   }

   /// <summary>
   /// Delete the selected Item (a Directory or File).
   /// </summary>
   /// <param name="branch"></param>
   /// <param name="item"></param>
   /// <returns>Task is returned</returns>
   public async Task DeleteItemAsync(
      FileSystemItemInfo? branch, FileSystemItemInfo? item)
   {
      var directoryHandle = branch.Handle as FileSystemDirectoryHandle;

      var ditem = await directoryHandle.ResolveAsync(item.Handle);
      if (ditem.Length == 0)
      {
         return;
      }

      await directoryHandle.RemoveEntryAsync(
         item.Name, new() { Recursive = true });

      branch.Children.Remove(item);
      branch.InstanceChildren.Remove(item.Handle);

      // remove item from catalog
      if (_Catalog.TryGetValue(item.Name, out TreeItem value))
      {
         _Catalog.Remove(item.Name);
         _CatalogChangeCount++;
      }
   }

   #endregion
   #region -- 4.00 - Read - Write File System Item Data

   /// <summary>
   /// Item changed, save item.
   /// </summary>
   /// <param name="item"></param>
   public async Task<string> ReadItemAsync(FileSystemItemInfo? item)
   {
      string? text = null;
      FileSystemFileHandle handle = item.Handle as FileSystemFileHandle;
      if (handle is not null)
      {
         var file = await handle.GetFileAsync();
         text = await file.TextAsync();
      }
      return text;
   }

   /// <summary>
   /// Item changed, save item.
   /// </summary>
   /// <param name="item"></param>
   public async Task<string> ReadItemTextAsync(FileSystemItemInfo? item)
   {
      string? text = null;
      FileSystemFileHandle handle = item.Handle as FileSystemFileHandle;
      if (handle is not null)
      {
         var file = await handle.GetFileAsync();
         text = await file.TextAsync();
      }
      return text;
   }

   /// <summary>
   /// Item changed, save item.
   /// </summary>
   /// <param name="item"></param>
   /// <param name="value">text v alue</param>
   public async Task WriteItemAsync(
      FileSystemItemInfo? item, string value)
   {
      FileSystemFileHandle handle = item.Handle as FileSystemFileHandle;
      if (handle is not null)
      {
         await using var writable = await handle.CreateWritableAsync();
         await writable.WriteAsync(value);
      }
   }

   #endregion

}

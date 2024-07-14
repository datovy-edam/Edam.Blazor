using Edam.DataObjects.Trees;
using KristofferStrube.Blazor.FileSystem;
using Newtonsoft.Json;
using MudBlazor;

namespace Edam.Web.FileSystemHelper;

/// <summary>
/// File System Item support / helper class.
/// </summary>
public class FileSystemItemInfo : TreeItem, IDisposable, ITreeItem
{

   public const string TYPE_DIRECTORY = "Directory";
   public const string TYPE_FILE = "File";

   public string TypeText
   {
      get { return Type.ToString(); }
      set
      {
         Type = value == TYPE_DIRECTORY ?
            TreeItemType.Branch : TreeItemType.Leaf;
      }
   }

   public new string Title
   {
      get { return CatalogEntry == null ? base.Title : CatalogEntry.Title; }
      set
      {
         if (CatalogEntry != null)
         {
            CatalogEntry.Title = value;
         }
         else
         {
            base.Title = value;
         }
      }
   }

   public bool HasChildren
   {
      get { return Children != null && Children.Count > 0; }
   }

   public HashSet<FileSystemItemInfo> Children { get; set; } =
      new HashSet<FileSystemItemInfo>();
   public FileSystemItemInfo Parent { get; set; }

   [JsonIgnore]
   public IFileSystemHandle? Handle { get; set; }

   [JsonIgnore]
   public List<IFileSystemHandle> InstanceChildren { get; set; } =
      new List<IFileSystemHandle>();

   public TreeItem? CatalogEntry { get; set; } = null;

   /// <summary>
   /// From a root element to 
   /// </summary>
   /// <param name="root"></param>
   /// <returns></returns>
   public String ToDirectoryJsonText(FileSystemItemInfo? root)
   {
      return TreeItem.ToDirectoryJsonText(root);
   }

   /// <summary>
   /// From a JSON file system info to 
   /// </summary>
   /// <param name="jsonText"></param>
   /// <returns></returns>
   public FileSystemItemInfo? FromDirectoryJsonText(string jsonText)
   {
      return TreeItem.FromDirectoryJsonText<FileSystemItemInfo?>(jsonText);
   }

   /// <summary>
   /// Get Text File Content.
   /// </summary>
   /// <returns>text as a string is returned.  
   /// If item handle is not a file it will retur null.</returns>
   public async Task<string?> GetFileTextAsync()
   {
      string? text = null;
      if (Handle != null && Type == TreeItemType.Leaf)
      {
         FileSystemFileHandle? fhandle = Handle as FileSystemFileHandle;
         if (fhandle != null)
         {
            var file = await fhandle.GetFileAsync();
            text = await file.TextAsync();
         }
      }
      return text;
   }

   /// <summary>
   /// Dispose of file handle resources async.
   /// </summary>
   /// <returns>instance to Task is returned</returns>
   public async Task DisposeAsync()
   {
      if (Handle != null)
      {
         switch (Type)
         {
            case TreeItemType.Branch:
               FileSystemDirectoryHandle? dhandle =
                  Handle as FileSystemDirectoryHandle;
               if (dhandle != null)
               {
                  await dhandle.DisposeAsync();
               }
               break;
            case TreeItemType.Leaf:
               FileSystemFileHandle? fhandle = Handle as FileSystemFileHandle;
               if (fhandle != null)
               {
                  await fhandle.DisposeAsync();
               }
               break;
         }
         Handle = null;
      }
   }

   /// <summary>
   /// Dispose of file handle resources.
   /// </summary>
   public async void Dispose()
   {
      await DisposeAsync();
   }

}

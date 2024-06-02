using KristofferStrube.Blazor.FileSystem;

namespace Edam.Web.FileSystemHelper;

/// <summary>
/// File System Item support / helper class.
/// </summary>
public class FileSystemItemInfo : IDisposable
{

   public const string KIND_DIRECTORY = "Directory";
   public const string KIND_FILE = "File";

   public FileSystemItemKind Kind { get; set; }
   public string KindText
   {
      get { return Kind.ToString(); }
      set
      {
         Kind = value == KIND_DIRECTORY ?
            FileSystemItemKind.Directory : FileSystemItemKind.File;
      }
   }

   public IFileSystemHandle? Handle { get; set; }
   public string? Name { get; set; }
   public List<FileSystemItemInfo> Children { get; set; } = 
      new List<FileSystemItemInfo>();

   /// <summary>
   /// Dispose of file handle resources async.
   /// </summary>
   /// <returns>instance to Task is returned</returns>
   public async Task DisposeAsync()
   {
      if (Handle != null)
      {
         switch (Kind)
         {
            case FileSystemItemKind.Directory:
               FileSystemDirectoryHandle? dhandle =
                  Handle as FileSystemDirectoryHandle;
               if (dhandle != null)
               {
                  await dhandle.DisposeAsync();
               }
               break;
            case FileSystemItemKind.File:
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
   /// Get Text File Content.
   /// </summary>
   /// <returns>text as a string is returned.  
   /// If item handle is not a file it will retur null.</returns>
   public async Task<string?> GetFileTextAsync()
   {
      string? text = null;
      if (Handle != null && Kind == FileSystemItemKind.File)
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
   /// Dispose of file handle resources.
   /// </summary>
   public async void Dispose()
   {
      await DisposeAsync();
   }

}

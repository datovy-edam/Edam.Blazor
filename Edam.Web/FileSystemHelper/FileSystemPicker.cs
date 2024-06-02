using KristofferStrube.Blazor.FileSystem;
using KristofferStrube.Blazor.FileSystemAccess;

using Edam.Diagnostics;
namespace Edam.Web.FileSystemHelper;

/// <summary>
/// Provide Access to the File System Pickers.
/// </summary>
/// <remarks>
/// In the Web Page, inject the FileSystemAccessService as follows:
/// 
///    @using KristofferStrube.Blazor.FileSystemAccess;
///    @inject IFileSystemAccessService FileSystemAccessService;
///    
/// </remarks>
public class FileSystemPicker
{

   /// <summary>
   /// Open File Picker and Read File.
   /// </summary>
   /// <param name="service">instance of FileSystem service reference</param>
   /// <example>
   /// private async Task OpenAndReadFile()
   /// {
   ///    FileSystemPicker pickers = new FileSystemPicker();
   ///    var results = await pickers.OpenAndReadFile(FileSystemAccessService);
   ///    StateHasChanged();
   /// }
   /// </example>
   /// <returns>instance of results log is returned</returns>
   public async Task<IResultsLog> OpenAndReadFile(
      IFileSystemAccessService service, bool dispose = false)
   {
      string? text = null;
      IResultsLog? resultsLog = null;
      FileSystemFileHandle? fileHandle = null;
      try
      {
         OpenFilePickerOptionsStartInWellKnownDirectory options = new()
         {
            Multiple = false,
            StartIn = WellKnownDirectory.Downloads
         };
         var fileHandles = await service.ShowOpenFilePickerAsync(options);
         fileHandle = fileHandles.Single();
      }
      catch (Exception ex)
      {
         // Handle Exception or cancelation of File Access prompt
         //Console.WriteLine(ex);
         resultsLog = new ResultLog();
         resultsLog.Failed(ex);
      }
      finally
      {
         if (fileHandle is not null)
         {
            var file = await fileHandle.GetFileAsync();
            text = await file.TextAsync();
            if (dispose)
            {
               await fileHandle.DisposeAsync();
            }
         }
         if (resultsLog is null)
         {
            var results = new ResultLog();
            results.ResultValueObject = text;
            results.Succeeded();
            resultsLog = results;
         }
      }

      return resultsLog;
   }

   /// <summary>
   /// Get Item Async...
   /// </summary>
   /// <param name="handle">file element handle</param>
   /// <returns>return the info of the specific item</returns>
   public async Task<FileSystemItemInfo> GetItemAsync(IFileSystemHandle handle)
   {
      FileSystemItemInfo i = new FileSystemItemInfo();
      i.KindText = (await handle.GetKindAsync()).ToString();
      i.Name = await handle.GetNameAsync();
      i.Handle = handle;
      return i;
   }

   /// <summary>
   /// Get Folder and child Folders hierarchy of children.
   /// </summary>
   /// <param name="handle">folder element handle</param>
   /// <param name="items">items contain in the folder element</param>
   /// <returns>returns the child elements of the given folder and children if
   /// any</returns>
   public async Task<List<FileSystemItemInfo>> GetFolderItemsAsync(
      FileSystemDirectoryHandle handle, List<FileSystemItemInfo> items)
   {
      var ditm = await GetItemAsync(handle);
      items.Add(ditm);
      var children = (await handle.ValuesAsync()).ToList();
      foreach (var item in children)
      {
         var fitem = await GetItemAsync(item);
         if (fitem.Kind == FileSystemItemKind.Directory)
         {
            await GetFolderItemsAsync(
               (FileSystemDirectoryHandle)item, fitem.Children);
         }
         items.Add(fitem);
      }
      return items;
   }

   /// <summary>
   /// Open File Picker and Read File.
   /// </summary>
   /// <param name="service">instance of FileSystem service reference</param>
   /// <example>
   /// private async Task OpenAndReadFile()
   /// {
   ///    FileSystemPicker pickers = new FileSystemPicker();
   ///    var results = await pickers.OpenAndReadFile(FileSystemAccessService);
   ///    StateHasChanged();
   /// }
   /// </example>
   /// <returns>instance of results log is returned</returns>
   public async Task<IResultsLog> OpenAndReadFolder(
      IFileSystemAccessService service, bool dispose = false)
   {
      IResultsLog? resultsLog = null;
      FileSystemDirectoryHandle? dirHandle = null;
      try
      {
         DirectoryPickerOptionsStartInWellKnownDirectory options = new()
         {
            StartIn = WellKnownDirectory.Documents
         };
         dirHandle = await service.ShowDirectoryPickerAsync(options);
      }
      catch (Exception ex)
      {
         // Handle Exception or cancelation of File Access prompt
         //Console.WriteLine(ex);
         resultsLog = new ResultLog();
         resultsLog.Failed(ex);
      }
      finally
      {
         List<FileSystemItemInfo> litems = new List<FileSystemItemInfo>();
         if (dirHandle is not null)
         {
            litems = await GetFolderItemsAsync(dirHandle, litems);

            if (dispose)
            {
               await dirHandle.DisposeAsync();
            }
         }
         if (resultsLog is null)
         {
            var results = new ResultLog();
            results.ResultValueObject = litems;
            results.Succeeded();
            resultsLog = results;
         }
      }

      return resultsLog;
   }

}

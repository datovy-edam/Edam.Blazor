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
      IFileSystemAccessService service, bool dispose = true)
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
      IFileSystemAccessService service, bool dispose = true)
   {
      string? text = null;
      IResultsLog? resultsLog = null;
      FileSystemDirectoryHandle? dirHandle = null;
      try
      {
         DirectoryPickerOptionsStartInWellKnownDirectory options = new()
         {
            StartIn = WellKnownDirectory.Downloads
         };
         var dirHandles = await service.ShowDirectoryPickerAsync(options);

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
         if (dirHandle is not null)
         {
            var dir = await dirHandle.ValuesAsync();
            //text = await file.TextAsync();
            if (dispose)
            {
               await dirHandle.DisposeAsync();
            }
         }
         if (resultsLog is null)
         {
            var results = new ResultLog();
            results.ResultValueObject = null;
            results.Succeeded();
            resultsLog = results;
         }
      }

      return resultsLog;
   }

}

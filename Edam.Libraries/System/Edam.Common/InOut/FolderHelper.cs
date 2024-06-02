using System;
using System.Collections.Generic;
using io = System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;
using Edam.Text;

namespace Edam.InOut
{

   public class FolderHelper
   {

      public static IResultsLog FileWordToUnderscoredWord(
         string inFilePath, string outFilePath)
      {
         ResultLog result = new ResultLog();
         try
         {
            TextFile f = new TextFile();
            f.FromFile(inFilePath);
            TextFile ucFile = f.MixCaseToUnderscoredWords(
               TextTokenOption.UpperCase);
            ucFile.ToFile(outFilePath);
            result.Succeeded();
         }
         catch (Exception ex)
         {
            result.Failed(ex);
         }
         return result;
      }

      /// <summary>
      /// Folder Copy.
      /// </summary>
      /// <param name="sourceName">source folder</param>
      /// <param name="destinationName">destination folder</param>
      /// <param name="copySubFolders">true to copy sub-folders</param>
      /// <exception cref="io.DirectoryNotFoundException"></exception>
      public static void FolderCopy(
         string sourceName, string destinationName, bool copySubFolders)
      {
         io.DirectoryInfo dir = new io.DirectoryInfo(sourceName);
         io.DirectoryInfo[] dirs = dir.GetDirectories();

         // If the source directory does not exist, throw an exception.
         if (!dir.Exists)
         {
            throw new io.DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceName);
         }

         // If the destination directory does not exist, create it.
         if (!io.Directory.Exists(destinationName))
         {
            io.Directory.CreateDirectory(destinationName);
         }

         // Get the file contents of the directory to copy.
         io.FileInfo[] files = dir.GetFiles();

         foreach (io.FileInfo file in files)
         {
            // Create the path to the new copy of the file.
            string temppath = io.Path.Combine(destinationName, file.Name);

            // Copy the file.
            file.CopyTo(temppath, false);
         }

         // If copySubFolders is true, copy the subdirectories.
         if (copySubFolders)
         {
            foreach (io.DirectoryInfo subdir in dirs)
            {
               // Create the subdirectory.
               string temppath = io.Path.Combine(destinationName, subdir.Name);

               // Copy the subdirectories.
               FolderCopy(subdir.FullName, temppath, copySubFolders);
            }
         }
      }

   }

}

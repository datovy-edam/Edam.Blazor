using Edam.Data.AssetConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Lexicon
{

   public class LexiconFileInfo
   {

      public const string LEXICON_FOLDER = "Lexicon";

      /// <summary>
      /// Get default Lexicon folder relative path.
      /// </summary>
      /// <returns></returns>
      public static string GetDefaultRelativeFolderPath()
      {
         return "./" + LEXICON_FOLDER;
      }

      /// <summary>
      /// Verify that Lexicon Path exists, if not create it.
      /// </summary>
      /// <param name="path">path to verify</param>
      public static void VerifyLexiconPath(string path)
      {
         // check if folder exists, if not create it
         if (!Directory.Exists(path))
         {
            Directory.CreateDirectory(path);
         }
      }

      /// <summary>
      /// Set Extension with given info.
      /// </summary>
      /// <remarks>
      /// If extension is null or empty it will be defaulted to OPEN_XML.
      /// </remarks>
      /// <param name="fileInfo">file info whose extension should be changed
      /// </param>
      /// <param name="extension">extension to change to</param>
      public static void SetExtension(
         InOut.FileInfo fileInfo, string extension)
      {
         if (String.IsNullOrWhiteSpace(extension))
         {
            extension = InOut.FileExtension.OPEN_XML;
         }

         if (!String.IsNullOrWhiteSpace(fileInfo.Extension))
         {
            fileInfo.Full.Replace(fileInfo.Extension, extension);
         }
         fileInfo.Extension = extension;
      }

      /// <summary>
      /// Get Lexicon File Information based on current project path.  By
      /// default the extension is set to OPEN_XML Excel Workbook extension.
      /// </summary>
      /// <remarks>
      /// The path is assumed to be relative to the current location.  If the
      /// expected folder don't exist it will be created.
      /// </remarks>
      /// <param name="arguments">request arguments</param>
      /// <param name="extension">
      /// (optional) file extension [default: OPEN_XML]</param>
      /// <returns>returns the prepared FileInfo</returns>
      public static InOut.FileInfo GetFileInfo(
         AssetConsoleArgumentsInfo arguments, 
         string extension = InOut.FileExtension.OPEN_XML)
      {
         InOut.FileInfo exportFile = new InOut.FileInfo();

         exportFile.Path = GetDefaultRelativeFolderPath();
         exportFile.Name =
            (arguments.Namespace.OrganizationDomainId + ".lexicon").ToLower();
         exportFile.Full = exportFile.Path + "/" + exportFile.Name + "." +
            extension;
         SetExtension(exportFile, extension);

         // check if folder exists, if not create it
         VerifyLexiconPath(exportFile.Path);

         return exportFile;
      }

   }

}

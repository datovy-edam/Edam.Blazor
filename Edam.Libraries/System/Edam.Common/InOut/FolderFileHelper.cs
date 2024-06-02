using System;

// -----------------------------------------------------------------------------

namespace Edam.InOut
{

   public class FolderFileHelper
   {

      public FolderFileHelper()
      {
      }

      public static String GetFullPath(
         Environment.SpecialFolder folder, String fileName = null)
      {
         var root = Environment.GetFolderPath(folder);
         String fldr;
         switch(folder)
         {
            case Environment.SpecialFolder.MyDocuments:
               fldr = "/Documents";
               break;
            default:
               fldr = null;
               break;
         }
         return root + (fldr ?? String.Empty) + '/'
            + (fileName ?? String.Empty);
      }

      public static void MoveToFolder(string folderPath)
      {
         if (System.IO.Directory.Exists(folderPath))
         {
            System.IO.Directory.SetCurrentDirectory(folderPath);
         }
      }

   }

}

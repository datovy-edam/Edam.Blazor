using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Application;

namespace Edam.InOut
{

   public class FileHelper
   {

      /// <summary>
      /// Given a file name, resolve the full file path using the application
      /// working directory...
      /// </summary>
      /// <param name="filename">file name</param>
      /// <returns>returns the full-file path for the application file working
      /// folder</returns>
      /// <remarks></remarks>
      public static string GetWorkingDirectoryFullPath(string filename)
      {
         var fhelper = DependencyService.Get<IFileHelper>();
         if (fhelper == null)
            return filename;
         return fhelper.GetLocalFilePath(filename);
      }

   }

}

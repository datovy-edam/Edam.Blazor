using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Xml.Linq;

// -----------------------------------------------------------------------------
using Edam.Data.AssetConsole;
using Edam.InOut;
using Edam.Xml.OpenXml;
using Edam.Data.Asset;
using Edam.DataObjects.Assets;

namespace Edam.Data.AssetDb.Readers
{

   public class FileReader
   {

      public static object ReadCodeSet(AssetConsoleArgumentsInfo arguments)
      {
         var uriList = UriResourceInfo.GetUriList(
           arguments.UriList, UriResourceType.xlsx);
         foreach (var fname in uriList)
         {
            var results = ExcelDocumentReader.ReadDocument(
               fname, CodeSet.TAB_CODE_SET);
            if (results.Success)
            {
               var result = CodeSet.ToCodeSet(results.Data);
               if (result.Success)
               {
                  CodeSetService.Save(result.Data);
               }
            }
         }
         return null;
      }

      public static object Reader(AssetConsoleArgumentsInfo arguments)
      {
         if (arguments.InFileExtension == FileExtension.OPEN_XML &&
            arguments.Procedure == AssetConsoleProcedure.FileToCodeSet)
         {
            return ReadCodeSet(arguments);
         }
         return null;
      }

   }

}

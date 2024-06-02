using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;

namespace Edam.Xml.OpenXml
{

   public class ExcelDocumentReader
   {

      /// <summary>
      /// Is Empty List...
      /// </summary>
      /// <param name="list">list of strings to evaluate</param>
      /// <returns>true is returned if it is an empty list</returns>
      public static bool IsEmptyList(List<string> list)
      {
         int cnt = 0;
         foreach (var item in list)
         {
            if (String.IsNullOrWhiteSpace(item))
            {
               cnt++;
            }
         }
         return cnt == list.Count;
      }

      /// <summary>
      /// Read document if the worksheet to be found by name is found, else
      /// failure will be returned if there was an exception or the worksheet
      /// is not found (EventCode == ReferenceNotFound).
      /// </summary>
      /// <param name="fileName"></param>
      /// <param name="worksheetName"></param>
      /// <returns>the list of rows are returned if worksheet is found, else
      /// failure with an EventCode or an exception may be returned</returns>
      public static ResultsLog<List<List<string>>> ReadDocument(
         string fileName, string worksheetName)
      {
         ResultsLog<List<List<string>>> results = 
            new ResultsLog<List<List<string>>>();
         ExcelDocument d = new ExcelDocument();
         try
         {
            d.OpenDocument(fileName);
            var r = d.GetWorksheetReader(worksheetName);
            if (r != null)
            {
               results.Data = d.ReadWorksheet(r, d.GetCurrentWorksheet());
               results.Succeeded();
            }
            else
            {
               results.Failed(EventCode.ReferenceNotFound);
            }
         }
         catch(Exception ex)
         {
            results.Failed(ex);
         }
         finally
         {
            if (d != null)
            {
               d.Dispose();
            }
         }
         return results;
      }

   }

}

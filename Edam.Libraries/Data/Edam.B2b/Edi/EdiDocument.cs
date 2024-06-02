using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Edam.Data.AssetConsole;
using Edam.Data.AssetSchema;
using Edam.Diagnostics;

namespace Edam.B2b.Edi
{

   public class EdiDocument
   {

      public EdiSegmentList Items { get; set; }

      /// <summary>
      /// Prepare a JSON (text) document of this.
      /// </summary>
      /// <returns>JSON text is returned</returns>
      public string ToJsonText()
      {
         return JsonConvert.SerializeObject(this, Formatting.Indented);
      }

      /// <summary>
      /// Write Document as JSON text to given path.
      /// </summary>
      /// <param name="filePath">URI or file path</param>
      public void ToFile(string filePath)
      {
         var jsonText = ToJsonText();
         System.IO.File.WriteAllText(filePath, jsonText);
      }

      /// <summary>
      /// Read Document from file using given path.
      /// </summary>
      /// <param name="filePath">file path</param>
      /// <returns>instance of Edi Document is returned</returns>
      public static ResultsLog<EdiDocument?> FromFile(string filePath)
      {
         System.IO.File.ReadAllText(filePath);
         ResultsLog<EdiDocument?> results = new ResultsLog<EdiDocument?>();
         try
         {
            results.Data = JsonConvert.DeserializeObject<EdiDocument>(filePath);
            results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         return results;
      }

   }

}

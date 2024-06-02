using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Jsonata.Net.Native.Json;

using Edam.Text;
using Edam.Diagnostics;
using Newtonsoft.Json;

namespace Edam.Json.JsonHelper
{

   public class JsonParserResults : IParserResults
   {
      [JsonIgnore]
      public object ParentContext { get; set; } = null;

      public object MapItem { get; set; } = null;
      public object Context { get; set; } = null;
      public string OriginalText { get; set; } = null;
      public string ParsedText { get; set; } = null;
      public string ResultText { get; set; } = null;
      public List<string> Extracts { get; set; } = new List<string>();

      [JsonIgnore]
      public ResultLog Results { get; set; } = new ResultLog();

      /// <summary>
      /// Write list of results to a file.
      /// </summary>
      /// <param name="filePath">file path</param>
      /// <param name="items">items list to write</param>
      public static void ToFile(string filePath, List<IParserResults> items)
      {
         string jsonText = Serialization.JsonSerializer.
            Serialize<List<IParserResults>>(items);
         File.WriteAllText(filePath, jsonText);
      }

      /// <summary>
      /// Read list of results from a file.
      /// </summary>
      /// <param name="filePath">file path</param>
      /// <returns>returns the listg of results from the file</returns>
      public static List<IParserResults> FromFile(string filePath)
      {
         string jsonText = File.ReadAllText(filePath);
         var results = Serialization.JsonSerializer.
            Deserialize<List<JsonParserResults>>(jsonText);
         List<IParserResults> items = new List<IParserResults>();
         foreach (var item in results)
         {
            items.Add(item);
         }
         return items;
      }
   }

}

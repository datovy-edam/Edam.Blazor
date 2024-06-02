using Edam.Diagnostics;
using Edam.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Json.JsonQuery
{

   public class JsonProcessorResults
   {
      public List<ResultsLog<string>> ResultsLog { get; set; } =
         new List<ResultsLog<string>>();

      public List<string> Definitions { get; set; }
      public List<IParserResults> Results { get; set; } = 
         new List<IParserResults>();

      /// <summary>
      /// Clear all previous results.
      /// </summary>
      public void Clear()
      {
         ResultsLog.Clear();
         Results.Clear();
      }

      /// <summary>
      /// Scan for definitions, errors and other.
      /// </summary>
      /// <param name="jsonText">input JSON</param>
      public void ProcessJson(string jsonText)
      {

      }

      /// <summary>
      /// Add a new Result log...
      /// </summary>
      /// <param name="log">log info to add</param>
      public void Add(ResultsLog<string> log)
      {
         if (!log.Success)
         {
            ResultsLog.Add(log);
         }
         else
         {
            var jsonResult = log.DataObject.ToString();
            ProcessJson(jsonResult);
            //Results.Add(jsonResult);
         }
      }
   }

}

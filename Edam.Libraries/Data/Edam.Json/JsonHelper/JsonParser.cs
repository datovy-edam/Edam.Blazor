using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Edam.Json.JsonHelper
{

   public class JsonParser
   {

      /// <summary>
      /// Remove Properties by their names.
      /// </summary>
      /// <param name="token"></param>
      /// <param name="fields">properties field names</param>
      /// <param name="results">parser results (optional)</param>
      /// <returns>JToken is returned</returns>
      public static JsonParserResults RemoveProperties(
         JToken token, string[] fields, JsonParserResults results = null)
      {
         JContainer container = token as JContainer;
         if (container == null)
         {
            return results;
         }

         List<JToken> removeList = new List<JToken>();
         foreach (JToken el in container.Children())
         {
            JProperty p = el as JProperty;
            if (p != null && fields.Contains(p.Name))
            {
               removeList.Add(el);
            }
            RemoveProperties(el, fields, results);
         }

         foreach (JToken el in removeList)
         {
            if (results != null)
            {
               results.Extracts.Add(el.ToString());
            }
            el.Remove();
         }

         return results;
      }

      /// <summary>
      /// Scan JSON text, extract given tag and return results.
      /// </summary>
      /// <param name="jsonText">JSON text to scan</param>
      /// <param name="tag">tag to scan and extract from given JSON text</param>
      /// <returns>instance of JsonParseResults is returned with extracted list
      /// of items</returns>
      public static JsonParserResults ScanProperties(
         string jsonText, string tag)
      {
         string[] fields = { tag };

         JsonParserResults results = new JsonParserResults();
         results.OriginalText = jsonText;

         object? token = null;
         try
         {
            token = JsonConvert.DeserializeObject(jsonText);
            RemoveProperties(token as JToken, fields, results);
            results.ParsedText = 
               token == null ? String.Empty : token.ToString();
            results.Results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Results.Failed(ex);
         }

         return results;
      }

   }

}

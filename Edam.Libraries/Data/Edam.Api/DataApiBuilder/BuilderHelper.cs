
using Newtonsoft.Json;

namespace Edam.Api.DataApiBuilder
{

   public class BuilderHelper
   {

      /// <summary>
      /// Serialize given item into a JSON string.
      /// </summary>
      /// <param name="item">object to serialize</param>
      /// <returns>JSON string is returned</returns>
      public static string ToJson(object item)
      {
         return JsonConvert.SerializeObject(
            item, Newtonsoft.Json.Formatting.None,
            new JsonSerializerSettings
            {
               NullValueHandling = NullValueHandling.Ignore
            });
      }

   }

}

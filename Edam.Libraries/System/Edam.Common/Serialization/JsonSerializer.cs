using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using json = System.Text.Json;
using newton = Newtonsoft.Json;
using lnewton = Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

// -----------------------------------------------------------------------------
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Edam.Diagnostics;
//using Windows.Data.Json;

namespace Edam.Serialization
{

   public class JsonSerializer
   {
      public static readonly string DATE_TIME_UTC_FORMAT = 
         "yyyy-MM-dd'T'HH:mm:ssK";

      public static string Serialize<T>(T obj)
      {
         newton.JsonSerializerSettings serializerSettings =
            new newton.JsonSerializerSettings();
         serializerSettings.ReferenceLoopHandling =
            newton.ReferenceLoopHandling.Ignore;
         //return newton.JsonConvert.SerializeObject(obj, serializerSettings);
         return newton.JsonConvert.SerializeObject(
            obj, newton.Formatting.Indented, serializerSettings);
      }

      public static T Deserialize<T>(string jsonText)
      {
         if (string.IsNullOrEmpty(jsonText))
         {
            return default(T);
         }
         return newton.JsonConvert.DeserializeObject<T>(jsonText);
      }

      public static ResultsLog<T> TryDeserialize<T>(string jsonText)
      {
         ResultsLog<T> results = new ResultsLog<T>();
         if (string.IsNullOrEmpty(jsonText))
         {
            results.Data = default(T);
            results.Failed("JSON is null or empty");
            return results;
         }

         try
         {
            results.Data = newton.JsonConvert.DeserializeObject<T>(jsonText);
            results.Succeeded();
         }
         catch(Exception ex)
         {
            results.Data = default(T);
            results.Failed(ex);
         }
         return results;
      }

      public static dynamic ToDynamic(string jsonText)
      {
         if (string.IsNullOrEmpty(jsonText))
         {
            return null;
         }
         dynamic result = lnewton.JObject.Parse(jsonText);
         return result;
      }

      /* System.Text.Json 
      public static string Serialize<T>(T obj)
      {
         return json.JsonSerializer.Serialize<T>(obj);
      }

      public static T Deserialize<T>(string jsonText)
      {
         if (string.IsNullOrEmpty(jsonText))
         {
            return default(T);
         }
         return json.JsonSerializer.Deserialize<T>(jsonText);
      }

      /*
      public static string Serialize<T>(T obj)
      {
         DataContractJsonSerializer serializer =
            new DataContractJsonSerializer(obj.GetType(),
            new DataContractJsonSerializerSettings
            {
               DateTimeFormat = new DateTimeFormat(DATE_TIME_UTC_FORMAT)
            });
         MemoryStream ms = new MemoryStream();
         serializer.WriteObject(ms, obj);
         string retVal = Encoding.UTF8.GetString(
            ms.ToArray(), 0, (int)ms.Length);
         return retVal;
      }

      public static T Deserialize<T>(string json)
      {
         if (string.IsNullOrEmpty(json))
         {
            return default(T);
         }
         T obj = Activator.CreateInstance<T>();
         MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
         DataContractJsonSerializer serializer = 
            new DataContractJsonSerializer(obj.GetType(),
            new DataContractJsonSerializerSettings
            { 
               DateTimeFormat = new DateTimeFormat(DATE_TIME_UTC_FORMAT)
            });
         obj = (T)serializer.ReadObject(ms);
         //ms.Close();
         return obj;
      }
       */

   }

}

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;
using Edam.Data.AssetConsole;

namespace Edam.Data.AssetConsole
{

   public class AssetConsoleSubmission
   {

      public DateTime CreatedDate { get; set; }
      public AssetConsoleArgumentsInfo Arguments { get; set; }
      public MessageLogInfo Results { get; set; }
      public String SessionId { get; set; }

      public AssetConsoleSubmission()
      {
         CreatedDate = DateTime.UtcNow;
         Arguments = new AssetConsoleArgumentsInfo();
         Results = new MessageLogInfo();
      }

      #region -- To/From JSON Support

      /// <summary>
      /// 
      /// </summary>
      /// <param name="jsonDocument"></param>
      /// <returns></returns>
      public static AssetConsoleSubmission FromJson(string jsonDocument)
      {
         var args =
            JsonSerializer.Deserialize<AssetConsoleSubmission>(jsonDocument);
         return args;
      }

      public static String ToJson(AssetConsoleSubmission submission)
      {
         var args =
            JsonSerializer.Serialize<AssetConsoleSubmission>(submission);
         return args;
      }

      #endregion

   }

}

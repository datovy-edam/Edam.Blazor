using Edam.Application;
using Edam.Data.AssetMapping;
using Edam.Data.AssetSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.AssetMapping
{

   public class MapLanguageHelper
   {

      // TODO: provide setting the default language in appsettings...
      public const string DEFAULT_LANGUAGE = "JSONATA";

      /// <summary>
      /// Get Instance of the map language.
      /// </summary>
      /// <param name="resourceName">resource name, if null the default 
      /// language is returned</param>
      /// <returns>map language interface is returned</returns>
      public static IMapLanguageInfo GetInstance(string? resourceName = null)
      {
         string name = String.IsNullOrWhiteSpace(resourceName) ?
            AssetResourceHelper.ASSET_MAPPING_LANGUAGE : resourceName;
         return AppAssembly.FetchInstance<IMapLanguageInfo>(name);
      }

   }

}

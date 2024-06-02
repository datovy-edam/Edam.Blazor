using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------
using Newtonsoft.Json.Linq;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;

namespace Edam.Json.JsonSchema
{

   public class JsonComplexType : JsonItemInfo
   {

      public JsonComplexType(JToken token, NamespaceList namepaces) :
         base(token, namepaces)
      {

      }

   }

}

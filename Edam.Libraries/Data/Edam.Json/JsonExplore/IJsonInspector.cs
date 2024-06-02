using Edam.Data.Asset;
using Edam.Data.AssetConsole;
using Edam.Data.AssetSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Json.JsonExplore
{

   public interface IJsonInspector
   {
      NamespaceInfo DefaultNamespace { get; set; }
      AssetConsoleArgumentsInfo Arguments { get; set; }
      AssetData Asset { get; set; }

      void Inspect();
   }

}

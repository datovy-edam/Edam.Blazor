using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetSchema;

namespace Edam.Json.LinkData
{

   public class LinkDataItemInfo
   {
      public NamespaceInfo Namespace { get; set; }
      public string EntityName { get; set; }
      public string ItemName { get; set; }
      public string LinkName { get; set; }
      public AssetDataElement ElementType { get; set; }
      public AssetDataElement Asset { get; set; }
   }

}

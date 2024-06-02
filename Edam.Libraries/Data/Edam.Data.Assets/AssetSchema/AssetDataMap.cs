using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.AssetSchema
{

   public class AssetDataMapEntry
   {
      public AssetDataMapItem Source { get; set; }
      public AssetDataMapItem Target { get; set; }
   }

   public class AssetDataMap
   {
      public string Name { get; set; }
      public string UseCaseName { get; set; }
      public MapAnnotationInfo Annotation { get; set; }
      public List<AssetDataMapEntry> Items { get; set; }
   }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using Edam.Data.AssetSchema;

namespace Edam.Data.AssetConsole
{

   public class SampleItem
   {
      public string FilePath { get; set; }
      public string DocumentText { get; set; }
      public object Data { get; set; }
   }

   public class DataMapItemInfo
   {
      public MapItemType Type { get; set; }
      public string Name { get; set; }

      [IgnoreDataMember]
      public object Data { get; set; } = null;

      public SampleItem Sample { get; set; }

      public string ParentProcessName { get; set; }
      public ProcessInfo Process { get; set; } = null;
   }

}

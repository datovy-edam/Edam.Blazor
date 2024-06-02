using Edam.Data.AssetSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Assets.AssetUseCases
{

   /// <summary>
   /// A report item identify a report line source and target elements and its 
   /// collective instructions based on a MapItem.
   /// </summary>
   public class AssetUseCaseReportItem
   {
      public int SequenceNo { get; set; }
      public int Index { get; set; }
      public AssetDataMapItem MapItem { get; set; }
      public AssetDataElement Source { get; set; }
      public AssetDataElement Target { get; set; }
   }

}

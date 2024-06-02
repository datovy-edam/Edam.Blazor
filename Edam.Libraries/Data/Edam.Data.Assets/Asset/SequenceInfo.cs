using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.Data.Assets.Asset
{

   public class SequenceInfo
   {
      public SequenceType Type { get; set; }
      public int StartNumber { get; set; }
      public int EndNumber { get; set; }
      public int IncrementNumber { get; set; }
   }

}

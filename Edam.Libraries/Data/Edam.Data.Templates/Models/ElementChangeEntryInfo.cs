using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.Models
{

   public class ElementChangeEntryInfo
   {
      public object OriginalValue { get; set; }
      public string NewValue { get; set; }
      public DateTime UpdateDate { get; set; }
   }

}

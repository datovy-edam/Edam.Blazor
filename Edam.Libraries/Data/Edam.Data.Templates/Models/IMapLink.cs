using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Models
{

   public interface IMapLink
   {
      Int64 ParentNodeNo { get; set; }
      String ParentElementName { get; set; }
      String ChildElementName { get; set; }
      Object Value { get; set; }
      Object DefaultValue { get; set; }
   }

}

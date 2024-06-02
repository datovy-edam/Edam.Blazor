using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Models
{

   public class MapLinkInfo : IMapLink
   {
      public long ParentNodeNo { get; set; }
      public string ParentElementName { get; set; }
      public string ChildElementName { get; set; }
      public object Value { get; set; }
      public object DefaultValue { get; set; }
   }

}

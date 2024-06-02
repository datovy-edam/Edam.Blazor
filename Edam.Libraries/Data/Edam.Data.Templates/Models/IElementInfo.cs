using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Models
{
   
   public interface IElementInfo
   {
      String Title { get; set; }
      String Description { get; set; }
      String Name { get; set; }
      ResourceType Type { get; set; }
      Boolean Validate();
   }

}

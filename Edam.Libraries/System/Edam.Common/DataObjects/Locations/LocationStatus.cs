using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Locations
{

   public enum LocationStatus
   {
      Unknown = -1,
      Invalid = 0,
      Valid = 1,
      Editing = 100,
      Pending = 300,
      Deleted = 999
   }

}

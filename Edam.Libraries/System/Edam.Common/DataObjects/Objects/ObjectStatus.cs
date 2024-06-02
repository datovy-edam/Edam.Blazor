using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Objects
{

   public enum ObjectStatus
   {
      Unknown = 0,
      Active = 1,
      Inactive = 2,
      Editing = 100,
      Accepted = 101,
      Suspended = 102,
      Canceled = 103,
      Closed = 104,
      Sealed = 105,
      Deleted = 999
   }

}


using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Requests
{

   public enum RequestStatus
   {
      Unknown = 0,
      Requested = 1,
      Pending = 2,
      InProgress = 3,
      Canceled = 4,
      Completed = 5,
      Retired = 6,
      Failed = 7,
      Validated = 8
   }

}

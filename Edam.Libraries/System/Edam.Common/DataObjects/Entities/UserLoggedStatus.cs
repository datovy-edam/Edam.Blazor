using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Entities
{

   public enum UserLoggedStatus
   {
      Unknown = 0,
      Active = 1,
      ActiveWithMessages = 2,
      Inactive = 3,
      InactiveWithMessages = 4,
      RequiresReset = 5,
      RequiresRevision = 6
   }

}


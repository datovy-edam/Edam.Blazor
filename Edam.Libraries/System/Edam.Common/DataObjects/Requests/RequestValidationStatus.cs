using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Requests
{

   public enum RequestValidationStatus
   {
      Unknown = 0,
      PendingValidation = 1,
      InvalidEmail = 2,
      ValidEmail = 3,
      EmailAlreadyInUse = 4,
      PasswordChangeRequested = 5,
      PasswordChangeDone = 6
   }

}

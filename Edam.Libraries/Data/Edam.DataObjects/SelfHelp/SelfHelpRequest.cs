using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.SelfHelp
{

   public enum SelfHelpRequest
   {
      Unknown = 0,
      NewRegistration = 1,
      PasswordReset = 2,
      Deactivate = 3,
      RequestAssistance = 4,
      UserLogin = 5,
      UserLogout = 6,
      UpdateRequest = 7,
      ForgotEmail = 8,
      EmailValidationRequest = 9,
      SessionActiveValidation = -997
   }

}

using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
// Copied from Kif v5r0

namespace Edam.Services
{

   public interface IServiceInfo
   {

      String Key { get; set; }
      String Alias { get; set; }
      String ServicePathUri { get; set; }
      String AccountId { get; set; }
      String AccountUserId { get; set; }
      Edam.Security.Password AccountPassword { get; set; }
      String AccountDomain { get; set; }
      Boolean IsValid { get; }
      String Version { get; set; }
      Boolean IsActive { get; set; }
      void ClearFields();

   }

}

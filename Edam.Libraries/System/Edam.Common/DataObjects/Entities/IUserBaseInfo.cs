using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Entities
{

   public interface IUserBaseInfo
   {
      String UserId { get; set; }
      PersonName Name { get; set; }
      String OrganizationId { get; set; }
      String Email { get; set; }
      PhoneInfo Phone { get; set; }
   }

}

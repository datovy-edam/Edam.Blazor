using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

using Edam.Data;

namespace Edam.DataObjects.SelfHelp
{

   public class RegistrationEmailVerifiedInfo
   {
      public String RequestId { get; set; }
      public String UserId { get; set; }
      public String OrganizationId { get; set; }
      public String Email { get; set; }
      public String PhoneNumber { get; set; }
      public Boolean Verified { get; set; }
      public SelfHelpRequest Request { get; set; }

      public RegistrationEmailVerifiedInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         RequestId = String.Empty;
         UserId = String.Empty;
         OrganizationId = String.Empty;
         Email = String.Empty;
         PhoneNumber = String.Empty;
         Verified = false;
         Request = SelfHelpRequest.Unknown;
      }

      /*
      public void ReadData(System.Data.Common.DbDataReader reader)
      {
         RequestId = DataField.GetString(reader[0]);
         UserId = DataField.GetString(reader[1]);
         OrganizationId = DataField.GetString(reader[2]);
         Email = DataField.GetString(reader[3]);
         PhoneNumber = DataField.GetString(reader[4]);
         Verified = DataField.GetBool(reader[5]);
      }
       */
   }

}

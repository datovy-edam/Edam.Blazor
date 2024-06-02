using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

using Edam.Data;
using Edam.DataObjects.Entities;
using Edam.DataObjects.Requests;

namespace Edam.DataObjects.SelfHelp
{

   /// <summary>
   /// Registration Request Info.  This information is needed to post a request
   /// using the "Help.HelpRegisterRequestInsertUpdate" stored procedure.
   /// </summary>
   [Serializable]
   public class RegistrationRequestInfo : SelfHelpUserInfo
   {

      public String DeviceId { get; set; }
      public String UserHostAddress { get; set; }

      public RegistrationRequestInfo() : base()
      {
         base.ClearFields();

         RequestType = SelfHelpRequest.Unknown;
         RequestStatus = RequestStatus.Pending;
         DeviceId = String.Empty;
         UserHostAddress = String.Empty;
      }

      public RegistrationRequestInfo(UserAccountInfo account) : base(account)
      {
      }

      /*
      public void ReadData(System.Data.Common.DbDataReader reader)
      {
         CreatedDate = DataField.GetDateTime(reader[0]);
         SessionId = DataField.GetGuidString(reader[1]);
         SessionAlternateId = DataField.GetString(reader[2]);
         RequestId = DataField.GetString(reader[3]);
         UserId = DataField.GetString(reader[4]);
         RequestTypeNo = DataField.GetInt16(reader[5]);
         StatusNo = DataField.GetInt16(reader[6]);
         Verified = DataField.GetBool(reader[7]);
         GivenName = DataField.GetString(reader[8]);
         MiddleName = DataField.GetString(reader[9]);
         FullName = DataField.GetString(reader[10]);
         BirthDate = DataField.GetDateTime(reader[11]);
         OrganizationId = DataField.GetString(reader[12]);
         Email = DataField.GetString(reader[13]);
         PhoneNumber = DataField.GetString(reader[14]);
         PasswordText = DataField.GetString(reader[15]);
         ValidStatusNo = DataField.GetInt16(reader[16]);
      }
      */

   }

}

using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Entities
{

   /// <summary>
   /// System User details.
   /// </summary>
   [Serializable]
   public class UserBaseInfo : PersonIdentificationInfo, IUserBaseInfo
   {

      public String UserId { get; set; }
      public String DisplayName
      {
         get { return Name.DisplayName; }
         set { Name.DisplayName = value; }
      }

      public UserBaseInfo() : base()
      {
         //ClearFields();
      }

      public new void ClearFields()
      {
         base.ClearFields();
         UserId = String.Empty;
         DisplayName = String.Empty;
         Email = String.Empty;
         Phone.PhoneType = PhoneType.Mobile;
      }

      public void Copy(UserAccountInfo details)
      {
         BirthDate = details.BirthDate;
         DisplayName = details.DisplayName;
         Email = details.Email;
         Name.Copy(details.Name);
         Phone.Copy(details.Phone);
         OrganizationId = details.OrganizationId;
         PasswordText = details.PasswordText;
         Phone.PhoneType = details.Phone.PhoneType;
         PostalCode = details.PostalCode;
         SecurityAnswer = details.SecurityAnswer;
         SecurityQuestion = details.SecurityQuestion;
         SocialSecurityId = details.SocialSecurityId;
         DriverLicense.Id = details.DriverLicense.Id;
         DriverLicense.IdIssuer = details.DriverLicense.IdIssuer;

         StateProvince = details.StateProvince;
         UserId = details.UserId;
      }

   }

}

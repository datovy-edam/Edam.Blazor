using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Security;

namespace Edam.DataObjects.Entities
{

   /// <summary>
   /// System User details.
   /// </summary>
   //[ Serializable ]
   public class UserInfo : UserBaseInfo, IUserBaseInfo
   {
      private Password m_Password = new Password();

      //[ System.Xml.Serialization.XmlIgnore ]
      public Edam.Security.Password Password
      {
         get { return m_Password; }
         set { m_Password = value; }
      }
      
      public UserInfo() : base()
      {
         base.ClearFields();
         ClearFields();
      }

      public new void ClearFields()
      {
         base.ClearFields();
         Password.ClearFields();
      }

      /// <summary>
      /// Copy account details into this user.
      /// </summary>
      /// <param name="account">account data to copy</param>
      public new void Copy(UserAccountInfo account)
      {
         UserId = account.UserId;
         Name.Copy(account.Name);
         OrganizationId = account.OrganizationId;
         Email = account.Email;
         Phone.Copy(account.Phone);
         Password = new Security.Password(account.PasswordText);
         BirthDate = account.BirthDate;
      }

   }

}

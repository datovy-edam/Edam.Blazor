using System;
using System.Collections.Generic;
// using System.Data.Common;

// -----------------------------------------------------------------------------
// using DbField = Edam.Data.DataField;

namespace Edam.DataObjects.Entities
{

   /// <summary>
   /// Logged User details...
   /// </summary>
   public class UserLoggedInfo : IUserBaseInfo
   {
      public String UserId
      {
         get { return ReferenceId; }
         set { ReferenceId = value; }
      }

      public PersonName Name { get; set; } = new PersonName();

      public PhoneInfo Phone { get; set; } = new PhoneInfo();

      public String SessionId { get; set; }
      public String OrganizationId { get; set; }

      public String Alias { get; set; }
      public String Email { get; set; }

      public String GivenName
      {
         get { return Name.GivenName; }
         set { Name.GivenName = value; }
      }
      public String SurName
      {
         get { return Name.Surname; }
         set { Name.Surname = value; }
      }
      public String FatherLastName
      {
         get { return Name.FatherSurname; }
         set { Name.FatherSurname = value; }
      }
      public String MotherLastName
      {
         get { return Name.MotherSurname; }
         set { Name.MotherSurname = value; }
      }

      public String FullName
      {
         get { return Name.FullName; }
         set { Name.FullName = value; }
      }

      public UserLoggedStatus Status { get; set; }
      public Boolean IsActive { get; set; }
      public Boolean ItRequiresReset { get; set; }
      public Boolean ItRequiresRevision { get; set; }

      public Boolean HasMessages { get; set; }
      public Int32 MessageCount { get; set; }

      public String RequestId { get; set; }
      public String ReferenceId { get; set; }

      public String PinNumber { get; set; }

      public References.PreferencesBag PreferencesBag { get; set; }

      public List<OrganizationInfo> Organizations { get; set; }
      public List<Edam.Application.BasePolicyInfo> Policies { get; set; }

      public UserLoggedInfo()
      {
         Policies = new List<Edam.Application.BasePolicyInfo>();
         ClearFields();
      }

      public void ClearFields()
      {
         SessionId = String.Empty;
         OrganizationId = String.Empty;
         Alias = String.Empty;
         Email = String.Empty;
         GivenName = String.Empty;
         SurName = String.Empty;
         FatherLastName = String.Empty;
         MotherLastName = String.Empty;
         FullName = String.Empty;
         Status = UserLoggedStatus.Unknown;
         IsActive = false;
         ItRequiresReset = false;
         ItRequiresRevision = false;
         HasMessages = false;
         MessageCount = 0;
         RequestId = String.Empty;
         ReferenceId = String.Empty;
         if (Policies != null)
            Policies.Clear();
         PreferencesBag = new References.PreferencesBag();
         PinNumber = String.Empty;
      }

 #if DATA_SUPPORT_

      /// <summary>
      /// Read Data...
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <returns>instance of UserLoggedInfo is returned</returns>
      public static UserLoggedInfo ReadData(DbDataReader reader)
      {
         UserLoggedInfo u = new UserLoggedInfo();

         u.SessionId = DbField.GetString(reader[0]);
         u.OrganizationId = DbField.GetString(reader[1]);
         u.Alias = DbField.GetString(reader[2]);
         u.Email = DbField.GetString(reader[3]);
         u.GivenName = DbField.GetString(reader[4]);
         u.SurName = DbField.GetString(reader[5]);
         u.FatherLastName = DbField.GetString(reader[6]);
         u.MotherLastName = DbField.GetString(reader[7]);
         u.FullName = DbField.GetString(reader[8]);
         u.Status = (UserLoggedStatus)DbField.GetInt16(reader[9]);
         u.IsActive = DbField.GetBool(reader[10]);
         u.ItRequiresReset = DbField.GetBool(reader[11]);
         u.ItRequiresRevision = DbField.GetBool(reader[12]);
         u.HasMessages = DbField.GetBool(reader[13]);
         u.MessageCount = DbField.GetInt32(reader[14]);
         u.RequestId = DbField.GetString(reader[15]);
         u.ReferenceId = DbField.GetString(reader[16]);

         return u;
      }

#endif

   }

}

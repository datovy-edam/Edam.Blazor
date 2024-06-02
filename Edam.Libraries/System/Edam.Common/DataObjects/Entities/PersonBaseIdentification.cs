using System;
using System.Collections.Generic;
//using System.Data.Common;

// -----------------------------------------------------------------------------
//using DbField = Edam.Data.DataField;

namespace Edam.DataObjects.Entities
{

   public enum PersonNameType
   {

      Unknown = 0,

      // from NIEM Core
      Alternate = 1,
      Asserted = 2,
      Legal = 3,

      // from NIEM Justice Domain
      Aka = 4,
      Alias = 5,
      CallSign = 6,
      Dba = 7,       // doing business as
      Fka = 8,       // formerly known as 
      Handle = 9,
      Moniker = 10,
      Nickname = 11,
      Other = 12,
      Provided = 13,
      Pseudonym = 14,
      UserId = 15

   }

   public interface IPersonSecureIdentifificationInfo
   {
      String PasswordText { get; set; }
      String SecurityQuestion { get; set; }
      String SecurityAnswer { get; set; }
      String SocialSecurityId { get; set; }
      EntityIdInfo DriverLicense { get; set; }
   }

   public interface IPersonName
   {
      PersonNameType NameType { get; set; }
      String GivenName { get; set; }
      String MiddleName { get; set; }
      String FatherSurname { get; set; }
      String MotherSurname { get; set; }
      String NamePrefix { get; set; }
      String NameSuffix { get; set; }
      String FullName { get; set; }
      String DisplayName { get; set; }
   }

   public class PersonName : IPersonName
   {

      public PersonNameType NameType { get; set; }
      public String GivenName { get; set; }
      public String MiddleName { get; set; }

      public String Surname
      {
         get { return FatherSurname; }
         set { FatherSurname = value; }
      }

      public String NamePrefix { get; set; }
      public String NameSuffix { get; set; }

      public String FatherSurname { get; set; }
      public String MotherSurname { get; set; }

      private String m_FullName = null;
      public String FullName
      {
         get
         {
            var fname = m_FullName;
            if (String.IsNullOrEmpty(fname))
            {
               fname = GetFullName();
            }
            return fname;
         }
         set { m_FullName = value; }
      }

      public String DisplayName { get; set; }

      public PersonName()
      {
         ClearFields();
         NameType = PersonNameType.Unknown;
      }

      public void ClearFields()
      {
         GivenName = String.Empty;
         MiddleName = String.Empty;
         FatherSurname = String.Empty;
         MotherSurname = String.Empty;
         NamePrefix = String.Empty;
         NameSuffix = String.Empty;
         DisplayName = String.Empty;
      }

      public void Copy(IPersonName name)
      {
         GivenName = name.GivenName;
         MiddleName = name.MiddleName;
         FatherSurname = name.FatherSurname;
         MotherSurname = name.MotherSurname;
         NamePrefix = name.NamePrefix;
         NameSuffix = name.NameSuffix;
         FullName = name.FullName;
         DisplayName = name.DisplayName;
      }

      /// <summary>
      /// Given the father and mother last names, compose the sur name.
      /// </summary>
      /// <param name="fatherLastName">father last name</param>
      /// <param name="motherLastName">mother last name</param>
      /// <returns>composed sur name is returned</returns>
      public static String GetSurname(
         String fatherSurname, String motherSurname)
      {
         String surname = fatherSurname;
         if (String.IsNullOrEmpty(surname))
            surname = String.Empty;
         surname = surname.Trim();
         surname += " " + motherSurname;
         surname = surname.Trim();
         return surname;
      }

      public String GetSurname()
      {
         return GetSurname(FatherSurname, MotherSurname);
      }

      /// <summary>
      /// Get Surname...
      /// </summary>
      /// <param name="person">person details</param>
      /// <returns>surname is returned</returns>
      public static String GetSurname(PersonIdentificationInfo person)
      {
         return person.Name.GetSurname();
      }

      /// <summary>
      /// Get full name...
      /// </summary>
      /// <param name="givenName">given name</param>
      /// <param name="middleName">middle name</param>
      /// <param name="fatherLastName">father surname</param>
      /// <param name="motherLastName">mother surname</param>
      /// <returns>full name is returned</returns>
      public static String GetFullName(String givenName, String middleName,
         String fatherLastName, String motherLastName)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();
         var surName = GetSurname(fatherLastName, motherLastName).Trim();
         sb.Append(surName);
         if (!String.IsNullOrWhiteSpace(surName))
            sb.Append(", ");
         sb.Append(givenName + " " + middleName);
         return sb.ToString();
      }

      public String GetFullName()
      {
         return GetFullName(GivenName, MiddleName,
            FatherSurname, MotherSurname);
      }

      public String SetCamelName()
      {
         GivenName = Convert.ToTitleCase(GivenName);
         MiddleName = Convert.ToTitleCase(MiddleName);
         FatherSurname = Convert.ToTitleCase(FatherSurname);
         MotherSurname = Convert.ToTitleCase(MotherSurname);
         return FullName = GetFullName();
      }

   }

   public interface IPersonBaseIdentificationInfo
   {

      References.ReferenceType Type { get; set; }
      Objects.ObjectStatus Status { get; set; }

      String TypeDescription { get; set; }
      String StatusDescription { get; set; }

      String OrganizationId { get; set; }
      String EntityId { get; set; }
      String AlternateId { get; set; }
      String Description { get; set; }
      String Email { get; set; }
      PhoneInfo Phone { get; set; }
      String StateProvince { get; set; }
      String PostalCode { get; set; }
      DateTime? BirthDate { get; set; }
      Int64? DemographicNo { get; set; }

      PersonName Name { get; set; }

      Int32 NoteCount { get; set; }
   }

   public class PersonBaseIdentificationInfo : IPersonBaseIdentificationInfo
   {
      public References.ReferenceType Type { get; set; }
      public Objects.ObjectStatus Status { get; set; }

      public String TypeDescription { get; set; }
      public String StatusDescription { get; set; }

      public DateTime? CreatedDate { get; set; }
      public String OrganizationId { get; set; }
      public String EntityId { get; set; }
      public String AlternateId { get; set; }
      public String Description { get; set; }
      public String Email { get; set; }
      public PhoneInfo Phone { get; set; } = new PhoneInfo();
      public String StateProvince { get; set; }
      public String PostalCode { get; set; }
      public Int64? DemographicNo { get; set; }

      public PersonName Name { get; set; }

      private DateTime? m_BirthDate = null;
      public DateTime? BirthDate
      {
         get { return m_BirthDate; }
         set
         {
            if (value.HasValue)
            {
               if (value.Value.Year == 1800)
                  value = null;
            }
            m_BirthDate = value;
         }
      }

      public String EmailId { get; set; }

      public Int32 NoteCount { get; set; }

      public PersonBaseIdentificationInfo()
      {
         ClearFields();
      }

      public void Copy(PersonIdentificationInfo details)
      {
         CreatedDate = details.CreatedDate;
         OrganizationId = details.OrganizationId;
         EntityId = details.EntityId;
         AlternateId = details.AlternateId;
         Description = details.Description;
         Email = details.Email;
         Phone.Copy(details.Phone);
         StateProvince = details.StateProvince;
         PostalCode = details.PostalCode;
         BirthDate = details.BirthDate;

         Name.Copy(details.Name);

         Type = details.Type;
         TypeDescription = details.TypeDescription;
         Status = details.Status;
         StatusDescription = details.StatusDescription;

         NoteCount = details.NoteCount;

         EmailId = details.EmailId;
         DemographicNo = details.DemographicNo;
      }

      public void ClearFields()
      {
         CreatedDate = null;
         OrganizationId = String.Empty;
         EntityId = String.Empty;
         AlternateId = String.Empty;
         Description = String.Empty;
         Email = String.Empty;
         Phone.ClearFields();
         StateProvince = String.Empty;
         PostalCode = String.Empty;
         BirthDate = null;
         DemographicNo = null;

         Name.ClearFields();

         Type = References.ReferenceType.Person;
         TypeDescription = String.Empty;
         Status = Objects.ObjectStatus.Unknown;
         StatusDescription = String.Empty;

         EmailId = String.Empty;

         NoteCount = 0;
      }

   }

   public class PersonIdentificationInfo : PersonBaseIdentificationInfo,
      IPersonBaseIdentificationInfo, IPersonSecureIdentifificationInfo
   {
      public static readonly Int32 INDEX_TILL_NOTE = 24;

      public String PasswordText { get; set; }
      public String SecurityQuestion { get; set; }
      public String SecurityAnswer { get; set; }
      public String SocialSecurityId { get; set; }

      public EntityIdInfo DriverLicense { get; set; }

      public PersonIdentificationInfo() : base()
      {
         ClearFields();
      }

      public new void ClearFields()
      {
         base.ClearFields();
         PasswordText = String.Empty;
         SecurityQuestion = String.Empty;
         SecurityAnswer = String.Empty;
         SocialSecurityId = String.Empty;
         if (DriverLicense == null)
            DriverLicense = new EntityIdInfo();
         DriverLicense.ClearFields();
         DriverLicense.IdType = EntityIdType.DriverLicenseNo;
      }

      public new void Copy(PersonIdentificationInfo details)
      {
         base.Copy(details);
         PasswordText = details.PasswordText;
         SecurityQuestion = details.SecurityQuestion;
         SecurityAnswer = details.SecurityAnswer;
         SocialSecurityId = details.SocialSecurityId;
         DriverLicense = details.DriverLicense;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read Data...
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <param name="person">person record to populate</param>
      /// <returns>instance of OrganizationInfo is returned</returns>
      public static PersonIdentificationInfo ReadData(DbDataReader reader,
         PersonIdentificationInfo person = null)
      {
         PersonIdentificationInfo o = person == null ?
            new PersonIdentificationInfo() : person;

         o.CreatedDate = DbField.GetDateTime(reader[0]);
         o.OrganizationId = DbField.GetString(reader[1]);
         o.Email = DbField.GetString(reader[2]);
         o.Name.GivenName = DbField.GetString(reader[3]);
         o.Name.MiddleName = DbField.GetString(reader[4]);
         o.Name.FatherSurname = DbField.GetString(reader[5]);
         o.Name.MotherSurname = DbField.GetString(reader[6]);
         o.Name.FullName = DbField.GetString(reader[7]);
         o.SocialSecurityId = DbField.GetString(reader[8]);

         o.DriverLicense.ReferenceId = DbField.GetString(reader[9]);
         o.DriverLicense.Id = DbField.GetString(reader[10]);
         o.DriverLicense.IdIssuer = DbField.GetString(reader[11]);
         o.DriverLicense.IdIssuedDate = DbField.GetDateTime(reader[12]);
         o.DriverLicense.IdExpirationDate = DbField.GetDateTime(reader[13]);
         o.DriverLicense.IdStatus = 
            (Objects.ObjectStatus) DbField.GetInt16(reader[14]);
         o.DriverLicense.IdCategoryCode = DbField.GetString(reader[15]);

         o.PhoneNumber = DbField.GetString(reader[16]);
         o.BirthDate = DbField.GetDateTime(reader[17]);
         o.StateProvince = DbField.GetString(reader[18]);
         o.PostalCode = DbField.GetString(reader[19]);
         o.EntityId = DbField.GetString(reader[20]);
         o.Status = (Objects.ObjectStatus) DbField.GetInt16(reader[21]);
         o.DemographicNo = DbField.GetInt64(reader[22]);

         if (reader.FieldCount >= INDEX_TILL_NOTE)
            o.NoteCount = DbField.GetInt32(reader[23]);

         return o;
      }

      /// <summary>
      /// Read list.
      /// </summary>
      /// <param name="reader"></param>
      /// <returns>list of OrganizationInfo is returned</returns>
      public static List<PersonIdentificationInfo> GetList(DbDataReader reader)
      {
         List<PersonIdentificationInfo> l =
            new List<PersonIdentificationInfo>();
         PersonIdentificationInfo o;
         while (reader.Read())
         {
            o = ReadData(reader);
            l.Add(o);
         }
         return l;
      }

#endif

   }

}

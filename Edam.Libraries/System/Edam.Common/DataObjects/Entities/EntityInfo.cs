using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------
// Copied from Kif Library v4r0
//using Edam.Data;

namespace Edam.DataObjects.Entities
{

   /// <summary>
   /// Entity Info...
   /// </summary>
   public class EntityInfo
   {

      #region -- 1.00 Properties and Fields

      public DateTime? CreatedDate { get; set; }
      public String OrganizationId { get; set; }
      public References.ReferenceBaseType EntityIdType { get; set; }
      public String EntityId { get; set; }

      public PersonName Name { get; set; }

      public EntityIdType AlternateIdType { get; set; }
      public String AlternateId { get; set; }
      public UrlType UrlIdType { get; set; }
      public String UrlId { get; set; }
      public String UrlText { get; set; }

      public PhoneInfo Phone { get; set; } = new PhoneInfo();

      public EntityIdInfo IdentifierId { get; set; }

      public DateType ReferenceDateType { get; set; }
      private DateTime? m_ReferenceDate = null;
      public DateTime? ReferenceDate
      {
         get { return m_ReferenceDate; }
         set
         {
            if (value.HasValue)
            {
               if (value.Value.Year == 1800)
                  value = null;
            }
            m_ReferenceDate = value;
         }
      }

      public String StateCode { get; set; }
      public String PostalCode { get; set; }

      #endregion
      #region -- 1.50 Initialize Object

      public EntityInfo()
      {
         IdentifierId = new EntityIdInfo();
         ClearFields();
      }

      #endregion
      #region -- 4.00 Support Methods

      /// <summary>
      /// Clear Fields...
      /// </summary>
      public void ClearFields()
      {
         CreatedDate = null;
         OrganizationId = Edam.Application.Resources.Strings.DefaultAgencyId;
         EntityIdType = References.ReferenceBaseType.Personal;
         EntityId = String.Empty;
         AlternateIdType = Entities.EntityIdType.SocialSecurityNo;
         AlternateId = String.Empty;
         UrlIdType = UrlType.Email;
         UrlId = String.Empty;
         UrlText = String.Empty;
         Phone.ClearFields();
         Phone.PhoneType = PhoneType.Mobile;

         Name.ClearFields();

         if (IdentifierId == null)
            IdentifierId = new EntityIdInfo();
         IdentifierId.ClearFields();
         IdentifierId.IdType = Entities.EntityIdType.DriverLicenseNo;

         ReferenceDateType = DateType.ReferenceDate;
         ReferenceDate = null;
         StateCode = String.Empty;
         PostalCode = String.Empty;
      }

      public static void FixNullValues(EntityInfo record)
      {
         record.OrganizationId =
            Convert.ToNotNullString(record.OrganizationId);
         record.AlternateId = Convert.ToNotNullString(record.AlternateId);
         record.UrlId = Convert.ToNotNullString(record.UrlId);
         record.UrlText = Convert.ToNotNullString(record.UrlText);
         record.Phone.PhoneNumber =
            Convert.ToNotNullString(record.Phone.PhoneNumber);

         record.Name.Copy(record.Name);

         record.IdentifierId.Id =
            Convert.ToNotNullString(record.IdentifierId.Id);
         record.IdentifierId.IdIssuer =
            Convert.ToNotNullString(record.IdentifierId.IdIssuer);

         record.StateCode = Convert.ToNotNullString(record.StateCode);
         record.PostalCode = Convert.ToNotNullString(record.PostalCode);
      }

      #endregion
      #region -- 4.00 Entities Support Methods

      /// <summary>
      /// Copy person details into an entity record.
      /// </summary>
      /// <param name="person">person details</param>
      public void Copy(PersonIdentificationInfo person)
      {
         OrganizationId = person.OrganizationId;
         EntityIdType = References.ReferenceBaseType.Personal;
         EntityId = person.EntityId;
         UrlText = person.Email;
         UrlId = person.Email;
         UrlIdType = UrlType.Email;
         Name.Copy(person.Name);
         Phone.PhoneNumber = person.Phone.PhoneNumber;
         Phone.PhoneType = PhoneType.Mobile;
         AlternateId = person.SocialSecurityId;
         AlternateIdType = Entities.EntityIdType.SocialSecurityNo;
         StateCode = person.StateProvince;
         PostalCode = person.PostalCode;
         Name.FullName = person.Name.GetFullName();

         // driver license ID
         IdentifierId.Copy(person.DriverLicense,
            Entities.EntityIdType.DriverLicenseNo);

         ReferenceDateType = DateType.DateOfBirth;
         ReferenceDate = person.BirthDate;
         UrlId = String.Empty;
      }

      #endregion

#if DATA_SUPPORT_

      #region -- 4.00 Data Reader Support Methods

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static EntityInfo ReadData(
         EntityInfo record, System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new EntityInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.CreatedDate = DataField.GetDateTime(reader[0]);
         record.OrganizationId = DataField.GetString(reader[1]);
         record.EntityIdType =
            (References.ReferenceBaseType)DataField.GetInt16(reader[2]);
         record.EntityId = DataField.GetString(reader[3]);
         record.PersonGivenName = DataField.GetString(reader[4]);
         record.PersonMiddleName = DataField.GetString(reader[5]);
         record.PersonFatherLastName = DataField.GetString(reader[6]);
         record.PersonMotherLastName = DataField.GetString(reader[7]);
         record.FullName = DataField.GetString(reader[8]);
         record.AlternateIdType = (EntityIdType)DataField.GetInt16(reader[9]);
         record.AlternateId = DataField.GetString(reader[10]);
         record.UrlIdType = (UrlType)DataField.GetInt16(reader[11]);
         record.UrlId = DataField.GetString(reader[12]);
         record.UrlText = DataField.GetString(reader[13]);
         record.PhoneNumberType = (PhoneType)DataField.GetInt16(reader[14]);
         record.PhoneNumber = DataField.GetString(reader[15]);
         record.IdentifierIdType = (EntityIdType)DataField.GetInt16(reader[16]);
         record.IdentifierId = DataField.GetString(reader[17]);
         record.IdentifierIdIssuer = DataField.GetString(reader[18]);
         record.ReferenceDateType = (DateType)DataField.GetInt16(reader[19]);
         record.ReferenceDate = DataField.GetDateTime(reader[20]);
         record.StateCode = DataField.GetString(reader[21]);
         record.PostalCode = DataField.GetString(reader[22]);

         return record;
      }

      /// <summary>
      /// Prepare a list with data supplied in given reader.
      /// </summary>
      /// <param name="list">list to add items too</param>
      /// <param name="reader">reader (source of data)</param>
      /// <returns>instance List of NoteInfo'es</returns>
      public static List<EntityInfo> GetList(
         List<EntityInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         EntityInfo entity;
         if (list == null)
            list = new List<EntityInfo>();
         while(reader.Read())
         {
            entity = new EntityInfo();
            ReadData(entity, reader);
            list.Add(entity);
         }
         return list;
      }

      #endregion
      #region -- 4.00 Data Reader Person Base Identification Support Methods

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static PersonBaseIdentificationInfo ReadData(
         PersonBaseIdentificationInfo record,
         System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new PersonBaseIdentificationInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         //record.CreatedDate = DataField.GetDateTime(reader[0]);
         record.OrganizationId = DataField.GetString(reader[1]);
         //record.EntityIdType =
         //   (References.ReferenceBaseType)DataField.GetInt16(reader[2]);
         record.EntityId = DataField.GetString(reader[3]);
         record.GivenName = DataField.GetString(reader[4]);
         record.MiddleName = DataField.GetString(reader[5]);
         record.FatherLastName = DataField.GetString(reader[6]);
         record.MotherLastName = DataField.GetString(reader[7]);
         record.FullName = DataField.GetString(reader[8]);
         //record.AlternateIdType = (EntityIdType)DataField.GetInt16(reader[9]);
         //record.AlternateId = DataField.GetString(reader[10]);
         //record.UrlIdType = (UrlType)DataField.GetInt16(reader[11]);
         //record.UrlId = DataField.GetString(reader[12]);
         record.Email = DataField.GetString(reader[13]);
         //record.PhoneNumberType = (PhoneType)DataField.GetInt16(reader[14]);
         record.PhoneNumber = DataField.GetString(reader[15]);
         //record.IdentifierIdType = (EntityIdType)DataField.GetInt16(reader[16]);
         //record.IdentifierId = DataField.GetString(reader[17]);
         //record.ReferenceDateType = (DateType)DataField.GetInt16(reader[18]);
         record.BirthDate = DataField.GetDateTime(reader[19]);
         record.StateProvince = DataField.GetString(reader[20]);
         record.PostalCode = DataField.GetString(reader[21]);

         return record;
      }

      /// <summary>
      /// Prepare a list with data supplied in given reader.
      /// </summary>
      /// <param name="list">list to add items too</param>
      /// <param name="reader">reader (source of data)</param>
      /// <returns>instance List of NoteInfo'es</returns>
      public static List<PersonBaseIdentificationInfo> GetList(
         List<PersonBaseIdentificationInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         PersonBaseIdentificationInfo entity;
         if (list == null)
            list = new List<PersonBaseIdentificationInfo>();
         while (reader.Read())
         {
            entity = new PersonBaseIdentificationInfo();
            ReadData(entity, reader);
            list.Add(entity);
         }
         return list;
      }

      #endregion

#endif

   }

}


using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Locations;
using Edam.DataObjects.References;
using Edam.Application.Resources;
//using DbField = Edam.Data.DataField;

namespace Edam.DataObjects.Entities
{

   public class OrganizationAgentInfo
   {

      public Int64 AssociationNo { get; set; }

      /// <summary>
      /// Parent Organization ID.
      /// </summary>
      public String OrganizationId { get; set; }
      
      /// <summary>
      /// Entity Organization details (e.g. Employer)
      /// </summary>
      public OrganizationInfo Organization { get; set; }

      /// <summary>
      /// Entity Reference details (e.g. Agent)
      /// </summary>
      public PersonBaseIdentificationInfo Agent { get; set; }

      public LocationAddressInfo Location { get; set; }

      public OrganizationAgentInfo()
      {
         Organization = new OrganizationInfo();
         Agent = new PersonBaseIdentificationInfo();
         Location = new LocationAddressInfo();
         ClearFields();
      }

      public void ClearFields()
      {
         OrganizationId = String.Empty;
         AssociationNo = 0;
         Organization.ClearFields();
         Agent.ClearFields();
         Location.ClearFields();
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read Data.
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <returns>instace of LocationAddressInfo is returned</returns>
      public static OrganizationAgentInfo ReadData(
         System.Data.Common.DbDataReader reader)
      {
         OrganizationAgentInfo a = new OrganizationAgentInfo();

         OrganizationInfo o = a.Organization;
         PersonBaseIdentificationInfo p = a.Agent;
         LocationAddressInfo l = a.Location;

         a.OrganizationId = DbField.GetString(reader[0]);

         o.Type = (ReferenceBaseType)DbField.GetInt16(reader[1]);
         Int16 entTypeNo = DbField.GetInt16(reader[2]);
         o.OrganizationId = DbField.GetString(reader[3]);
         o.AlternateId = DbField.GetString(reader[4]);
         o.Alias = DbField.GetString(reader[5]);
         o.OrganizationName = DbField.GetString(reader[6]);

         p.OrganizationId = o.OrganizationId;
         p.EntityId = DbField.GetString(reader[7]);
         p.AlternateId = DbField.GetString(reader[8]);
         p.FullName = DbField.GetString(reader[9]);
         p.Description = DbField.GetString(reader[10]);

         l.LocationId = DbField.GetString(reader[11]);
         l.Neighborhood = DbField.GetString(reader[12]);
         l.Alias = DbField.GetString(reader[13]);
         l.AttentionTo = DbField.GetString(reader[14]);
         l.Type = LocationType.MailingAddress;
         l.Line1 = DbField.GetString(reader[15]);
         l.Line2 = DbField.GetString(reader[16]);
         l.Country = Edam.Application.Resources.Strings.DefaultCountry;
         l.Category = LocationCategory.AgencyFacilities;
         l.CityName = DbField.GetString(reader[17]);
         l.StateCode = DbField.GetString(reader[18]);
         l.PostalCode = DbField.GetString(reader[19]);

         p.EmailId = DbField.GetString(reader[20]);
         p.Email = DbField.GetString(reader[21]);
         p.PhoneId = DbField.GetString(reader[22]);
         p.PhoneNumber = DbField.GetString(reader[23]);

         a.AssociationNo = DbField.GetInt64(reader[24]);

         return a;
      }

      /// <summary>
      /// Get List of addresses..
      /// </summary>
      /// <param name="reader">data reader to read addresses from</param>
      /// <returns>List of LocationAddressInfoes is returned</returns>
      public static List<OrganizationAgentInfo> GetList(
         System.Data.Common.DbDataReader reader)
      {
         List<OrganizationAgentInfo> l = new List<OrganizationAgentInfo>();
         while (reader.Read())
         {
            l.Add(ReadData(reader));
         }
         return l;
      }

#endif

   }

}

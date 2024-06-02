using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.DataObjects.References;
// using DbField = Edam.Data.DataField;

namespace Edam.DataObjects.Entities
{

   public class EntityContactInfo
   {

      public String OrganizationId { get; set; }
      public String OrganizationName { get; set; }
      public ReferenceBaseType AssociationType { get; set; }
      public String EntityId { get; set; }
      public String EntityAlternateId { get; set; }
      public String EntityAlias { get; set; }
      public String EntityDescription { get; set; }
      public String EntityEmailUrlId { get; set; }
      public String EntityEmail { get; set; }
      public String EntityPhoneNumber { get; set; }

      public String StateCode { get; set; }
      public String PostalCode { get; set; }

      public DateTime LastUpdateDate { get; set; }

      public EntityContactInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         OrganizationId = String.Empty;
         OrganizationName = String.Empty;
         AssociationType = ReferenceBaseType.Agent;
         EntityId = String.Empty;
         EntityAlternateId = String.Empty;
         EntityAlias = String.Empty;
         EntityDescription = String.Empty;
         EntityEmailUrlId = String.Empty;
         EntityEmail = String.Empty;
         EntityPhoneNumber = String.Empty;
         StateCode = String.Empty;
         PostalCode = String.Empty;
         LastUpdateDate = DateTime.Now;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read Data.
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <returns>instace of LocationAddressInfo is returned</returns>
      public static EntityContactBaseInfo ReadData(
         System.Data.Common.DbDataReader reader)
      {
         EntityContactBaseInfo t = new EntityContactBaseInfo();

         t.OrganizationId = DbField.GetString(reader[0]);
         t.OrganizationName = DbField.GetString(reader[1]);
         t.AssociationType = (ReferenceBaseType)DbField.GetInt16(reader[2]);
         t.EntityId = DbField.GetString(reader[3]);
         t.EntityAlternateId = DbField.GetString(reader[4]);
         t.EntityAlias = DbField.GetString(reader[5]);
         t.EntityDescription = DbField.GetString(reader[6]);
         t.EntityEmailUrlId = DbField.GetString(reader[7]);
         t.EntityEmail = DbField.GetString(reader[8]);
         t.EntityPhoneNumber = DbField.GetString(reader[9]);
         t.StateCode = DbField.GetString(reader[10]);
         t.PostalCode = DbField.GetString(reader[11]);
         
         return t;
      }

      /// <summary>
      /// Get List of addresses..
      /// </summary>
      /// <param name="reader">data reader to read addresses from</param>
      /// <returns>List of TenantAgentInfo is returned</returns>
      public static List<EntityContactBaseInfo> GetList(
         System.Data.Common.DbDataReader reader)
      {
         List<EntityContactBaseInfo> l = new List<EntityContactBaseInfo>();
         while (reader.Read())
         {
            l.Add(ReadData(reader));
         }
         return l;
      }

#endif

   }

}

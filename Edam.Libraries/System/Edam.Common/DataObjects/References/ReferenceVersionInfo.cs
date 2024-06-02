using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
// using Edam.Data;

namespace Edam.DataObjects.References
{

   public class ReferenceVersionInfo
   {

      public DateTime CreatedDate { get; set; }
      public DateTime LastUpdate { get; set; }
      public String OrganizationId { get; set; }
      public String ApplicationId { get; set; }
      public Int32 VersionNo { get; set; }
      public String VersionId { get; set; }
      public Objects.ObjectStatus Status { get; set; }
      public Objects.ObjectScope Scope { get; set; }
      public String CountryCode { get; set; }
      public String StateCode { get; set; }
      public String GlAccountId { get; set; }
      public String Description { get; set; }

      public ReferenceVersionInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         CreatedDate = DateTime.Now;
         LastUpdate = DateTime.Now;
         OrganizationId = Edam.Application.Resources.Strings.DefaultAgencyId;
         ApplicationId = String.Empty;
         VersionNo = 0;
         VersionId = String.Empty;
         Status = Objects.ObjectStatus.Active;
         Scope = Objects.ObjectScope.Public;
         CountryCode = String.Empty;
         StateCode = String.Empty;
         GlAccountId = String.Empty;
         Description = String.Empty;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ReferenceVersionInfo ReadData(
         ReferenceVersionInfo record,
         System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ReferenceVersionInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.CreatedDate = DataField.GetDateTime(reader[0]);
         record.LastUpdate = DataField.GetDateTime(reader[1]);
         record.OrganizationId = DataField.GetString(reader[2]);
         record.ApplicationId = DataField.GetString(reader[3]);
         record.VersionNo = DataField.GetInt32(reader[4]);
         record.VersionId = DataField.GetString(reader[5]);
         record.Status = (Objects.ObjectStatus)DataField.GetInt16(reader[6]);
         record.Scope = (Objects.ObjectScope)DataField.GetInt16(reader[7]);
         record.CountryCode = DataField.GetString(reader[8]);
         record.StateCode = DataField.GetString(reader[9]);
         record.GlAccountId = DataField.GetString(reader[10]);
         record.Description = DataField.GetString(reader[11]);

         return record;
      }

      public void ReadData(System.Data.Common.DbDataReader reader)
      {
         ReadData(this, reader);
      }

      /// <summary>
      /// Prepare a list with data supplied in given reader.
      /// </summary>
      /// <param name="list">list to add items too</param>
      /// <param name="reader">reader (source of data)</param>
      /// <returns>instance List of ActivityProgramInfo'es</returns>
      public static List<ReferenceVersionInfo> GetList(
         List<ReferenceVersionInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ReferenceVersionInfo record;
         if (list == null)
            list = new List<ReferenceVersionInfo>();
         while (reader.Read())
         {
            record = new ReferenceVersionInfo();
            record.ReadData(reader);
            list.Add(record);
         }
         return list;
      }

#endif

   }

}

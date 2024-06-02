using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data;

namespace Edam.DataObjects.Activities
{

   public class ActivityContentVersionInfo
   {

      public String OrganizationId { get; set; }
      public String ApplicationId { get; set; }
      public Int32 VersionNo { get; set; }
      public String VersionId { get; set; }
      public String IdentifierId { get; set; }
      public String ThreadId { get; set; }
      public Objects.ObjectStatus Status { get; set; }
      public Objects.ObjectScope Scope { get; set; }
      public String CountryCode { get; set; }
      public String StateCode { get; set; }
      public String GlAccountId { get; set; }
      public Decimal TotalHours { get; set; }
      public Decimal UnitPrice { get; set; }
      public String Alias { get; set; }
      public String Abstract { get; set; }

      public ActivityContentVersionInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         OrganizationId = Edam.Application.Resources.Strings.DefaultAgencyId;
         ApplicationId = String.Empty;
         VersionNo = 0;
         VersionId = String.Empty;
         IdentifierId = String.Empty;
         ThreadId = String.Empty;
         Status = Objects.ObjectStatus.Active;
         Scope = Objects.ObjectScope.Public;
         CountryCode = String.Empty;
         StateCode = String.Empty;
         GlAccountId = String.Empty;
         TotalHours = 0.0M;
         UnitPrice = 0.0M;
         Alias = String.Empty;
         Abstract = String.Empty;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ActivityContentVersionInfo ReadData(
         ActivityContentVersionInfo record, 
         System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ActivityContentVersionInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }
         
         record.OrganizationId = DataField.GetString(reader[0]);
         record.ApplicationId = DataField.GetString(reader[1]);
         record.VersionNo = DataField.GetInt32(reader[2]);
         record.VersionId = DataField.GetString(reader[3]);
         record.IdentifierId = DataField.GetString(reader[4]);
         record.ThreadId = DataField.GetString(reader[5]);
         record.Status = (Objects.ObjectStatus) DataField.GetInt16(reader[6]);
         record.Scope = (Objects.ObjectScope) DataField.GetInt16(reader[7]);
         record.CountryCode = DataField.GetString(reader[8]);
         record.StateCode = DataField.GetString(reader[9]);
         record.GlAccountId = DataField.GetString(reader[10]);
         record.TotalHours = DataField.GetDecimal(reader[11]);
         record.UnitPrice = DataField.GetDecimal(reader[12]);
         record.Alias = DataField.GetString(reader[13]);
         record.Abstract = DataField.GetString(reader[14]);

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
      public static List<ActivityContentVersionInfo> GetList(
         List<ActivityContentVersionInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ActivityContentVersionInfo record;
         if (list == null)
            list = new List<ActivityContentVersionInfo>();
         while (reader.Read())
         {
            record = new ActivityContentVersionInfo();
            record.ReadData(reader);
            list.Add(record);
         }
         return list;
      }

#endif

   }

}

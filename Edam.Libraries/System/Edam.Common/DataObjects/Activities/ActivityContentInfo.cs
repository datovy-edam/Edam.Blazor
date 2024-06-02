using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data;

namespace Edam.DataObjects.Activities
{

   public class ActivityContentInfo
   {

      #region -- 1.00 Properties and Fields

      public DateTime CreatedDate { get; set; }
      public String OrganizationId { get; set; }
      public String ThreadId { get; set; }
      public String IdentifierId { get; set; }
      public ActivityContentType Type { get; set; }
      public Edam.DataObjects.Objects.ObjectStatus Status { get; set; }
      public String Alias { get; set; }
      public String Abstract { get; set; }
      public String DaysPattern { get; set; }
      public TimeSpan StartTime { get; set; }
      public TimeSpan EndTime { get; set; }
      public Decimal UnitHours { get; set; }
      public Decimal TotalHours { get; set; }
      public String GlAccountId { get; set; }
      public Decimal FeeAmount { get; set; }

      #endregion

      public ActivityContentInfo()
      {
         ClearFields();
      }

      /// <summary>
      /// Clear Fields...
      /// </summary>
      public void ClearFields()
      {
         CreatedDate = Edam.Application.Defaults.NullDate;
         OrganizationId = String.Empty;
         ThreadId = String.Empty;
         IdentifierId = String.Empty;
         Type = ActivityContentType.Unknown;
         Status = Objects.ObjectStatus.Unknown;
         Alias = String.Empty;
         Abstract = String.Empty;
         DaysPattern = String.Empty;
         StartTime = new TimeSpan(0, 0, 0);
         EndTime = new TimeSpan(0, 0, 0);
         UnitHours = new Decimal(0.0);
         TotalHours = new Decimal(0.0);
         GlAccountId = String.Empty;
         FeeAmount = new Decimal(0.0);
      }

      public static void FixNullValues(ActivityContentInfo record)
      {
         record.OrganizationId =
            Edam.Convert.ToNotNullString(record.OrganizationId);
         record.ThreadId =
            Edam.Convert.ToNotNullString(record.ThreadId);
         record.IdentifierId =
            Edam.Convert.ToNotNullString(record.IdentifierId);
         record.Alias =
            Edam.Convert.ToNotNullString(record.Alias);
         record.Abstract =
            Edam.Convert.ToNotNullString(record.Abstract);
         record.GlAccountId =
            Edam.Convert.ToNotNullString(record.GlAccountId);
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ActivityContentInfo ReadData(
         ActivityContentInfo record, System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ActivityContentInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.CreatedDate = DataField.GetDateTime(reader[0]);
         record.OrganizationId = DataField.GetString(reader[1]);
         record.ThreadId = DataField.GetString(reader[2]);
         record.IdentifierId = DataField.GetString(reader[3]);
         record.Type = (ActivityContentType)DataField.GetInt16(reader[4]);
         record.Status = (Objects.ObjectStatus)DataField.GetInt16(reader[5]);
         record.Alias = DataField.GetString(reader[6]);
         record.Abstract = DataField.GetString(reader[7]);
         record.DaysPattern = DataField.GetString(reader[8]);
         record.StartTime = DataField.GetTimeSpan(reader[9]);
         record.EndTime = DataField.GetTimeSpan(reader[10]);
         record.UnitHours = DataField.GetDecimal(reader[11]);
         record.TotalHours = DataField.GetDecimal(reader[12]);
         record.GlAccountId = DataField.GetString(reader[13]);
         record.FeeAmount = DataField.GetDecimal(reader[14]);

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
      /// <returns>instance List of ActivityThreadInfo'es</returns>
      public static List<ActivityContentInfo> GetList(
         List<ActivityContentInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ActivityContentInfo record;
         if (list == null)
            list = new List<ActivityContentInfo>();
         while (reader.Read())
         {
            record = new ActivityContentInfo();
            record.ReadData(reader);
            list.Add(record);
         }
         return list;
      }

#endif

   }

}

using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data;

namespace Edam.DataObjects.Activities
{

   public class ActivityRatedItemInfo
   {

      #region -- 1.00 Properties and Fields

      public String OrganizationId { get; set; }
      public String OrganizationName { get; set; }
      public String EntityId { get; set; }
      public String ThreadId { get; set; }
      public String ThemeName { get; set; }
      public String SessionName { get; set; }
      public String ParticipantName { get; set; }
      public DateTime ActivityDate { get; set; }
      public String ActivityName { get; set; }
      public Decimal RatedValue { get; set; }

      #endregion

      public ActivityRatedItemInfo()
      {
         ClearFields();
      }

      /// <summary>
      /// Clear Fields...
      /// </summary>
      public void ClearFields()
      {
         OrganizationId = String.Empty;
         OrganizationName = String.Empty;
         EntityId = String.Empty;
         ThreadId = String.Empty;
         ThemeName = String.Empty;
         SessionName = String.Empty;
         ParticipantName = String.Empty;
         ActivityDate = Edam.Application.Defaults.NullDate;
         ActivityName = String.Empty;
         RatedValue = (Decimal)0.0;
      }

      public static void FixNullValues(ActivityRatedItemInfo record)
      {
         record.OrganizationId =
            Edam.Convert.ToNotNullString(record.OrganizationId);
         record.OrganizationName =
            Edam.Convert.ToNotNullString(record.OrganizationName);
         record.EntityId =
            Edam.Convert.ToNotNullString(record.EntityId);
         record.ThreadId =
            Edam.Convert.ToNotNullString(record.ThreadId);
         record.ThemeName =
            Edam.Convert.ToNotNullString(record.ThemeName);
         record.ParticipantName =
            Edam.Convert.ToNotNullString(record.ParticipantName);
         record.ActivityName =
            Edam.Convert.ToNotNullString(record.ActivityName);
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ActivityRatedItemInfo ReadData(
         ActivityRatedItemInfo record,
         System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ActivityRatedItemInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.OrganizationId = DataField.GetString(reader[0]);
         record.OrganizationName = DataField.GetString(reader[1]);
         record.EntityId = DataField.GetString(reader[2]);
         record.ThreadId = DataField.GetString(reader[3]);
         record.ThemeName = DataField.GetString(reader[4]);
         record.SessionName = DataField.GetString(reader[5]);
         record.ParticipantName = DataField.GetString(reader[6]);
         record.ActivityDate = DataField.GetDateTime(reader[7]);
         record.ActivityName = DataField.GetString(reader[8]);
         record.RatedValue = DataField.GetDecimal(reader[9]);

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
      public static List<ActivityRatedItemInfo> GetList(
         List<ActivityRatedItemInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ActivityRatedItemInfo record;
         if (list == null)
            list = new List<ActivityRatedItemInfo>();
         while (reader.Read())
         {
            record = new ActivityRatedItemInfo();
            record.ReadData(reader);
            list.Add(record);
         }
         return list;
      }

#endif

   }

}

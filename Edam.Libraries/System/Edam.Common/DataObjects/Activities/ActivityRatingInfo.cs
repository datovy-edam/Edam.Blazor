using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data;

namespace Edam.DataObjects.Activities
{

   public class ActivityRatingInfo
   {

      #region -- 1.00 Properties and Fields

      public String OrganizationId { get; set; }
      public String ReferenceId { get; set; }
      public DateTime ReferenceDate { get; set; }
      public String EvaluationId { get; set; }
      public ActivityRatingType Type { get; set; }
      public String Alias { get; set; }
      public Objects.ObjectStatus Status { get; set; }
      public Decimal Weight { get; set; }

      #endregion

      public ActivityRatingInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         OrganizationId = String.Empty;
         ReferenceId = String.Empty;
         ReferenceDate = Edam.NullDateTime.Value;
         EvaluationId = String.Empty;
         Type = ActivityRatingType.Unknown;
         Alias = String.Empty;
         Status = Objects.ObjectStatus.Active;
         Weight = new Decimal(1.0);
      }

      public static void FixNullValues(ActivityRatingInfo record)
      {
         record.OrganizationId =
            Edam.Convert.ToNotNullString(record.OrganizationId);
         record.ReferenceId =
            Edam.Convert.ToNotNullString(record.ReferenceId);
         record.Alias =
            Edam.Convert.ToNotNullString(record.Alias);
         record.EvaluationId =
            Edam.Convert.ToNotNullString(record.EvaluationId);
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ActivityRatingInfo ReadData(
         ActivityRatingInfo record, System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ActivityRatingInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.OrganizationId = DataField.GetString(reader[0]);
         record.ReferenceId = DataField.GetString(reader[1]);
         record.ReferenceDate = DataField.GetDateTime(reader[2]);
         record.EvaluationId = DataField.GetString(reader[3]);
         record.Type = (ActivityRatingType)DataField.GetInt16(reader[4]);
         record.Alias = DataField.GetString(reader[5]);
         record.Status = (Objects.ObjectStatus)DataField.GetInt16(reader[6]);
         record.Weight = DataField.GetDecimal(reader[7]);

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
      public static List<ActivityRatingInfo> GetList(
         List<ActivityRatingInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ActivityRatingInfo record;
         if (list == null)
            list = new List<ActivityRatingInfo>();
         while (reader.Read())
         {
            record = new ActivityRatingInfo();
            record.ReadData(reader);
            list.Add(record);
         }
         return list;
      }

#endif

   }

}

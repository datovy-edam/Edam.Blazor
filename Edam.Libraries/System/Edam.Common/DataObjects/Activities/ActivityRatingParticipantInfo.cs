using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data;
using Reference = Edam.DataObjects.References;

namespace Edam.DataObjects.Activities
{

   public class ActivityRatingParticipantInfo
   {

      #region -- 1.00 Properties and Fields

      public Int16 ItemNo { get; set; }
      public String OrganizationId { get; set; }
      public String EvaluationId { get; set; }
      public String ParticipantId { get; set; }
      public ActivityParticipantResultType Result { get; set; }
      public Int16 ParticipationCount { get; set; }
      public Decimal RatedValue { get; set; }
      public String ParticipantAlias { get; set; }
      public Reference.ReferenceBaseType Role { get; set; }
      public String RoleDescription { get; set; }

      #endregion

      public ActivityRatingParticipantInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         OrganizationId = String.Empty;
         EvaluationId = String.Empty;
         ParticipantId = String.Empty;
         Result = ActivityParticipantResultType.Assisted;
         ParticipationCount = 0;
         RatedValue = 0;
         ParticipantAlias = String.Empty;
         Role = Reference.ReferenceBaseType.Unknown;
         RoleDescription = String.Empty;
      }

      public static void FixNullValues(ActivityRatingParticipantInfo record)
      {
         record.OrganizationId =
            Edam.Convert.ToNotNullString(record.OrganizationId);
         record.EvaluationId =
            Edam.Convert.ToNotNullString(record.EvaluationId);
         record.ParticipantAlias =  
            Edam.Convert.ToNotNullString(record.ParticipantAlias);
         record.ParticipantId =
            Edam.Convert.ToNotNullString(record.ParticipantId);
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ActivityRatingParticipantInfo ReadData(
         ActivityRatingParticipantInfo record,
         System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ActivityRatingParticipantInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.OrganizationId = DataField.GetString(reader[0]);
         record.EvaluationId = DataField.GetString(reader[1]);
         record.ParticipantId = DataField.GetString(reader[2]);
         record.Result =
            (ActivityParticipantResultType)DataField.GetInt16(reader[3]);
         record.ParticipationCount = DataField.GetInt16(reader[4]);
         record.RatedValue = DataField.GetDecimal(reader[5]);
         record.ParticipantAlias = DataField.GetString(reader[6]);
         record.Role = (Reference.ReferenceBaseType)
            DataField.GetInt16(reader[7]);
         record.RoleDescription = DataField.GetString(reader[8]);

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
      public static List<ActivityRatingParticipantInfo> GetList(
         List<ActivityRatingParticipantInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ActivityRatingParticipantInfo record;
         if (list == null)
            list = new List<ActivityRatingParticipantInfo>();
         while (reader.Read())
         {
            record = new ActivityRatingParticipantInfo();
            record.ReadData(reader);
            list.Add(record);
         }
         return list;
      }

#endif

   }

}

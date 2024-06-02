using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using ReqResp = Edam.DataObjects.Requests;
using Edam.DataObjects.References;
using Edam.Data;
using Edam.Diagnostics;
using Edam.DataObjects.Objects;
using Edam.DataObjects.Entities;
using Edam.DataObjects.Notes;

namespace Edam.DataObjects.Activities
{

   public class ActivityFollowUpScheduleInfo
   {

      #region -- 1.00 Properties and Fields

      public String OrganizationId { get; set; }
      public String OrganizationName { get; set; }

      /// <summary>
      /// The Agent, person or entity doing the Follow-Up...
      /// </summary>
      public String AgentId { get; set; }
      /// <summary>
      /// Agent Email...
      /// </summary>
      public String AgentEmail { get; set; }
      /// <summary>
      /// Agent Alias (name)...
      /// </summary>
      public String AgentAlias { get; set; }

      public String FollowUpId { get; set; }
      public Int64 FollowUpNo { get; set; }
      public DateTime? FollowUpDate { get; set; }

      /// <summary>
      /// Last related Entity involved in the follow-Up (eg. Employer)
      /// </summary>
      public String EntityAlias { get; set; }

      /// <summary>
      /// Alias is the name or title of what is being followed-Up
      /// </summary>
      public String Alias { get; set; }
      /// <summary>
      /// Email of what is being followed-up
      /// </summary>
      public String Email { get; set; }

      public DateTime? ServiceStartTime { get; set; }
      public DateTime? ServiceEndTime { get; set; }

      public DateTime? ServedtartTime { get; set; }
      public DateTime? ServedEndTime { get; set; }

      public DateTime? ServedDate { get; set; }

      public EntityCompensationType CompensationType { get; set; }

      public Int16 Quantity { get; set; }

      public Decimal AmountStart { get; set; }
      public Decimal AmountEnd { get; set; }
      public Decimal AmountReported { get; set; }

      public ObjectStatus Status { get; set; }

      #endregion
      #region -- 1.50 Initialize Object

      public ActivityFollowUpScheduleInfo()
      {
         ClearFields();
      }

      public ActivityFollowUpScheduleInfo(String organizationId, 
         String followUpId, Int64 followUpNo)
      {
         ClearFields();
         OrganizationId = organizationId;
         FollowUpId = followUpId;
         FollowUpNo = followUpNo;
      }

      #endregion
      #region -- 4.00 Support Methods

      public void ClearFields()
      {
         OrganizationId = String.Empty;
         AgentId = String.Empty;
         FollowUpId = String.Empty;
         FollowUpNo = -1;
         FollowUpDate = null;
         EntityAlias = String.Empty;
         Alias = String.Empty;
         ServiceStartTime = null;
         ServiceEndTime = null;
         ServedtartTime = null;
         ServedEndTime = null;
         ServedDate = null;
         CompensationType = EntityCompensationType.Unknown;
         Quantity = 0;
         AmountStart = 0;
         AmountEnd = 0;
         AmountReported = 0;
         Status = ObjectStatus.Unknown;
         AgentAlias = String.Empty;
         AgentEmail = String.Empty;
         Email = String.Empty;
      }

      #endregion
      #region -- 4.00 Data Reader Support Methods

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ActivityFollowUpScheduleInfo ReadData(
         ActivityFollowUpScheduleInfo record, 
         System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ActivityFollowUpScheduleInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.OrganizationId = DataField.GetString(reader[0]);
         record.AgentId = DataField.GetString(reader[1]);
         record.FollowUpId = DataField.GetString(reader[2]);
         record.FollowUpNo = DataField.GetInt64(reader[3]);
         record.FollowUpDate = DataField.GetNullableDateTime(reader[4]);
         record.EntityAlias = DataField.GetString(reader[5]);
         record.Alias = DataField.GetString(reader[6]);

         record.ServiceStartTime = DataField.GetNullableDateTime(reader[7]);
         record.ServiceEndTime = DataField.GetNullableDateTime(reader[8]);
         record.ServedtartTime = DataField.GetNullableDateTime(reader[9]);
         record.ServedEndTime = DataField.GetNullableDateTime(reader[10]);
         record.ServedDate = DataField.GetNullableDateTime(reader[11]);

         record.CompensationType = 
            (EntityCompensationType) DataField.GetInt16(reader[12]);
         record.Quantity = DataField.GetInt16(reader[13]);
         record.AmountStart = DataField.GetDecimal(reader[14]);
         record.AmountEnd = DataField.GetDecimal(reader[15]);
         record.AmountReported = DataField.GetDecimal(reader[16]);
         record.Status = (ObjectStatus)DataField.GetInt16(reader[17]);

         record.OrganizationName = DataField.GetString(reader[18]);
         record.AgentAlias = DataField.GetString(reader[19]);
         record.AgentEmail = DataField.GetString(reader[20]);
         record.Email = DataField.GetString(reader[21]);

         return record;
      }

      /// <summary>
      /// Prepare a list with data supplied in given reader.
      /// </summary>
      /// <param name="list">list to add items too</param>
      /// <param name="reader">reader (source of data)</param>
      /// <returns>instance List of NoteInfo'es</returns>
      public static List<ActivityFollowUpScheduleInfo> GetList(
         List<ActivityFollowUpScheduleInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ActivityFollowUpScheduleInfo entity;
         if (list == null)
            list = new List<ActivityFollowUpScheduleInfo>();
         while (reader.Read())
         {
            entity = new ActivityFollowUpScheduleInfo();
            ReadData(entity, reader);
            list.Add(entity);
         }
         return list;
      }
#endif

      #endregion
      #region -- 4.00 FollowUp Notes support

      /// <summary>
      /// Prepare Note for Follow-Up item...
      /// </summary>
      /// <param name="note">note</param>
      /// <param name="type">(optional) note type</param>
      /// <param name="referenceDate">(optional) reference date</param>
      /// <returns>instance of NoteInfo is returned</returns>
      public Notes.NoteInfo PrepareNote(String note, 
         NoteType type = NoteType.FollowUpNote, DateTime? referenceDate = null)
      {
         NoteInfo n = new NoteInfo();
         n.OrganizationId = OrganizationId;
         n.ReferenceId = FollowUpId;
         n.ReferenceDate = referenceDate.HasValue ? referenceDate.Value :
            DateTime.Now;
         n.Type = type;
         n.NoteText = note;
         n.Alias = "FollowUp";
         n.Alias += String.IsNullOrWhiteSpace(Alias) ? " (" + Alias + ")" :
            " Note...";
         return n;
      }

      #endregion

   }

}

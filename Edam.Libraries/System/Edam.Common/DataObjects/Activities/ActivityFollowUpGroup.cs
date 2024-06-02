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

   public class ActivityFollowUpGroupInfo
   {

      #region -- 1.00 Properties and Fields

      public String OrganizationId { get; set; }

      /// <summary>
      /// The Agent, person or entity doing the Follow-Up...
      /// </summary>
      public String AgentId { get; set; }

      /// <summary>
      /// Last related Entity involved in the follow-Up (eg. Employer)
      /// </summary> 
      public ReferenceBaseType EntityType { get; set; }
      public String EntityId { get; set; }
      public String EntityAlias { get; set; }

      /// <summary>
      /// Reference Id = source of follow-up (eg. Student Certification Program;
      /// DMV Inspection)
      /// </summary>
      public ReferenceBaseType ReferenceType { get; set; }
      public String ReferenceId { get; set; }

      /// <summary>
      /// Entity being Followed-Up (eg. Student); Entity, Organization, 
      /// Person Id to Follow-Up
      /// </summary>
      public String FollowedEntityId { get; set; }

      public DateTime? ServiceStartTime { get; set; }
      public DateTime? ServiceEndTime { get; set; }

      public DateTime? ReferenceDate { get; set; }

      public String FollowUpId { get; set; }
      public String Alias { get; set; }

      public ObjectStatus Status { get; set; }
      public EntityCompensationType CompensationType { get; set; }
      public Decimal AmountDefault { get; set; }

      #endregion
      #region -- 1.50 Initialize Object

      public ActivityFollowUpGroupInfo()
      {
         ClearFields();
      }

      #endregion
      #region -- 4.00 Support Methods

      public void ClearFields()
      {
         OrganizationId = String.Empty;
         AgentId = String.Empty;
         EntityType = ReferenceBaseType.Unknown;
         EntityId = String.Empty;
         EntityAlias = String.Empty;
         ReferenceType = ReferenceBaseType.Unknown;
         ReferenceId = String.Empty;
         FollowedEntityId = String.Empty;
         ServiceStartTime = null;
         ServiceEndTime = null;
         ReferenceDate = null;
         FollowUpId = String.Empty;
         Alias = String.Empty;
         Status = ObjectStatus.Unknown;
         CompensationType = EntityCompensationType.Unknown;
         AmountDefault = 0;
      }

      #endregion

#if DATA_SUPPORT_

#region -- 4.00 Data Reader Support Methods

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ActivityFollowUpGroupInfo ReadData(
         ActivityFollowUpGroupInfo record, System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ActivityFollowUpGroupInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.OrganizationId = DataField.GetString(reader[0]);
         record.AgentId = DataField.GetString(reader[1]);
         record.EntityType =
            (References.ReferenceBaseType)DataField.GetInt16(reader[2]);
         record.EntityId = DataField.GetString(reader[3]);
         record.EntityAlias = DataField.GetString(reader[4]);
         record.ReferenceType =
            (References.ReferenceBaseType)DataField.GetInt16(reader[5]);
         record.ReferenceId = DataField.GetString(reader[6]);
         record.FollowedEntityId = DataField.GetString(reader[7]);
         record.ServiceStartTime = DataField.GetNullableDateTime(reader[8]);
         record.ServiceEndTime = DataField.GetNullableDateTime(reader[9]);
         record.ReferenceDate = DataField.GetNullableDateTime(reader[10]);
         record.FollowUpId = DataField.GetString(reader[11]);
         record.Alias = DataField.GetString(reader[12]);
         record.Status = (ObjectStatus) DataField.GetInt16(reader[13]);
         record.CompensationType = (EntityCompensationType)
            DataField.GetInt16(reader[14]);
         record.AmountDefault = DataField.GetDecimal(reader[15]);

         return record;
      }

      /// <summary>
      /// Prepare a list with data supplied in given reader.
      /// </summary>
      /// <param name="list">list to add items too</param>
      /// <param name="reader">reader (source of data)</param>
      /// <returns>instance List of NoteInfo'es</returns>
      public static List<ActivityFollowUpGroupInfo> GetList(
         List<ActivityFollowUpGroupInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ActivityFollowUpGroupInfo entity;
         if (list == null)
            list = new List<ActivityFollowUpGroupInfo>();
         while (reader.Read())
         {
            entity = new ActivityFollowUpGroupInfo();
            ReadData(entity, reader);
            list.Add(entity);
         }
         return list;
      }

#endregion

#endif

   }

}

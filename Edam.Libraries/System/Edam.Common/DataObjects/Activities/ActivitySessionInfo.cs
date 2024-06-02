using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data;

namespace Edam.DataObjects.Activities
{

   public class ActivitySessionInfo
   {

      #region -- 1.00 Properties and Fields

      public DateTime CreatedDate { get; set; }
      public String OrganizationId { get; set; }
      public String ProgramId { get; set; }
      public String ThreadId { get; set; }
      public String SessionId { get; set; }
      public String IdentifierId { get; set; }
      public Edam.DataObjects.Objects.ObjectStatus Status { get; set; }
      public String Alias { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
      public Decimal UnitHours { get; set; }
      public String GlAccountId { get; set; }
      public Decimal FeeAmount { get; set; }
      public Int16 Capacity { get; set; }
      public String LocationId { get; set; }
      public String GroupId { get; set; }
      public String ContentText { get; set; }

      #endregion

      public ActivitySessionInfo()
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
         Status = Objects.ObjectStatus.Unknown;
         Alias = String.Empty;
         StartDate = Edam.NullDateTime.Value;
         EndDate = Edam.NullDateTime.Value;
         UnitHours = new Decimal(0.0);
         GlAccountId = String.Empty;
         FeeAmount = new Decimal(0.0);
         Capacity = 0;
         LocationId = String.Empty;
         GroupId = String.Empty;
         ContentText = String.Empty;
         ProgramId = String.Empty;
         SessionId = String.Empty;
      }

      public void Copy(ActivitySessionInfo session)
      {
         CreatedDate = session.CreatedDate;
         OrganizationId = session.OrganizationId;
         ThreadId = session.ThreadId;
         IdentifierId = session.IdentifierId;
         Status = session.Status;
         Alias = session.Alias;
         StartDate = session.StartDate;
         EndDate = session.EndDate;
         UnitHours = session.UnitHours;
         GlAccountId = session.GlAccountId;
         FeeAmount = session.FeeAmount;
         Capacity = session.Capacity;
         LocationId = session.LocationId;
         GroupId = session.GroupId;
         ContentText = session.ContentText;
         ProgramId = session.ProgramId;
         SessionId = session.SessionId;
      }

      public static void FixNullValues(ActivitySessionInfo record)
      {
         record.OrganizationId =
            Edam.Convert.ToNotNullString(record.OrganizationId);
         record.ProgramId =
            Edam.Convert.ToNotNullString(record.ProgramId);
         record.ThreadId =
            Edam.Convert.ToNotNullString(record.ThreadId);
         record.SessionId =
            Edam.Convert.ToNotNullString(record.SessionId);
         record.IdentifierId =
            Edam.Convert.ToNotNullString(record.IdentifierId);
         record.Alias =
            Edam.Convert.ToNotNullString(record.Alias);
         record.GlAccountId =
            Edam.Convert.ToNotNullString(record.GlAccountId);
         record.GroupId =
            Edam.Convert.ToNotNullString(record.GroupId);
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ActivitySessionInfo ReadData(
         ActivitySessionInfo record,
         System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ActivitySessionInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.CreatedDate = DataField.GetDateTime(reader[0]);
         record.OrganizationId = DataField.GetString(reader[1]);
         record.ProgramId = DataField.GetString(reader[2]);
         record.ThreadId = DataField.GetString(reader[3]);
         record.SessionId = DataField.GetString(reader[4]);
         record.IdentifierId = DataField.GetString(reader[5]);
         record.Alias = DataField.GetString(reader[6]);
         record.Status = (Objects.ObjectStatus)DataField.GetInt16(reader[7]);
         record.UnitHours = DataField.GetDecimal(reader[8]);
         record.StartDate = DataField.GetDateTime(reader[9]);
         record.EndDate = DataField.GetDateTime(reader[10]);
         record.GlAccountId = DataField.GetString(reader[11]);
         record.FeeAmount = DataField.GetDecimal(reader[12]);
         record.Capacity = DataField.GetInt16(reader[13]);
         record.LocationId = DataField.GetString(reader[14]);
         record.GroupId = DataField.GetString(reader[15]);
         record.ContentText = DataField.GetString(reader[16]);

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
      /// <returns>instance List of ActivityProgramThreadSessionInfo'es</returns>
      public static List<ActivitySessionInfo> GetList(
         List<ActivitySessionInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ActivitySessionInfo record;
         if (list == null)
            list = new List<ActivitySessionInfo>();
         while (reader.Read())
         {
            record = new ActivitySessionInfo();
            record.ReadData(reader);
            list.Add(record);
         }
         return list;
      }

#endif

   }

}

using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using ReqResp = Edam.DataObjects.Requests;
using Edam.Data;
using Edam.Diagnostics;

namespace Edam.DataObjects.Activities
{

   public class ActivityProgramScheduleInfo
   {

      #region -- 1.00 Properties and Fields

      public String OrganizationId { get; set; }
      public String ReferenceId { get; set; }
      public String ItemId { get; set; }
      public String LocationId { get; set; }
      public DateTime ActivityDate { get; set; }
      public String CityName { get; set; }
      public String StateCode { get; set; }
      public String LocationAlias { get; set; }
      public String Title { get; set; }
      public String Description { get; set; }
      public Int32 Participants { get; set; }
      public Int32 Available { get; set; }

      #endregion

      public ActivityProgramScheduleInfo()
      {
         ClearFields();
      }

      /// <summary>
      /// Clear Fields...
      /// </summary>
      public void ClearFields()
      {
         OrganizationId = String.Empty;
         ReferenceId = String.Empty;
         ItemId = String.Empty;
         LocationId = String.Empty;
         ActivityDate = Edam.NullDateTime.Value;
         CityName = String.Empty;
         StateCode = String.Empty;
         LocationAlias = String.Empty;
         Title = String.Empty;
         Description = String.Empty;
         Participants = 0;
         Available = 0;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ActivityProgramScheduleInfo ReadData(
         ActivityProgramScheduleInfo record, 
         System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ActivityProgramScheduleInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.OrganizationId = DataField.GetString(reader[0]);
         record.ReferenceId = DataField.GetString(reader[1]);
         record.ItemId = DataField.GetString(reader[2]);
         record.LocationId = DataField.GetString(reader[3]);
         record.ActivityDate = DataField.GetDateTime(reader[4]);
         record.CityName = DataField.GetString(reader[5]);
         record.StateCode = DataField.GetString(reader[6]);
         record.LocationAlias = DataField.GetString(reader[7]);
         record.Title = DataField.GetString(reader[8]);
         record.Description = DataField.GetString(reader[9]);
         record.Participants = DataField.GetInt32(reader[10]);
         record.Available = DataField.GetInt32(reader[11]);

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
      /// <returns>instance List of Activiti'es</returns>
      public static List<ActivityProgramScheduleInfo> GetList(
         List<ActivityProgramScheduleInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ActivityProgramScheduleInfo entity;
         if (list == null)
            list = new List<ActivityProgramScheduleInfo>();
         while(reader.Read())
         {
            entity = new ActivityProgramScheduleInfo();
            entity.ReadData(reader);
            list.Add(entity);
         }
         return list;
      }

#endif

   }

}

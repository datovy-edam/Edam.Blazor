using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data;

namespace Edam.DataObjects.Activities
{

   public class ActivityNotificationInfo
   {

      public DateTime? CreatedDate { get; set; }
      public String Title { get; set; }
      public String OrganizationId { get; set; }
      public String OrganizationName { get; set; }
      public DateTime? StartDate { get; set; }
      public Int32? Duration { get; set; }
      public String DurationTypeText { get; set; }
      public DateTime? ExpirationDate { get; set; }
      public String Description { get; set; }
      public String ContactName { get; set; }
      public String PhoneNumber { get; set; }
      public String Email { get; set; }
      public String OtherContactId { get; set; }
      public String OtherContactTypeText { get; set; }
      public String LocationText { get; set; }
      public String LocationTypeText { get; set; }
      public String UrlWebSite { get; set; }
      public Int32? Capacity { get; set; }
      public String CapacityScopeText { get; set; }
      public Int32? TotalRegistered { get; set; }
      public String ActivityId { get; set; }
      public String WebSiteUrl { get; set; }

      public ActivityNotificationInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         CreatedDate = null;
         Title = String.Empty;
         OrganizationId = String.Empty;
         OrganizationName = String.Empty;
         StartDate = null;
         Duration = null;
         DurationTypeText = String.Empty;
         ExpirationDate = null;
         Description = String.Empty;
         ContactName = String.Empty;
         PhoneNumber = String.Empty;
         Email = String.Empty;
         OtherContactId = String.Empty;
         OtherContactTypeText = String.Empty;
         LocationText = String.Empty;
         LocationTypeText = String.Empty;
         UrlWebSite = String.Empty;
         Capacity = null;
         CapacityScopeText = String.Empty;
         TotalRegistered = null;
         ActivityId = String.Empty;
         WebSiteUrl = String.Empty;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ActivityNotificationInfo ReadData(
         ActivityNotificationInfo record,
         System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ActivityNotificationInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.CreatedDate = DataField.GetDateTime(reader[0]);
         record.Title = DataField.GetString(reader[1]);
         record.OrganizationId = DataField.GetString(reader[2]);
         record.OrganizationName = DataField.GetString(reader[3]);
         record.StartDate = DataField.GetDateTime(reader[4]);
         record.Duration = DataField.GetInt32(reader[5]);
         record.DurationTypeText = DataField.GetString(reader[6]);
         record.ExpirationDate = DataField.GetDateTime(reader[7]);
         record.Description = DataField.GetString(reader[8]);
         record.ContactName = DataField.GetString(reader[9]);
         record.PhoneNumber = DataField.GetString(reader[10]);
         record.Email = DataField.GetString(reader[11]);
         record.OtherContactId = DataField.GetString(reader[12]);
         record.OtherContactTypeText = DataField.GetString(reader[13]);
         record.LocationText = DataField.GetString(reader[14]);
         record.LocationTypeText = DataField.GetString(reader[15]);
         record.UrlWebSite = DataField.GetString(reader[16]);
         record.Capacity = DataField.GetInt32(reader[17]);
         record.CapacityScopeText = DataField.GetString(reader[18]);
         record.TotalRegistered = DataField.GetInt32(reader[19]);
         record.ActivityId = DataField.GetString(reader[20]);
         record.WebSiteUrl = DataField.GetString(reader[21]);

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
      public static List<ActivityNotificationInfo> GetList(
         List<ActivityNotificationInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ActivityNotificationInfo item;
         if (list == null)
            list = new List<ActivityNotificationInfo>();
         while (reader.Read())
         {
            item = new ActivityNotificationInfo();
            ActivityNotificationInfo.ReadData(item, reader);
            list.Add(item);
         }
         return list;
      }

#endif

   }

}

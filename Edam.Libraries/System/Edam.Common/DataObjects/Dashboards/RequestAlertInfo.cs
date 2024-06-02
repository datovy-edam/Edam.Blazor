using System;
//using System.Data.Common;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data;
using Edam.DataObjects.References;
using resource = Edam.Application.ApplicationHelper;

//using DbField = Edam.Data.DataField;

namespace Edam.DataObjects.Dashboards
{

   public class RequestAlertInfo
   {
      public String YearMonthText { get; set; }
      public Int32 DateYear { get; set; }
      public Int32 DateMonth { get; set; }
      public Int32 Elaps { get; set; }
      public String OrganizationId { get; set; }
      public Int16 RequestTypeNo { get; set; }
      public String RequestDescription { get; set; }
      public Int16 StatusNo { get; set; }
      public String StatusDescription { get; set; }
      public DateTime? MinDate { get; set; }
      public DateTime? MaxDate { get; set; }
      public Int32 RequestCount { get; set; }

      public RequestAlertInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         YearMonthText = String.Empty;
         DateYear = 0;
         DateMonth = 0;
         Elaps = 0;
         OrganizationId = Edam.Application.Session.OrganizationId;
         RequestTypeNo = (Int16)ReferenceListGroup.NewProspect;
         RequestDescription = resource.GetString("ListGroupProspect");
         StatusNo = 0;
         StatusDescription = String.Empty;
         MinDate = null;
         MaxDate = null;
         RequestCount = 0;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read Data...
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <returns>instance of OrganizationInfo is returned</returns>
      public static RequestAlertInfo ReadData(DbDataReader reader)
      {
         RequestAlertInfo r = new RequestAlertInfo();

         r.YearMonthText = DbField.GetString(reader[0]);
         r.DateYear = DbField.GetInt32(reader[1]);
         r.DateMonth = DbField.GetInt32(reader[2]);
         r.Elaps = DbField.GetInt32(reader[3]);
         r.OrganizationId = DbField.GetString(reader[4]);
         r.RequestTypeNo = DbField.GetInt16(reader[5]);
         r.RequestDescription = DbField.GetString(reader[6]);
         r.StatusNo = DbField.GetInt16(reader[7]);
         r.StatusDescription = DbField.GetString(reader[8]);
         r.MinDate = DbField.GetDateTime(reader[9]);
         r.MaxDate = DbField.GetDateTime(reader[10]);
         r.RequestCount = DbField.GetInt32(reader[11]);

         return r;
      }

      /// <summary>
      /// Read list.
      /// </summary>
      /// <param name="reader"></param>
      /// <returns>list of OrganizationInfo is returned</returns>
      public static List<RequestAlertInfo> ReadList(DbDataReader reader)
      {
         List<RequestAlertInfo> l = new List<RequestAlertInfo>();
         RequestAlertInfo o;
         while (reader.Read())
         {
            o = ReadData(reader);
            l.Add(o);
         }
         return l;
      }

#endif

   }

}

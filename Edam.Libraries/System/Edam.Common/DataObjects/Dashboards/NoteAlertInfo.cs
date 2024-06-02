using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data;

namespace Edam.DataObjects.Dashboards
{

   public class NoteAlertInfo
   {
      public String Title { get; set; }
      public String YearMonthText { get; set; }
      public Int32 DateYear { get; set; }
      public Int32 DateMonth { get; set; }
      public Int32 Elaps { get; set; }
      public Int16 StatusNo { get; set; }
      public String StatusDescription { get; set; }
      public Int32 NotesCount { get; set; }

      public NoteAlertInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         Title = String.Empty;
         YearMonthText = String.Empty;
         DateYear = 0;
         DateMonth = 0;
         Elaps = 0;
         StatusNo = 0;
         StatusDescription = String.Empty;
         NotesCount = 0;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read Data...
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <returns>instance of OrganizationInfo is returned</returns>
      public static NoteAlertInfo ReadData(DbDataReader reader)
      {
         NoteAlertInfo r = new NoteAlertInfo();

         r.Title = DbField.GetString(reader[0]);
         r.YearMonthText = DbField.GetString(reader[1]);
         r.DateYear = DbField.GetInt32(reader[2]);
         r.DateMonth = DbField.GetInt32(reader[3]);
         r.Elaps = DbField.GetInt32(reader[4]);
         r.StatusNo = DbField.GetInt16(reader[5]);
         r.StatusDescription = DbField.GetString(reader[6]);
         r.NotesCount = DbField.GetInt32(reader[7]);

         return r;
      }

      /// <summary>
      /// Read list.
      /// </summary>
      /// <param name="reader"></param>
      /// <returns>list of OrganizationInfo is returned</returns>
      public static List<NoteAlertInfo> ReadList(DbDataReader reader)
      {
         List<NoteAlertInfo> l = new List<NoteAlertInfo>();
         NoteAlertInfo o;
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

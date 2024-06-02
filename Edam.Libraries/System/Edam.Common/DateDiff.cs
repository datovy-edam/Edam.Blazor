using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using resource = Edam.Application.ApplicationHelper;

namespace Edam
{

   public class DateDiff
   {

      public static readonly double DAYS_IN_YEAR = 365.2425;
      public static readonly double DAYS_IN_MONTH = 30.42;

      private TimeSpan m_Span;

      public void Initialize(DateTime start, DateTime end)
      {
         if (start < end)
         {
            DateTime t = start;
            start = end;
            end = t;
         }
         m_Span = start - end;
      }

      public DateDiff(DateTime start, DateTime end)
      {
         Initialize(start, end);
      }

      public String HowOldText()
      {
         if (m_Span.TotalDays <= 0.0)
            return resource.GetString("HowOldRecent");
         double tyears = m_Span.TotalDays / DAYS_IN_YEAR;
         Int32 iyears = (Int32)tyears;
         if (tyears > 2.0)
            return iyears.ToString() + " " + resource.GetString("HowOldYears");
         if (tyears > 1.0)
            return resource.GetString("HowOldYear");
         double tmonths = ((Int32)m_Span.TotalDays) / DAYS_IN_MONTH;
         Int32 imonths = (Int32)tmonths;
         if (tmonths > 2.0)
            return imonths.ToString() + " " + 
               resource.GetString("HowOldMonths");
         if (tmonths > 1.0)
            return resource.GetString("HowOldMonth");
         if (m_Span.TotalDays > 1.0)
            return ((Int32)m_Span.TotalDays).ToString() + " " + 
               resource.GetString("HowOldDays");
         if (m_Span.TotalDays > 0.0)
            return "1 " + resource.GetString("HowOldDays");
         return resource.GetString("HowOldRecent");
      }

   }

}

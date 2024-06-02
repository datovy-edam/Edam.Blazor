using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.References
{

   public class ReferencePeriodDate
   {
      public static readonly Int32 DAYS_IN_WEEK = 7;

      private DateTimeFormatInfo m_DateInfo = DateTimeFormatInfo.CurrentInfo;
      private Calendar m_Calendar;

      private DateTime m_ReferenceDate;

      public ReferencePeriod Period { get; set; }
      public String DateText
      {
         get
         {
            return m_ReferenceDate.ToString(
               Edam.Application.Resources.Strings.DefaultDateYyyyMmDdFormat);
         }
      }

      public DateTime ReferenceDate
      {
         get { return m_ReferenceDate; }
         set { m_ReferenceDate = value; }
      }

      public String YearText
      {
         get { return m_ReferenceDate.Year.ToString(); }
      }

      public Int32 Year
      {
         get { return m_ReferenceDate.Year; }
      }

      public String WeekText
      {
         get { return GetWeek(m_ReferenceDate).ToString(); }
      }
      public String WeekFormattedText
      {
         get
         {
            return GetWeek(m_ReferenceDate).ToString("00");
         }
      }

      public Int32 Week
      {
         get { return GetWeek(m_ReferenceDate); }
      }

      private String m_PeriodText;

      public String PeriodText
      {
         get
         {
            switch(Period)
            {
               case ReferencePeriod.Week:
                  m_PeriodText = YearText + " Week " + WeekFormattedText;
                  break;
               default:
                  m_PeriodText = DateText;
                  break;
            }
            return m_PeriodText;
         }
      }

      public String PeriodId
      {
         get
         {
            switch (Period)
            {
               case ReferencePeriod.Week:
                  m_PeriodText = YearText + "-" + WeekFormattedText;
                  break;
               default:
                  m_PeriodText = DateText;
                  break;
            }
            return m_PeriodText;
         }
      }

      public ReferencePeriodDate()
      {
         Period = ReferencePeriod.Week;
         m_Calendar = m_DateInfo.Calendar;
         Now();
      }

      public Int32 GetWeek(DateTime date)
      {
         var week = m_Calendar.GetWeekOfYear(date,
            m_DateInfo.CalendarWeekRule, m_DateInfo.FirstDayOfWeek);
         return week;
      }

      public void NextWeek()
      {
         m_ReferenceDate = m_ReferenceDate.AddDays(DAYS_IN_WEEK);
      }

      public void PreviousWeek()
      {
         m_ReferenceDate = m_ReferenceDate.AddDays(-DAYS_IN_WEEK);
      }

      public void Now()
      {
         m_ReferenceDate = DateTime.Now;
      }

      public ReferencePeriod Toggle()
      {
         switch(Period)
         {
            case ReferencePeriod.Week:
               Period = ReferencePeriod.Day;
               break;
            default:
               Period = ReferencePeriod.Week;
               break;
         }
         return Period;
      }

      public static String GetReferencePeriodId(
         ReferencePeriodInfo reference = null)
      {
         String id = String.Empty;
         if (reference == null)
            reference = new ReferencePeriodInfo();
         if (reference.Period == ReferencePeriod.Unknown)
            reference.Period = ReferencePeriod.Week;
         switch(reference.Period)
         {
            case ReferencePeriod.Week:
               id = reference.PeriodDate.YearText + "-" +
                  reference.PeriodDate.WeekText;
               break;
         }
         return id;
      }

   }

}

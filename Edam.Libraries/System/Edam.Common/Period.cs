using System;

// -----------------------------------------------------------------------------

namespace Edam
{

   public class Period
   {

      private TimeSpan? m_Duration;
      public DateTime? Start { get; set; }
      public DateTime? End { get; set; }

      public TimeSpan? Duration
      {
         get
         {
            m_Duration = ToDuration(Start, End);
            return m_Duration;
         }
         set
         {
            m_Duration = value;
            if (value.HasValue)
            {
               End = Start.Value.AddMilliseconds(
                  m_Duration.Value.TotalMilliseconds);
            }
         }
      }

      public void Initialize(DateTime? start, DateTime? end)
      {
         if (!start.HasValue)
            start = end;
         if (!start.HasValue)
         {
            Start = DateTime.UtcNow.AddDays(1);
            End = Start.Value.AddHours(1);
         }

         if (start.Value > end.Value)
         {
            DateTime t = start.Value;
            start = end.Value;
            end = t;
         }

         Start = start.Value;
         End = end.Value;
         m_Duration = ToDuration(Start, End);
      }

      public Period(DateTime? start, DateTime? end)
      {
         Initialize(start, end);
      }

      /// <summary>
      /// Given a start and end date-time calculate duration, else use given default,
      /// else one hour duration will be assumed.
      /// </summary>
      /// <param name="start">start date-time</param>
      /// <param name="end">end date-time</param>
      /// <param name="defaultDuration">(optional) duration</param>
      /// <returns>Duration timespan is returned</returns>
      public static TimeSpan? ToDuration(DateTime? start, DateTime? end,
         TimeSpan? defaultDuration = null)
      {
         TimeSpan? d = new TimeSpan(1, 0, 0);
         if (start.HasValue && end.HasValue)
         {
            if (start.Value > end.Value)
            {
               DateTime t = start.Value;
               start = end.Value;
               end = t;
            }
            d = end.Value - start.Value;
         }
         else if (defaultDuration.HasValue)
         {
            d = defaultDuration.Value;
         }
         return d;
      }

   }

}

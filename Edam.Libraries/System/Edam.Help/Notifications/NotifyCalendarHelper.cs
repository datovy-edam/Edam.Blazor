using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using ReqResp = Edam.DataObjects.Requests;
using Activity = Edam.DataObjects.Activities;
//using Edam.DataObjects.Notifications;
//using Edam.Net.Calendar;
using Edam.DataObjects;
using Resource = Edam.Application.Resources;

namespace Edam.Help.Notifications
{
   /*
   public class NotifyCalendarHelper
   {

      #region -- 4.0 Notify iCalendar Event

      /// <summary>
      /// Given an Activity Follow-Up item, build an iCalendar appointment...
      /// </summary>
      /// <param name="item">Activity Follow-Up item</param>
      /// <returns>An iCalendar appointment is returned</returns>
      public static cal.CalendarInfo ToiCalendar(
         Activity.ActivityFollowUpDetailsInfo item)
      {
         CalendarInfo ical = new CalendarInfo();
         Edam.Period p = new Period(item.ServiceStartTime, item.ServiceEndTime);

         // prepare description...
         String messageFrom = String.IsNullOrWhiteSpace(item.OrganizationName) ?
            String.Empty : String.Format(
               Edam.Application.Resources.ApplicationStrings.MessageFrom,
               item.OrganizationName);
         String aDescript = Edam.Application.Resources.
            ApplicationStrings.ActivityFollowUpDescription;
         Boolean hasMessageFrom = String.IsNullOrWhiteSpace(messageFrom);
         String descript = hasMessageFrom ?
            aDescript : messageFrom + Resource.Strings.Html.TagBreak + aDescript;

         // get RSVP...
         Boolean rsvp = (Edam.Application.Resources.Strings.True.ToLower() ==
            Resource.ConfigResource.ActivityFollowUpDefaultRsvp);

         // build calendar appointment...
         ical.CreatedDate = DateTime.UtcNow;
         ical.AddAttendee(item.AgentAlias, item.AgentEmail);
         ical.AddAttendee(item.Alias, item.Email);
         ical.Description = descript;
         ical.Duration = p.Duration;
         ical.EndDate = p.End;
         ical.HtmlFormatted = String.IsNullOrWhiteSpace(messageFrom);
         ical.OrganizerEmail = item.AgentEmail;
         ical.RequireRsvp = rsvp;
         ical.StartDate = p.Start;

         ical.Summary = Resource.ApplicationStrings.ActivityFollowUpSummary;
         ical.Venue = Resource.ApplicationStrings.ActivityFollowUpPhoneVenue;

         return ical;
      }

      #endregion

   }
   */
}

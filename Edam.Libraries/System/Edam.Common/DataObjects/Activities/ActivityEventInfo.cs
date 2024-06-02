using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.References;

namespace Edam.DataObjects.Activities
{

   public class ActivityEventInfo
   {

      public String OrganizationId { get; set; }
      public string ActivityId { get; set; }
      public string EventId { get; set; }
      public string EventName { get; set; }

      public ReferencePeriodDate PeriodDate { get; } 
         = new ReferencePeriodDate();
      public ActivityRatings Evaluation { get; } = new ActivityRatings();

      public List<ActivityRatingInfo> Ratings
      {
         get { return Evaluation.Ratings; }
      }

      public List<ActivityRatingParticipantInfo> Participants
      {
         get { return Evaluation.RatingParticipants; }
      }

      public ActivityEventInfo()
      {
         Evaluation.Ratings = new List<ActivityRatingInfo>();
         Evaluation.RatingParticipants = 
            new List<ActivityRatingParticipantInfo>();
      }

   }

}

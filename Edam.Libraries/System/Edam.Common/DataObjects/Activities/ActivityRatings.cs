using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Activities
{

   public class ActivityRatings
   {

      public List<ActivityRatingInfo> Ratings { get; set; }
      public List<ActivityRatingParticipantInfo>
         RatingParticipants { get; set; }

   }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.DataObjects.DataCodes;

namespace Edam.DataObjects.Activities
{

   public class ActivityPeriodRatingCodes
   {

      public List<DataCodeInfo> ObjectStatus { get; set; }
      public List<DataCodeInfo> ActivityRatingType { get; set; }
      public List<DataCodeInfo> ActivityParticipationResultsType { get; set; }
      public List<DataCodeInfo> ParticipantRoleType { get; set; }
      public List<ActivityPeriodRatingInfo> ActivityRatings { get; set; }

   }

}

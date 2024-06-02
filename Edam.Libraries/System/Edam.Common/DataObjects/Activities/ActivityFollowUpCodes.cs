using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Entities;
using Edam.DataObjects.DataCodes;

namespace Edam.DataObjects.Activities
{

   public class ActivityFollowUpCodes
   {
      public List<EntityContactInfo> Agents { get; set; }
      public EntityContactInfo FollowedEntity { get; set; }
      public List<IDataCode> CompensationTypes { get; set; }
      public List<IDataCode> StatusTypes { get; set; }
      public List<ActivityFollowUpGroupInfo> Groups { get; set; }
      public List<ActivityFollowUpScheduleInfo> Schedule { get; set; }
   }

}

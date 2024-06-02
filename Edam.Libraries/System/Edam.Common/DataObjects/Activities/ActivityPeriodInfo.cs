using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.References;
using Edam.DataObjects.Entities;
using Edam.DataObjects.Locations;

namespace Edam.DataObjects.Activities
{

   public class ActivityPeriodInfo : ReferencePeriodDate
   {
      
      public String PeriodLabel { get; set; }
      public List<LocationInfo> Locations { get; set; }
      public List<ParticipantInfo> GroupMembers { get; set; }
      public List<ActivityPeriodParticipantInfo> Participants { get; set; }

   }

}

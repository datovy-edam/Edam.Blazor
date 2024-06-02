using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.References;
using Edam.DataObjects.Locations;
using Edam.DataObjects.Entities;

namespace Edam.DataObjects.Activities
{

   public class ActivityPeriodParticipantInfo
   {

      public String GroupId { get; set; }
      public String MemberId { get; set; }

      public LocationInfo Location { get; set; }
      public ParticipantInfo Participant { get; set; }

      public ReferencePeriodDate Date { get; set; }

   }

}

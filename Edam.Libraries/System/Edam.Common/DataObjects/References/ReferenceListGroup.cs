using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.References
{

   public enum ReferenceListGroup
   {
      Find = -1,
      Unknown = 0,
      MatchAny = 1,
      NewProspect = 10,
      QualifyForAid = 20,
      InSponsorProcess = 30,
      PendingSponsorApproval = 40,
      SelfCash = 50,
      StartSoon = 60,
      InProcess = 70,
      FinishSoon = 80,
      MustCall = 90,
      Archive = 100,
      Purge = 990
   }

}

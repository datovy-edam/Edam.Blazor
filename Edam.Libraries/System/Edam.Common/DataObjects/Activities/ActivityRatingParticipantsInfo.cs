using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data;

namespace Edam.DataObjects.Activities
{

   public class ActivityRatingParticipantsInfo
   {

      public Documents.DocumentSignatureInfo DocumentSignature { get; set; }
      public ActivityRatingInfo Rating { get; set; }
      public List<ActivityRatingParticipantInfo> Participants { get; set; }

   }

}

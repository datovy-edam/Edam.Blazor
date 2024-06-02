using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Objects
{

   public enum ObjectState
   {

      Unknown = 0,
      Submitted = 1,
      PreApproved = 2,
      Registered = 3,
      Parsed = 4,
      Verified = 5,
      Approved = 6,
      Rejected = 7,
      InEvaluation = 8,
      InInvestigation = 9,
      InArchive = 10,
      InLegal = 11,
      ToEvaluate = 12,
      ToInvestigation = 13,
      ToArchive = 14,
      ToLegal = 15,
      ToRemove = 16,
      FailedWithErrors = 17,
      Completed = 18

   }

}

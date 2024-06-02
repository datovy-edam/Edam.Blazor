using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.DataObjects.DataCodes;

namespace Edam.DataObjects.Activities
{

   public class ActivitySessionCodes
   {

      public List<IDataCode> LocationCodes { get; set; }
      public List<IDataCode> ContentCodes { get; set; }
      public List<ActivitySessionInfo> Sessions { get; set; }

   }

}

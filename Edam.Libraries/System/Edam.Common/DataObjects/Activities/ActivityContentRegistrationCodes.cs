using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Activities
{

   public class ActivityContentRegistrationCodes
   {

      public List<DataCodes.IDataCode> ObjectStatus { get; set; }
      public List<DataCodes.IDataCode> ObjectScopes { get; set; }
      public List<DataCodes.IDataCode> Applications { get; set; }

   }

}

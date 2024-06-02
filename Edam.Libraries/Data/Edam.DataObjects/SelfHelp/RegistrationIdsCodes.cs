using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data;
using Edam.Diagnostics;

namespace Edam.DataObjects.SelfHelp
{

   public class RegistrationIdsCodes
   {
      public List<DataCodes.DataCodeInfo> LocationCountries { get; set; }
      public List<Locations.LocationStateInfo> LocationStates { get; set; }
      public List<DataCodes.DataCodeInfo> Applications { get; set; }
      public List<DataCodes.DataCodeInfo> Organizations { get; set; }
   }

}

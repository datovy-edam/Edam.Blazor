using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Locations
{

   public class LocationInfo
   {

      public String LocationId { get; set; }
      public String Alias { get; set; }
      public String CityName { get; set; }
      public String StateCode { get; set; }

      public LocationInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         LocationId = String.Empty;
         Alias = String.Empty;
         CityName = String.Empty;
         StateCode = String.Empty;
      }

   }

}

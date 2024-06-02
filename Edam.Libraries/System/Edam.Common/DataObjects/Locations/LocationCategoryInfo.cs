using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Locations
{

   public class LocationCategoryInfo
   {
      public String CategoryId { get; set; }
      public String Description { get; set; }
      public LocationCategoryInfo()
      {
         ClearFields();
      }
      public void ClearFields()
      {
         CategoryId = String.Empty;
         Description = String.Empty;
      }
   }

}

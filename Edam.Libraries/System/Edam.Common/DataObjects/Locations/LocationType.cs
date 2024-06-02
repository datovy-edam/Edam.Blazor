using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Locations
{

   public enum LocationType
   {
      Unknown = 0,
      MailingAddress = 1,
      BillingAddress = 2,
      ActivityAddress = 3,
      ResidentialAddress = 4,
      WorkAddress = 5,
      SchoolAddress = 6,
      RelativeAdddress = 7,
      ReferenceAddress = 98,
      Other = 99
   }

}

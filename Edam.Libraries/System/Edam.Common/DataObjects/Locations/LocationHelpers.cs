using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Locations
{

   public class LocationHelpers
   {

      /// <summary>
      /// Validate, Set and return the location type text.
      /// </summary>
      /// <param name="type">Location Address</param>
      /// <param name="location">(optional) location if you like to set it up
      /// with found values</param>
      /// <returns>returns the location type as text</returns>
      public static String GetTypeText(LocationType type, 
         LocationAddressReferenceInfo location = null)
      {
         LocationType b;
         String t;
         LocationCategory c = LocationCategory.Unknown;
         switch (type)
         {
            case LocationType.MailingAddress:
               t = "Mailing Address";
               c = LocationCategory.PostOffice;
               b = LocationType.MailingAddress;
               break;
            case LocationType.BillingAddress:
               t = "Billing Address";
               c = LocationCategory.PostOffice;
               b = LocationType.BillingAddress;
               break;
            case LocationType.ActivityAddress:
               t = "Activity Address";
               c = LocationCategory.Other;
               b = LocationType.ActivityAddress;
               break;
            case LocationType.ResidentialAddress:
               t = "Residential Address";
               c = LocationCategory.Residencial;
               b = LocationType.ResidentialAddress;
               break;
            case LocationType.WorkAddress:
               t = "Work Address";
               c = LocationCategory.Office;
               b = LocationType.WorkAddress;
               break;
            case LocationType.SchoolAddress:
               t = "School Address";
               c = LocationCategory.School;
               b = LocationType.SchoolAddress;
               break;
            case LocationType.RelativeAdddress:
               t = "Relative Address";
               c = LocationCategory.Residencial;
               b = LocationType.RelativeAdddress;
               break;
            case LocationType.ReferenceAddress:
               t = "Reference Address";
               c = LocationCategory.Other;
               b = LocationType.ReferenceAddress;
               break;
            case LocationType.Other:
               t = "Other";
               c = LocationCategory.Other;
               b = LocationType.Other;
               break;
            default:
               t = "Not Specified";
               type = LocationType.Unknown;
               c = LocationCategory.Unknown;
               b = LocationType.Unknown;
               break;
         }
         if (location != null)
         {
            location.AssociationType = b;
            location.Type = type;
            location.LocationTypeText = t;
            if (location.Category == LocationCategory.Unknown)
               location.Category = c;
         }
         return t;
      }

   }

}

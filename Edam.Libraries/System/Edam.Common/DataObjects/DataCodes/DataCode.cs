using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.DataCodes
{

   public enum DataCode
   {
      Uknown = 0,
      OrganizationIdentifiers = 20,
      LocationTypes = 2,
      LocationCategories = 3,
      LocationStatus = 4,
      ReferenceTypes = 5,

      LocationReferences = 60,
      EntitieReferences = 61,
      ActivityProgramReferences = 62,
      ActivityThreadReferences = 63,
      ActivitySessionReferences = 64,
      LedgerReferences = 65,
      DocumentReferences = 66,
      References = 67,

      GroupReferences = 800

   }

}

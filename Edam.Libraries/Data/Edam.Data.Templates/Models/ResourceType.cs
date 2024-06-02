using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Models
{

   public enum ResourceType
   {
      Unknown = 0,
      Root = 1,

      Column = 10,
      Parameter = 11,
      Variable = 12,
      Constant = 13,
      OutputValue = 14,
      ReturnValue = 15,
      Section = 16,

      View = 20,
      Function = 21,
      StoredProcedure = 30,

      MapKeyValue = 51,
      MapLink = 52,
      MapLinkKeyValue = 53,

      Presentation = 80,
      PresentationContent = 81,
      PresentationGroup = 82,
      PresentationRow = 83,
      PresentationColumn = 84,
      PresentationStep = 85,

      TemplateGroup = 90,
      Group = 91,
      Dynamic = 100
   }

}

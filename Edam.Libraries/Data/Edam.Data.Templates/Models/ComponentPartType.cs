using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Models
{

   public enum ComponentPartType
   {
      Unknown = 0,
      DivisionInput = 1,
      DivisionSelect = 2,
      Form = 3,
      Row = 4,

      CodeListDeclaration = 10,
      CodeListInitialization = 11,

      // Code layout:
      // - Declarations
      // - Component
      //   * Form-Control list
      CodeDeclaration = 20,
      CodeFormControl = 21,
      CodeComponent = 22
   }

}

using System;
using System.Collections.Generic;
using System.Text;

namespace Edam.Json.Jsd
{
   public enum JsdSchemaItem
   {
      Unknown = 0,
      Property = 1,
      ProlertyInline = 2,
      BlockOpen = 3,
      BlockClose = 4,
      ArrayOpen = 5,
      ArrayClose = 6,
      Reference = 7
   }
}

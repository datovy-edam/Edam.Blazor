using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Asset
{
   public enum DataElementKind
   {
      Unknown = 0,
      Property = 1,
      CodeSet = 2,
      ExternalReference = 3,
      AuditProperty = 10
   }
}

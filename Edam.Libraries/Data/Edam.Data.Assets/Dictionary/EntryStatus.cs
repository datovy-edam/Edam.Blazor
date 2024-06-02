using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Dictionary
{

   public enum EntryStatus
   {
      Unknown = 0,
      Added = 1,
      Found = 2,
      NotFound = 3,
      Removed = 4,
      Unavailable = 99
   }

}

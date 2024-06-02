using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Asset
{

   public enum ConstraintType
   {
      unknown = 0,
      nonkey = 1,
      key = 2,
      autoGenerate = 3,
      none = 4,
      undefined = 99
   }

}

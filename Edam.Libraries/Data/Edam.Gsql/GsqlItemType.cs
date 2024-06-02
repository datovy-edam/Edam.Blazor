using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Gsql
{

   public enum GsqlItemType
   {
      Unknown = 0,
      Graph = 1,

      Integer = 2,
      UnsignedInteger = 3,
      Float = 4,
      Double = 5,
      String = 6,
      Boolean = 7,
      Vertex = 8,
      Edge = 9,
      JsonObject = 10,
      JsonArray = 11,
      DateTime = 12
   }

}

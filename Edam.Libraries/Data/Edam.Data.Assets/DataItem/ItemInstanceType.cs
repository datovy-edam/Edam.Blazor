using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.DataItem
{

   public enum ItemInstanceType
   {
      None,
      Object,
      Array,
      Constructor,
      Property,
      Comment,
      Integer,
      Float,
      String,
      Boolean,
      Null,
      Undefined,
      Date,
      Raw,
      Bytes,
      Guid,
      Uri,
      TimeSpan
   }

}

using System;
using System.Collections.Generic;
using System.Text;

namespace Edam.Json.Jsd
{
   public enum JsdType
   {
      Unknown = 0,
      Supress = 1,

      Number = 2,
      Null = 3,
      Empty = 4,

      Boolean = 10,
      Byte = 11,
      Short = 12,
      Float = 13,
      Long = 14,
      Integer = 15,
      Decimal = 16,
      Int = 17,
      Base64Binary = 18,
      Date = 19,
      DateTime = 20,
      AnyUri = 21,
      GYear = 22,
      Object = 23,
      String = 24,
      Time = 25
   }
}

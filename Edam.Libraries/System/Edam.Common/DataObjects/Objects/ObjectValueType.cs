using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.Objects
{
   public enum ObjectValueType
   {
      Unknown = -1,
      String = 1,
      Char = 2,
      Bit = 3,
      SByte = 4,
      Byte = 5,
      Int16 = 6,
      Int32 = 7,
      Int64 = 8,
      UInt16 = 9,
      UInt32 = 10,
      UInt64 = 11,
      Single = 12,
      Double = 13,
      Decimal = 14,
      Money = 15,
      Date = 16,
      Time = 17,
      DateTime = 18,
      Text = 19,
      SecretText = 20,
      SmallInt = 41,
      Integer = 42,
      BigInt = 43,
      GUID = 44
   }
}

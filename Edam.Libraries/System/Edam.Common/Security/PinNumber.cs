using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Security.Cryptography;
using Edam.Diagnostics;

namespace Edam.Security
{

   public class PinNumber
   {
      public string Value { get; set; }
      public string HashValue
      {
         get
         {
            if (String.IsNullOrWhiteSpace(Value))
               return string.Empty;
            return Encryptor.Hash(Value);
         }
      }
      public int Length
      {
         get { return Value == null ? 0 : Value.Length; }
      }
      public bool HashCompare(PinNumber pin)
      {
         if (pin == null)
            return false;
         return HashValue == pin.HashValue;
      }
      public bool HashCompare(string hashValue)
      {
         if (String.IsNullOrWhiteSpace(hashValue))
            return false;
         return HashValue == hashValue;
      }

      public PinNumber(string value = null)
      {
         Value = value;
      }
   }

}

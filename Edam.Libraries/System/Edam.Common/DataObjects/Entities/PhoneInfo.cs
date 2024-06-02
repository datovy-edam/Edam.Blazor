using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.Entities
{
   public class PhoneInfo
   {
      public string PhoneId { get; set; }
      public PhoneType PhoneType { get; set; }
      public string PhoneNumber { get; set; }

      public void ClearFields()
      {
         PhoneId = string.Empty;
         PhoneNumber = string.Empty;
         PhoneType = PhoneType.Unknown;
      }

      public void Copy(PhoneInfo phone)
      {
         PhoneId = phone.PhoneId;
         PhoneNumber = phone.PhoneNumber;
         PhoneType = phone.PhoneType;
      }
   }
}

using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.Security
{

   public interface ISecretKey
   {
      String Key { get; set; }
      Byte[] Salt { get; set; }
   }

}

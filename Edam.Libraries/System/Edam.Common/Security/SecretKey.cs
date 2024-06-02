using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Security
{

   public class SecretKey
   {
      public String Key { get; set; }
      public Byte [] Salt { get; set; }
   }

}

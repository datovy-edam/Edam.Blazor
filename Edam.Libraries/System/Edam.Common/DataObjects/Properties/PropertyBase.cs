using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Properties
{

   public class PropertyBase : IProperty
   {
      public string KeyId { get; set; }
      public string Name { get; set; }
      public object Value { get; set; }
   }

}

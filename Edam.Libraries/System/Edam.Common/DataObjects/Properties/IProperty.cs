using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.Properties
{

   public interface IProperty
   {
      string KeyId { get; set; }
      string Name { get; set; }
      object Value { get; set; }
   }

}

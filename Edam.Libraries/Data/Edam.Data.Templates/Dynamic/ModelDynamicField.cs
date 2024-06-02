using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.Dynamic
{
   public class ModelDynamicField
   {
      public string Name;
      public Type Type;
      public object Value;

      public ModelDynamicField(string name, Type type, object value)
      {
         Name = name;
         Type = type;
         Value = value;
      }
   }
}

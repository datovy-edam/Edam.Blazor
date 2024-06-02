using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Dynamic
{

   public class ModelDynamicObject : DynamicObject
   {
      private Dictionary<string, KeyValuePair<Type, object>> _fields;

      public ModelDynamicObject(List<ModelDynamicField> fields)
      {
         _fields = new Dictionary<string, KeyValuePair<Type, object>>();
         fields.ForEach(x => _fields.Add(x.Name,
             new KeyValuePair<Type, object>(x.Type, null)));
      }

      public override bool TrySetMember(SetMemberBinder binder, object? value)
      {
         if (_fields.ContainsKey(binder.Name))
         {
            var type = _fields[binder.Name].Key;
            if (value.GetType() == type)
            {
               _fields[binder.Name] = 
                  new KeyValuePair<Type, object>(type, value);
               return true;
            }
            else 
               throw new Exception(
                  "Value " + value + " is not of type " + type.Name);
         }
         return false;
      }

      public override bool TryGetMember(
         GetMemberBinder binder, out object result)
      {
         result = _fields[binder.Name].Value;
         return true;
      }
   }

}

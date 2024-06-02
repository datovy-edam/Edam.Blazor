using Edam.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.Application
{

   // TODO: SaveItem and GetItem must be reassess...

   /// <summary>
   /// Extremly simple Cache.  Here you can make a composition using your
   /// favorite Cache Class the code will not break as long as you support the
   /// basic functionality that this class offers.
   /// </summary>
   public class Cache : ICache
   {
      private static Dictionary<String,Object> m_Items = null;

      static Cache()
      {
         m_Items = new Dictionary<string, object>();
      }

      public static Object Find(String key)
      {
         Object value = null;
         m_Items.TryGetValue(key, out value);
         return value;
      }

      public static void Add(String key, Object value)
      {
         Object v = Find(key);
         if (v == null)
            m_Items.Add(key, value);
         else
            m_Items[key] = value;
      }

      public void Set<T>(
         string name, T item, string description = null)
      {
         Cache.Add(name, item);
      }

      public T Get<T>(string name)
      {
         var obj = Cache.Find(name);
         return (T)obj;
      }
   }

}

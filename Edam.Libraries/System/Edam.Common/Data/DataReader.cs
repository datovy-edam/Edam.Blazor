using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.Common;

// -----------------------------------------------------------------------------
// https://www.codeproject.com/Articles/827984/
//    Generically-Populate-List-of-Objects-from-SqlDataR

namespace Edam.Data
{

   public class DataReader
   {

      public static T GetObject<T>(DbDataReader reader) where T : new()
      {
         Type etype = typeof(T);
         Hashtable hashtable = new Hashtable();
         PropertyInfo[] properties = etype.GetProperties();
         foreach (PropertyInfo info in properties)
         {
            hashtable[info.Name.ToUpper()] = info;
         }

         T newObject = new T();
         if (reader.Read())
         {
            for (int index = 0; index < reader.FieldCount; index++)
            {
               PropertyInfo info = (PropertyInfo)
                  hashtable[reader.GetName(index).ToUpper()];
               if ((info != null) && info.CanWrite)
               {
                  var val = reader.GetValue(index);
                  if (val != DBNull.Value)
                  {
                     info.SetValue(newObject, val, null);
                  }
                  else
                  {
                     info.SetValue(newObject, null, null);
                  }
               }
            }
         }
         return newObject;
      }

      public static List<T> GetList<T>(DbDataReader reader) where T : new()
      {
         Type etype = typeof(T);
         List<T> list = new List<T>();
         Hashtable hashtable = new Hashtable();
         PropertyInfo[] properties = etype.GetProperties();
         foreach (PropertyInfo info in properties)
         {
            hashtable[info.Name.ToUpper()] = info;
         }
         while (reader.Read())
         {
            T newObject = new T();
            for (int index = 0; index < reader.FieldCount; index++)
            {
               PropertyInfo info = (PropertyInfo)
                  hashtable[reader.GetName(index).ToUpper()];
               if ((info != null) && info.CanWrite)
               {
                  var val = reader.GetValue(index);
                  if (val != DBNull.Value)
                  {
                     info.SetValue(newObject, val, null);
                  }
                  else
                  {
                     info.SetValue(newObject, null, null);
                  }
               }
            }
            list.Add(newObject);
         }
         return list;
      }

   }

}

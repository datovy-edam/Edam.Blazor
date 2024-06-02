using System;
using System.Collections.Generic;
using System.Reflection;

// -----------------------------------------------------------------------------

namespace Edam.B2b.Edi
{

   public class ExchangeDefinitionHelper
   {

      private List<string> m_Header;
      public List<string> Header
      {
         get { return m_Header; }
      }

      public ExchangeDefinitionHelper(List<string> headers)
      {
         m_Header = headers;
      }

      /// <summary>
      /// Given a row as a list of columns, assuming that their position are
      /// well known and correspond exactly to the "ExchangeDefinitionInfo" 
      /// properties layout, then assign their values positionally.
      /// </summary>
      /// <remarks>deviations to the expected properties layout will cause
      /// exceptions to rise</remarks>
      /// <param name="values">properties values list</param>
      /// <returns>values are mapped into the "ExchangeDefinitionInfo" class
      /// and an instance of this class is returned</returns>
      public ExchangeDefinitionInfo GetDefinition(List<string> values)
      {
         int count = 0;
         ExchangeDefinitionInfo def = new ExchangeDefinitionInfo();
         Type t = typeof(ExchangeDefinitionInfo);
         foreach (var i in m_Header)
         {
            PropertyInfo? pinfo = t.GetProperty(i);
            if (pinfo == null)
            {
               continue;
            }

            if (pinfo.PropertyType == typeof(string))
            {
               pinfo.SetValue(def, values[count]);
            }
            else if (pinfo.PropertyType == typeof(int) ||
               pinfo.PropertyType == typeof(int?))
            {
               int v;
               if (int.TryParse(values[count], out v))
               {
                  pinfo.SetValue(def, v);
               }
            }
            else if (pinfo.PropertyType == typeof(short) ||
               pinfo.PropertyType == typeof(short?))
            {
               short v;
               if (short.TryParse(values[count], out v))
               {
                  pinfo.SetValue(def, v);
               }
            }
            else if (pinfo.PropertyType == typeof(long) ||
               pinfo.PropertyType == typeof(long?))
            {
               long v;
               if (long.TryParse(values[count], out v))
               {
                  pinfo.SetValue(def, v);
               }
            }
            else if (pinfo.PropertyType == typeof(decimal) ||
               pinfo.PropertyType == typeof(decimal?))
            {
               decimal v;
               if (decimal.TryParse(values[count], out v))
               {
                  pinfo.SetValue(def, v);
               }
            }
            else if (pinfo.PropertyType == typeof(bool) ||
               pinfo.PropertyType == typeof(bool?))
            {
               bool v;
               if (bool.TryParse(values[count], out v))
               {
                  pinfo.SetValue(def, v);
               }
            }
            count++;
         }
         return def;
      }

   }

}

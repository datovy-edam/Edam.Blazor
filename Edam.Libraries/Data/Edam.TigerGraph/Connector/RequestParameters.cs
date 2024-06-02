using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TigerGraph.Connector
{

   public class RequestParameters
   {
      public const String DefaultDateTimeFormat = "yyyy-MM-dd HH:mm";

      private Int16 m_Count = 0;
      protected System.Text.StringBuilder m_Builder =
         new System.Text.StringBuilder();

      public void Add(String key, String value)
      {
         if (String.IsNullOrWhiteSpace(key))
            return;
         m_Builder.Append(((m_Count >= 1) ? "&" : "?") + key + "=" +
            (String.IsNullOrWhiteSpace(value) ? String.Empty : value));
         m_Count++;
      }

      public void Add(String key, DateTime? value)
      {
         Add(key, (value.HasValue ? String.Empty : value.Value.ToString(
            DefaultDateTimeFormat)));
         m_Count++;
      }

      public override String ToString()
      {
         return m_Builder.ToString();
      }

   }

}

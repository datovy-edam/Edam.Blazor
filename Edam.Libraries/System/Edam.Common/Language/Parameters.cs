using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Language
{

   public class Parameters
   {
      private Dictionary<string, Object> m_Items =
         new Dictionary<string, Object>();

      public Dictionary<string, Object> Items
      {
         get { return m_Items; }
      }

      public void Clear()
      {
         m_Items.Clear();
      }

      /// <summary>
      /// Add an string value.
      /// </summary>
      /// <param name="name">name of parameter</param>
      /// <param name="value">parameter value</param>
      public void Add(string name, string value)
      {
         m_Items.Add(name, value);
      }

      /// <summary>
      /// Add an integer value.
      /// </summary>
      /// <param name="name">name of parameter</param>
      /// <param name="value">parameter value</param>
      public void Add(string name, int value)
      {
         m_Items.Add(name, value);
      }

      /// <summary>
      /// Add an float value.
      /// </summary>
      /// <param name="name">name of parameter</param>
      /// <param name="value">parameter value</param>
      public void Add(string name, float value)
      {
         m_Items.Add(name, value);
      }
   }

}

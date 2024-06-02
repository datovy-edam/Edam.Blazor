using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Models
{

   public class PresentationField
   {
      private String m_Tag = null;
      private ComponentItemType m_Type = ComponentItemType.Unknown;
      public String Name { get; set; }
      public String Value { get; set; }
      public ComponentItemType Type
      {
         get { return m_Type; }
         set
         {
            String t = ComponentHelper.GetFieldTag(value);
            if (t == null)
               return;
            m_Tag = t;
            m_Type = value;
         }
      }
      public String TagLabel
      {
         get { return m_Tag; }
      }
   }

}

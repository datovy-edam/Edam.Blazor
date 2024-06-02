using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Objects;

namespace Edam.DataObjects.Models
{

   public class ObjectValueBase
   {
      private string m_ValueTypeText = ObjectValueType.String.ToString();
      private ObjectValueType m_ValueType = ObjectValueType.String;

      public ObjectValueType ValueType
      {
         get { return m_ValueType; }
         set
         {
            m_ValueType = value;
            m_ValueTypeText = value.ToString();
         }
      }

      public string ValueTypeText
      {
         get { return m_ValueTypeText; }
         set
         {
            if (m_ValueTypeText != value)
            {
               ValueType? val = GetType(value);
               if (val != null)
               {
                  m_ValueType = (ObjectValueType)val;
                  m_ValueTypeText = val.ToString();
               }
            }
         }
      }

      public object OriginalValue { get; set; }
      public string ValueText { get; set; }

      public bool ChangedIndicator
      {
         get
         {
            return HasChanged();
         }
      }

      public ObjectValueBase()
      {
         OriginalValue = null;
         ValueType = ObjectValueType.String;
         ValueText = String.Empty;
      }

      private bool HasChanged()
      {
         if (OriginalValue == null && String.IsNullOrWhiteSpace(ValueText))
            return false;
         if (OriginalValue != null)
            return OriginalValue.ToString() != ValueText;
         return !String.IsNullOrWhiteSpace(ValueText);
      }

      public static ValueType? GetType(string value)
      {
         if (string.IsNullOrEmpty(value))
         {
            value = ObjectValueType.String.ToString();
         }
         if (Enum.TryParse(
            typeof(ObjectValueType), value, out Object? val))
         {
            return (ValueType)val;
         }
         return null;
      }
   }

}

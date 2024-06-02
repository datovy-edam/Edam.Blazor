using System;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Models;

namespace Edam.DataObjects.ReferenceData
{

   /// <summary>
   /// 
   /// </summary>
   public class ReferenceDataValueInfo
   {

      private ElementItemInfo? m_Element;

      public String? ElementName { get; set; }
      public Object? ElementValue { get; set; }
      public Int32? OrdinalValue { get; set; }

      public String? Title
      {
         get { return m_Element == null ? ElementName : m_Element.Title; }
      }
      public Int32 MaxLength
      {
         get { return m_Element == null ? 20 : m_Element.MaxLength; }
      }

      public ReferenceDataValueInfo(ElementItemInfo element)
      {
         m_Element = element;
         ClearFields();
      }

      public void ClearFields()
      {
         ElementName = String.Empty;
         ElementValue = null;
         OrdinalValue = null;
      }

      public static String GetColumnJson(
         String name, String title, Object? value)
      {
         Object? val;
         Boolean isString;
         StringBuilder sb = new StringBuilder();

         // TODO: use JSOB Builder instead...
         if (value is DateTime)
         {
            var dt = (DateTime)value;
            val = dt.Year == 1800 ? "" :
               JsonSerializer.Serialize(dt,typeof(Object)).Replace("\"","");
         }
         else
            val = value;

         // TODO: don't "Replace" but use appropiate encoding...
         isString = val is string;
         String v = (val is bool) ? 
            ((bool)value ? "true": "false") : val.ToString().Replace("\"","'");

         sb.Append("\"" + name + "\":"
            + (isString ? "\"" : "")
            + (name == null ? "" : v)
            + (isString ? "\"" : ""));

         return sb.ToString();
      }

      public String ToJson()
      {
         Boolean isString;
         StringBuilder sb = new StringBuilder();

         isString = ElementValue is string;
         if (ElementName != null)
         {
            String title = m_Element == null ? ElementName : m_Element.Title;
            sb.AppendLine(GetColumnJson(ElementName, title, ElementValue));
         }

         return sb.ToString();
      }

      public void Set(String name, Object value, Int32 ordinal)
      {
         ElementName = name;
         ElementValue = value;
         OrdinalValue = ordinal;
      }

   }

}

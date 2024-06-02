using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Reflection;
using Edam.Text;
using System.Collections;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Models;

namespace Edam.DataObjects.Dynamic
{

   /// <summary>
   /// Model Expando Object helps manage dynamic classes...
   /// </summary>
   public class ModelExpandoObject
   {
      private ExpandoObject m_Instance;
      public ExpandoObject Instance
      {
         get { return m_Instance; }
      }
      public ModelExpandoObject(ExpandoObject? instance = null)
      {
         m_Instance = instance ?? new ExpandoObject();
      }
      public static ExpandoObject NewObject()
      {
         return new ExpandoObject();
      }

      public static dynamic Clone(ExpandoObject item)
      {
         dynamic eObject = new ExpandoObject();
         var expandoDict = item as IDictionary<string, object>;
         foreach (var i in expandoDict.Keys)
         {
            AddProperty(eObject, i, expandoDict[i]);
         }
         return eObject;
      }

      public static dynamic New(List<ModelColumnInfo> columns)
      {
         dynamic expando = new ExpandoObject();
         foreach (var i in columns)
         {
            AddProperty(expando, i.ColumnName, String.Empty);
         }
         return expando;
      }

      public void SetValue(string propertyName, object value)
      {
         SetValue(m_Instance, propertyName, value);
      }

      public static void SetValue(
         ExpandoObject expando, string propertyName, object value)
      {
         // ExpandoObject supports IDictionary so we can extend it like this
         var expandoDict = expando as IDictionary<string, object>;
         if (expandoDict.ContainsKey(propertyName))
            expandoDict[propertyName] = value;
      }

      public object GetValue(string propertyName)
      {
         return GetValue(m_Instance, propertyName);
      }

      public static object? GetValue(
         ExpandoObject expando, string propertyName)
      {
         // ExpandoObject supports IDictionary so we can extend it like this
         var expandoDict = expando as IDictionary<string, object>;
         if (expandoDict.ContainsKey(propertyName))
            return expandoDict[propertyName];
         return null;
      }

      /// <summary>
      /// Get the Data from a dynamic object that has been edited in a form or
      /// elsewhere.
      /// </summary>
      /// <typeparam name="T">type of data</typeparam>
      /// <param name="dataObject">Expando object</param>
      /// <param name="data">object whose values will be set</param>
      public static void GetData<T>(dynamic dataObject, T data)
      {
         ModelExpandoObject model = new ModelExpandoObject(dataObject);

         object value;
         Type etype = typeof(T);
         Hashtable hashtable = new Hashtable();
         PropertyInfo[] properties = etype.GetProperties();
         foreach (PropertyInfo info in properties)
         {
            var obj = model.GetValue(info.Name);
            if (obj == null)
            {
               continue;
            }
            value = null;
            switch(info.PropertyType.Name)
            {
               case "String":
                  value = obj.ToString();
                  break;
               case "Int16":
                  if (short.TryParse(obj.ToString(), out var shortValue))
                  {
                     value = shortValue;
                  }
                  break;
               case "Int32":
                  if (short.TryParse(obj.ToString(), out var intValue))
                  {
                     value = intValue;
                  }
                  break;
               case "Int64":
                  if (long.TryParse(obj.ToString(), out var longValue))
                  {
                     value = longValue;
                  }
                  break;
               case "Decimal":
                  if (decimal.TryParse(obj.ToString(), out var decimalValue))
                  {
                     value = decimalValue;
                  }
                  break;
               case "DateTime":
                  if (DateTime.TryParse(obj.ToString(), out var dateValue))
                  {
                     value = dateValue;
                  }
                  break;
               case "DateTimeOffset":
                  if (DateTimeOffset.TryParse(obj.ToString(), out var dtoValue))
                  {
                     value = dtoValue;
                  }
                  break;
               case "Single":
                  if (float.TryParse(obj.ToString(), out var floatValue))
                  {
                     value = floatValue;
                  }
                  break;
               case "Double":
                  if (double.TryParse(obj.ToString(), out var doubleValue))
                  {
                     value = doubleValue;
                  }
                  break;
               case "Boolean":
                  if (bool.TryParse(obj.ToString(), out var boolValue))
                  {
                     value = boolValue;
                  }
                  break;
               default:
                  value = obj;
                  break;
            }
            info.SetValue(data, value, null);
         }
      }

      public void AddProperty(string propertyName, object propertyValue)
      {
         AddProperty(m_Instance, propertyName, propertyValue);
      }

      public static void AddProperty(
         ExpandoObject expando, string propertyName, object propertyValue)
      {
         // ExpandoObject supports IDictionary so we can extend it like this
         var expandoDict = expando as IDictionary<string, object>;
         if (expandoDict.ContainsKey(propertyName))
            expandoDict[propertyName] = propertyValue;
         else
            expandoDict.Add(propertyName, propertyValue);
      }

      public void AddEvent(
         string eventName, Action<object, EventArgs> handler)
      {
         AddEvent(m_Instance, eventName, handler);
      }

      public static void AddEvent(
         ExpandoObject expando, string eventName,
         Action<object, EventArgs> handler)
      {
         var expandoDict = expando as IDictionary<string, object>;
         if (expandoDict.ContainsKey(eventName))
            expandoDict[eventName] = handler;
         else
            expandoDict.Add(eventName, handler);
      }

      /// <summary>
      /// Copy a source to a destination... both must be using the same exact
      /// underlying dictionary...
      /// </summary>
      /// <param name="source">source to copy from</param>
      /// <param name="destination">destination to copy to</param>
      public static void Copy(ExpandoObject source, ExpandoObject destination)
      {
         IDictionary<string, object> sdic =
            source as IDictionary<string, object>;
         IDictionary<string, object> ddic =
            destination as IDictionary<string, object>;
         foreach(var i in sdic)
         {
            ddic[i.Key] = sdic[i.Key];
         }
      }

      /// <summary>
      /// Expando object to JSON string.  Each object property - value is 
      /// translated into its corresponding JSON key - value and returned as a 
      /// set.
      /// </summary>
      /// <param name="expando">expando object</param>
      /// <returns>JSON string is returned</returns>
      public static string ToJson(ExpandoObject expando)
      {
         System.Text.StringBuilder sb = new StringBuilder();
         sb.AppendLine("{");
         IDictionary<string, object> expandoDict =
            expando as IDictionary<string, object>;
         int iCount = expandoDict.Count;
         int c = 1;
         foreach (var i in expandoDict)
         {
            sb.AppendLine(JsonBuilder.ToJson(i.Key, i.Value, c != iCount));
            c++;
         }
         sb.AppendLine("}");
         return sb.ToString();
      }

      public static string ToJson(dynamic item)
      {
         return ToJson(new ModelExpandoObject(item));
      }

   }

}

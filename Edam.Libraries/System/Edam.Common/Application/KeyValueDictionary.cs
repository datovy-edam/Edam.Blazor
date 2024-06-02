using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.Application
{

   public class KeyValueEntry
   {

      /// <summary>
      /// how many times the value has been inspected?  This is not done for you
      /// ... you must update the count in your code.
      /// </summary>
      public int InspectCount { get; set; }

      public string Key { get; set; }
      public object Value { get; set; }

      public KeyValueEntry(string key, object value)
      {
         InspectCount = 0;
         Key = key;
         Value = value;
      }

   }

   /// <summary>
   /// KeyValue Dictionary of KeyValue Entries...
   /// </summary>
   public class KeyValueDictionary
   {
      private Dictionary<string, KeyValueEntry> m_KeyValuePairs = 
         new Dictionary<string, KeyValueEntry>();

      /// <summary>
      /// max times the value can be inspected
      /// </summary>
      public int MaxInspectCount { get; set; }

      public KeyValueDictionary()
      {
         MaxInspectCount = 1;
      }

      public KeyValueEntry Find(string key)
      {
         if (m_KeyValuePairs.ContainsKey(key))
         {
            return m_KeyValuePairs[key];
         }
         return null;
      }

      public KeyValueEntry Add(string key, object value)
      {
         KeyValueEntry entry = Find(key);
         if (entry == null)
         {
            entry = new KeyValueEntry(key, value);
            m_KeyValuePairs.Add(key, entry);
         }
         else
         {
            entry.Value = value;
         }
         return entry;
      }
   }

}

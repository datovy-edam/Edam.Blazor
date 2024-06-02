using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// -----------------------------------------------------------------------------

namespace Edam.TextParse
{

   /// <summary>Helper to manage Web Query Strings, related strings and alike in
   /// different formats</summary>
   public class KeyValueParser
   {

      /// <summary>
      /// Given a String in Hex encoding parse it and return corresponding
      /// Key - Value dictionary.
      /// </summary>
      /// <param name="hexText">Hex String</param>
      /// <param name="sentenceSeparator">sentence separator</param>
      /// <param name="keyValueSeparator">key-value separator</param>
      /// <returns>instance of Dictionary is returned</returns>
      public static Dictionary<String, String> TextToKeyValue(
         String text, string sentenceSeparator, char keyValueSeparator,
         bool removeValueQuotes = true)
      {
         Dictionary<String, String> d = new Dictionary<String, String>();
         String[] keyValuesText = text.Split(sentenceSeparator);
         String[] keyValues;
         String value;

         foreach (String i in keyValuesText)
         {
            var txt = i.Trim();
            if (String.IsNullOrWhiteSpace(txt))
               continue;

            keyValues = i.Split(keyValueSeparator);
            if (keyValues.Length != 2)
               continue;

            value = keyValues[1];
            if (removeValueQuotes)
            {
               if (String.IsNullOrWhiteSpace(value) || value == "\"\"")
               {
                  value = String.Empty;
               }
               else
               {
                  if (value.Substring(0, 1) == "\"")
                     value = value.Substring(1, value.Length - 1);
                  if (!String.IsNullOrWhiteSpace(value))
                  {
                     if (value.Substring(value.Length - 1) == "\"")
                        value = value.Substring(0, value.Length - 1);
                  }
               }
            }

            d.Add(keyValues[0].Trim(), value.Trim());
         }

         return d;
      }  // end of HexTextToKeyValue

      /// <summary>
      /// Given a String in Hex encoding parse it and return corresponding
      /// Key - Value dictionary.
      /// </summary>
      /// <param name="hexText">Hex String</param>
      /// <param name="sentenceSeparator">sentence separator</param>
      /// <param name="keyValueSeparator">key-value separator</param>
      /// <returns>instance of Dictionary is returned</returns>
      public static Dictionary<String, String> HexTextToKeyValue(
         String hexText, char sentenceSeparator, char keyValueSeparator)
      {
         Dictionary<String, String> d = new Dictionary<String, String>();
         String s = Edam.Serialization.Serialize.HexToText(hexText);
         String[] keyValuesText = s.Split(sentenceSeparator);
         String[] keyValues;

         foreach (String i in keyValuesText)
         {
            keyValues = i.Split(keyValueSeparator);
            if (keyValues.Length != 2)
               continue;
            d.Add(keyValues[0].Trim(), keyValues[1].Trim());
         }

         return d;
      }  // end of HexTextToKeyValue

      /// <summary>
      /// Given a Web Query String in Hex encoding parse it and return
      /// corresponding Key - Value dictionary.
      /// </summary>
      /// <param name="hexText">Hex Web Query String</param>
      /// <returns>instance of Dictionary is returned</returns>
      public static Dictionary<String, String> WebHexQueryStringToKeyValue(
         String hexText)
      {
         return HexTextToKeyValue(hexText, '&', '=');
      }  // end of HexTextToKeyValue

   }  // end of QueryStringParser

}  // end of Edam.Text namespace




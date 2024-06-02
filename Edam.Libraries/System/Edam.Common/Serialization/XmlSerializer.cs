using System;
using System.Collections.Generic;
using System.Xml.Serialization;

// -----------------------------------------------------------------------------

namespace Edam.Serialization
{

   /// <summary>
   /// Generic XML serialization.
   /// </summary>
   /// <typeparam name="T">Type to serialize.</typeparam>
   public class XmlSerializer<T>
   {

      /// <summary>
      /// Serialize the configuration to an XML string.
      /// </summary>
      /// <param name="value">item to serialize</param>
      /// <returns>XML string is returned</returns>
      public static String ToXmlString(T value)
      {
         return Edam.Serialization.Serialize.ToXmlString(value);
      }

      /// <summary>
      /// Deserialize a configuration XML string and return an instance
      /// of the configuration.
      /// </summary>
      /// <param name="value">XML string to deserialize</param>
      /// <returns>instanced of the Config object is retured</returns>
      public static T FromXmlString(String value)
      {
         return (T) Edam.Serialization.Serialize.
            FromXmlString(value, typeof(T));
      }

      ///// <summary>
      ///// Persist the config to a file.
      ///// </summary>
      ///// <param name="filePath">file Path</param>
      ///// <param name="value">config object instance to persist</param>
      //public static Boolean ToFile(String filePath, T value)
      //{
      //   String xmlDoc = ToXmlString(value);
      //   System.IO.File.WriteAllText(filePath, xmlDoc);
      //   return true;
      //}

      ///// <summary>
      ///// Read the config from a file.
      ///// </summary>
      ///// <param name="filePath">file path</param>
      ///// <returns>instance of DataAggregateList is returned</returns>
      //public static T FromFile(String filePath)
      //{
      //   String data = System.IO.File.ReadAllText(filePath);
      //   return FromXmlString(data);
      //}
   }

}

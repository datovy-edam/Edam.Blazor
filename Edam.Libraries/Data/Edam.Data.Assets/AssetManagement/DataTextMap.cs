using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

// -----------------------------------------------------------------------------

using Edam.Data.AssetConsole;
using Edam.Data.Asset;

namespace Edam.Data.AssetManagement
{

   public class DataTextMap
   {
      public const string DDL_MAP_FILE = "DdlTextMap.json";

      public List<DataTextMapItem> Items { get; set; }
      public List<DataTextTypeMap> Types { get; set; }
      public List<DataTextElementInfo> Element { get; set; }
      public List<DataTextElementPropertyInfo> ElementProperty { get; set; }

      public DataTextMap()
      {
         Items = new List<DataTextMapItem>();
         Types = new List<DataTextTypeMap>();
         Element = new List<DataTextElementInfo>();
         ElementProperty = new List<DataTextElementPropertyInfo>();
      }

      public static DataTextMap FromFile(String filePath)
      {
         String mapText = File.ReadAllText(filePath);
         var items = JsonConvert.DeserializeObject<DataTextMap>(mapText);
         return items;
      }

      /// <summary>
      /// Load the Text Map from arguments.
      /// </summary>
      /// <param name="arguments">Arguments list</param>
      /// <returns>the loaded DataTextMap is returned</returns>
      public static DataTextMap FromFile(AssetConsoleArgumentsInfo arguments)
      {
         if (String.IsNullOrWhiteSpace(arguments.TextMapFilePath))
         {
            return new DataTextMap();
         }
         return DataTextMap.FromFile(arguments.TextMapFilePath);
      }

      public static DataTextMap FromFolder(AssetConsoleArgumentsInfo arguments)
      {
         if (String.IsNullOrWhiteSpace(arguments.TextMapFolderPath))
         {
            return new DataTextMap();
         }
         return DataTextMap.FromFile(arguments.TextMapFolderPath + DDL_MAP_FILE);
      }

      /// <summary>
      /// Map Text based on given direction.
      /// </summary>
      /// <param name="textName">the name of the text to map</param>
      /// <param name="direction">the direction, To or [From] (default)</param>
      /// <returns>mapped type name (if any, else the given textName is returned
      /// </returns>
      public String MapText(String textName, 
         DataTextMapDirection direction = DataTextMapDirection.From)
      {
         if (Items == null)
         {
            return textName;
         }

         if (direction == DataTextMapDirection.Unknown)
            return textName;
         DataTextMapItem itm;
         if (direction == DataTextMapDirection.From)
         {
            itm = Items.Find((x) => x.SourceText == textName.ToLower());
            return itm == null ? textName : itm.TargetText;
         }

         //var l = textName.Split(':');
         //string tname = l.Length > 1 ? l[1] : l[0];
         itm = Items.Find((x) => x.SourceText == textName);
         return itm == null ? null : itm.TargetText;
      }

      /// <summary>
      /// Find Element to review how to manage it while mapping to another 
      /// schema.
      /// </summary>
      /// <param name="qualifiedElementName">qualified element name</param>
      /// <returns>found element info else null</returns>
      public DataTextElementInfo FindElement(string qualifiedElementName)
      {
         if (Element == null)
            return null;
         if (String.IsNullOrWhiteSpace(qualifiedElementName))
            return null;
         return Element.Find((x)=> x.ElementName == qualifiedElementName);
      }

      /// <summary>
      /// Find Element to review how to manage it while mapping to another 
      /// schema.
      /// </summary>
      /// <param name="qualifiedElementName">qualified element name</param>
      /// <returns>found element info else null</returns>
      public DataTextElementPropertyInfo FindElementProperty(
         string qualifiedElementName)
      {
         if (ElementProperty == null)
            return null;
         if (String.IsNullOrWhiteSpace(qualifiedElementName))
            return null;
         return ElementProperty.Find(
            (x) => x.ElementName == qualifiedElementName);
      }

      /// <summary>
      /// Map a given Name based on given direction.
      /// </summary>
      /// <param name="textName">the name to map</param>
      /// <param name="direction">the direction, To or From</param>
      /// <returns>mapped name (if any, else the given name is returned
      /// </returns>
      public String MapName(String name,
         DataTextMapDirection direction = DataTextMapDirection.From)
      {
         if (String.IsNullOrWhiteSpace(name) ||
            direction == DataTextMapDirection.Unknown)
            return name;
         DataTextMapItem itm;
         if (direction == DataTextMapDirection.From)
         {
            itm = Items.Find(
               (x) => x.SourceText.Length <= name.Length &&
                      x.SourceText == name.Substring(0, x.SourceText.Length));
            return itm == null ? name : itm.TargetText;
         }
         itm = Items.Find(
            (x) => x.TargetText.Length <= name.Length &&
                   x.TargetText == name.Substring(0, x.TargetText.Length));
         return itm == null ? name : itm.SourceText;
      }

      public DataTextTypeInfo MatchElementType(
         IDataElement element, DataTextTypeInfo defaultType = null)
      {
         if (Types == null)
         {
            return defaultType;
         }

         Match match;
         DataTextTypeInfo type = defaultType;
         if (element == null)
            return defaultType;
         foreach(var i in Types)
         {
            foreach(var r in i.Items)
            {
               match = null;
               if (!String.IsNullOrWhiteSpace(r.NamePattern))
                  match = Regex.Match(
                     element.ElementQualifiedName.OriginalName.ToLower(), 
                     r.NamePattern.ToLower());
               if (match == null && !String.IsNullOrWhiteSpace(r.TypePattern))
                  match = Regex.Match(
                     element.TypeQualifiedName.OriginalName.ToLower(), 
                     r.TypePattern.ToLower());
               if (match != null && match.Success)
               {
                  type = r.Type;
                  return type;
               }
            }
         }
         return type;
      }

   }

}

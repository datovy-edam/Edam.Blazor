using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// -----------------------------------------------------------------------------
using Edam.InOut;

namespace Edam.Data.Asset
{

   public class UriResourceInfo
   {
      public const string EXT_DDL = ".sql";
      public const string EXT_JSD = ".jsd.json";
      public const string EXT_JSON = ".json";
      public const string EXT_XML = ".xml";
      public const string EXT_XSD = ".xsd";
      public const string EXT_YAML = ".yaml";
      public const string EXT_YSD = ".ysd.yaml";
      public const string EXT_XLSX = ".xlsx";

      public static List<string> GetExtensionTexts()
      {
         return new List<string> { EXT_XSD, EXT_JSD };
      }

      public static List<UriResourceType> GetExtensionTypes()
      {
         return new List<UriResourceType>
         {
            UriResourceType.xsd,
            UriResourceType.jsdjson
         };
      }

      /// <summary>
      /// Get Resource/File Extension base on its type.
      /// </summary>
      /// <param name="type"></param>
      /// <returns></returns>
      public static string GetResourceExtension(UriResourceType type)
      {
         string txt;
         switch(type)
         {
            case UriResourceType.ddl:
               txt = EXT_DDL;
               break;
            case UriResourceType.jsdjson:
               txt = EXT_JSD;
               break;
            case UriResourceType.json:
               txt = EXT_JSON;
               break;
            case UriResourceType.xml:
               txt = EXT_XML;
               break;
            case UriResourceType.xsd:
               txt = EXT_XSD;
               break;
            case UriResourceType.yaml:
               txt = EXT_YAML;
               break;
            case UriResourceType.ysdyaml:
               txt = EXT_YSD;
               break;
            case UriResourceType.xlsx:
               txt = EXT_XLSX;
               break;
            default:
               txt = String.Empty;
               break;
         }
         return txt;
      }

      /// <summary>
      /// Get Resource/File Extension base on its type.
      /// </summary>
      /// <param name="type"></param>
      /// <returns></returns>
      public static UriResourceType GetResourceExtension(string type)
      {
         UriResourceType t;
         switch (type)
         {
            case EXT_DDL:
               t = UriResourceType.ddl;
               break;
            case EXT_JSD:
               t = UriResourceType.jsdjson;
               break;
            case EXT_JSON:
               t = UriResourceType.json;
               break;
            case EXT_XML:
               t = UriResourceType.xml;
               break;
            case EXT_XSD:
               t = UriResourceType.xsd;
               break;
            case EXT_YAML:
               t = UriResourceType.yaml;
               break;
            case EXT_YSD:
               t = UriResourceType.ysdyaml;
               break;
            case EXT_XLSX:
               t = UriResourceType.xlsx;
               break;
            default:
               t = UriResourceType.Unknown;
               break;
         }
         return t;
      }

      /// <summary>
      /// Get Expanded Uri List
      /// </summary>
      /// <param name="UriList"></param>
      /// <param name="type"></param>
      /// <returns></returns>
      public static List<string> GetUriList(
         List<Uri> UriList, UriResourceType type)
      {
         string ext = GetResourceExtension(type);
         if (String.IsNullOrWhiteSpace(ext))
         {
            throw new Exception("Type (" + type.ToString() + ") not supported");
         }

         List<string> items = new List<string>();
         foreach (var i in UriList)
         {
            // TODO: declare .xsd as a constant...
            var item = i.IsAbsoluteUri ? i.AbsolutePath : i.OriginalString;
            FolderFileReader.GetFilePathNames(item, ext, items);
         }
         return items;
      }

      public static IEnumerable<string> GetUriFiles(
         String folderPath, List<string> extensions)
      {
         //var ext = new List<string> { "json" };
         var files = Directory
             .EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
             .Where(s => extensions.Contains(
                Path.GetExtension(s).TrimStart('.').ToLowerInvariant()));
         return files;
      }

   }

}

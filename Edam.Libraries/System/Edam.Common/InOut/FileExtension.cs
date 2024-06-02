using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.InOut
{

   public class FileExtension
   {
      public const string CSV = "csv";
      public const string JSONLD = "jsonld";
      public const string OPEN_XML = "xlsx";
      public const string XSD = "xsd";
      public const string JSD = "jsd.json";
      public const string JSON = "json";
      public const string GQL = "gql";
      public const string DDL = "sql";

      public static bool IsCsv(string extension)
      {
         return extension == CSV;
      }
      public static bool IsOpenXml(string extension)
      {
         return extension == OPEN_XML;
      }
      public static bool IsXsd(string extension)
      {
         return extension == XSD;
      }
      public static bool IsJsd(string extension)
      {
         return extension == JSD;
      }
      public static bool IsJson(string extension)
      {
         return extension == JSON;
      }
   }

}

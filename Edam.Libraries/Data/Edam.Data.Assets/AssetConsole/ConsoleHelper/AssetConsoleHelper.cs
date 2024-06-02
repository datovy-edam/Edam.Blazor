using System;
using System.Collections.Generic;
using System.Text;

namespace Edam.Data.AssetConsole.ConsoleHelper
{

   public class AssetConsoleHelper
   {
      public const string NEW = "new";
      public const string USE = "use";
      public const string SCHEMA_TYPE = "schematype";
      public const string URI_PREFIX = "uriprefix";
      public const string URI_NAMESPACE = "urins";
      public const string ROOT_ELEMENT_NAME = "rootname";
      public const string TEXT_MAP_FILE_PATH = "textmapfilepath";
      public const string TEXT_MAP_FOLDER_PATH = "textmapfolderpath";
      public const string MAX_THRESHOLD = "maxthreshold";
      public const string LIST_LENGTH = "listlength";
      public const string PROCEDURE_NAME = "procedurename";
      public const string USE_ARGS_FILE = "useargsfile";

      public const string ORG_URI = "orgdomainuri";

      public const string IN_FILE_PATH = "infilepath";
      public const string IN_FILE_NAME = "infilename";
      public const string IN_FILE_EXT = "infileextention";
      public const string IN_FILE_FULL_PATH = "infullfilepath";

      public const string OUT_FILE_PATH = "outfpath";
      public const string OUT_FILE_NAME = "outfname";
      public const string OUT_FILE_EXT = "outfext";
      public const string OUT_FILE_FULL_PATH = "outffpath";

      public const string ELEMENT_TRANSFORM = "elementransform";

      public const string CONNECTION_STRING = "connectionstring";

      public static void WriteHelp()
      {
         // TODO: make sure that the help is generalized and applicable to all
         //     : schema types...

         Console.WriteLine(
            "XmlGen - Utility to generate XML instance given an XML Schema");
         Console.WriteLine("Usage - XmlGen <schema>.xsd [/"
            + ROOT_ELEMENT_NAME + ":] [/"
            + URI_NAMESPACE + ":] [/"
            + MAX_THRESHOLD + ":] [/"
            + LIST_LENGTH + ":] [/"
            + OUT_FILE_PATH + ":] [/"
            + IN_FILE_EXT + ":]");
         Console.WriteLine("/" + ROOT_ELEMENT_NAME
            + ": LocalName of the root element from where generate XML");
         Console.WriteLine("/" + URI_NAMESPACE
            + ": Namespace of the root element");
         Console.WriteLine("/" + MAX_THRESHOLD
            + ": Number of elements to be generated if maxOccurs='unbounded' or a really high number");
         Console.WriteLine("/" + LIST_LENGTH
            + ": Number of items while generating a list type");
         Console.WriteLine("/" + OUT_FILE_PATH
            + ": Output Dictionary file path name");
         Console.WriteLine("/" + OUT_FILE_EXT
            + ": Output Dictionary file extension");
         Console.WriteLine("/" + IN_FILE_EXT
            + ": Input Dictionary file extension");

         Console.WriteLine("/" + CONNECTION_STRING
            + ": Database Connection String");
      }

      public static String PrepareParam(String itemName, String value)
      {
         return "/" + itemName + ":" + value;
      }

      // assumes all same case.        
      public static bool ArgumentMatch(string arg, string toMatch)
      {
         return arg == toMatch;
         //if (arg[0] != '/' && arg[0] != '-')
         //{
         //   return false;
         //}
         //arg = arg.Substring(1);
         //return arg == toMatch;
      }
   }

}


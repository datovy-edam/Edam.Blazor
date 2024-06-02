using System;
using System.Collections.Generic;
using System.Text;
using Edam.Data.Asset;

namespace Edam.Json.Jsd
{

   public class JsdItemInfo
   {
      public const String ADDITIONAL_PROPERTIES =
         "additionalProperties";
      public const String ID = "$id";
      public const String TYPE = "type";
      public const String TITLE = "title";
      public const String STRING = "string";
      public const String PROPERTIES = "properties";
      public const String DEFINITIONS = "definitions";
      public const String DESCRIPTION = "description";
      public const String COMPONENTS = "components";
      public const String CONTEXT = "context";
      public const String SCHEMAS = "schemas";
      public const String ENUM = "enum";

      public NamespaceInfo Namespace { get; set; }
      public String ItemName { get; set; }
      public String UriPrefix { get; set; }
      public IDataElement Element { get; set; }
      public String Description { get; set; }
      public JsdType Type { get; set; }
      public String TypeText
      {
         get { return JsdTypeInfo.ToString(Type); }
      }
      public Boolean AdditionalProperties { get; set; }

      public JsdItemInfo(NamespaceInfo ns, String name, JsdType type,
         Boolean additionalProperties = true)
      {
         Namespace = ns;
         ItemName = name;
         Type = type;
         AdditionalProperties = additionalProperties;
      }

      public static String ToString(JsdItem item)
      {
         var txt = item switch
         {
            JsdItem.Properties => PROPERTIES,
            JsdItem.Definitions => DEFINITIONS,
            _ => String.Empty,
         };
         return txt;
      }
   }

}

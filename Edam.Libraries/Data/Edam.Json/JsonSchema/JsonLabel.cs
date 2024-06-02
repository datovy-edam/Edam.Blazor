using System;
using System.Collections.Generic;
using System.Text;

namespace Edam.Json.JsonSchema
{

   public class JsonLabel
   {
      public const string DEFINITIONS = "definitions";
      public const string PROPERTIES = "properties";
      public const string DESCRIPTION = "description";
      public const string REQUIRED = "required";
      public const string ONE_OF = "oneOf";
      public const string ANY_OF = "anyOf";
      public const string ALL_OF = "allOf";
      public const string FORMAT = "format";
      public const string TYPE = "type";
      public const string ENUM = "enum";
      public const string ARRAY = "array";
      public const string GLOBAL = "(global)";

      public const string STRING = "string";
      public const string INTEGER = "integer";
      public const string OBJECT = "object";
      public const string NUMBER = "number";
      public const string BOOL = "boolean";

      public const string URI = "uri";
      public const string EMAIL = "email";
      public const string DATE = "date";
      public const string TIME = "time";
      public const string DATETIME = "datetime";
      public const string DATE_TIME = "date-time";

      public const string NOT_SUPPORTED = "NOT-SUPPORTED";
      public const string REF = "$ref";
      public const string ZERO_TO_MANY = "(0:*)";
      public const string ZERO_TO_ONE = "(0:1)";
      public const string SLASH = "/";
      public const string DOLLAR_ID = "$id";

      public const string JSON_SCHEMA_URI =
         "http://json-schema.org/draft-07/schema";
      public const string JSON_SCHEMA_PREFIX = "jsd";
   }

}

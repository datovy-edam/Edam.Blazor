using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Gsql
{

   public class GsqlTypeInfo
   {

      public const String STRING = "string";
      public const String BOOLEAN = "bool";
      public const String FLOAT = "float";
      public const String DOUBLE = "double";
      public const String INT = "int";
      public const String DATETIME = "dateTime";
      public const String JSON_OBJECT = "jsonobject";
      public const String JSON_ARRAY = "jsonarray";
      public const String UINT = "uint";

      public static GsqlItemType DataTypeToJsonType(String type)
      {
         GsqlItemType gtype;
         if (type == BOOLEAN)
            gtype = GsqlItemType.Boolean;
         else if (type == STRING)
            gtype = GsqlItemType.String;
         else if (type == JSON_OBJECT)
            gtype = GsqlItemType.JsonObject;
         else if (type == JSON_ARRAY)
            gtype = GsqlItemType.JsonArray;
         else if (type == FLOAT)
            gtype = GsqlItemType.Float;
         else if (type == UINT)
            gtype = GsqlItemType.UnsignedInteger;
         else if (type == INT)
            gtype = GsqlItemType.Integer;
         else if (type == DOUBLE)
            gtype = GsqlItemType.Double;
         else if (type == INT)
            gtype = GsqlItemType.Integer;
         else if (type == DATETIME)
            gtype = GsqlItemType.DateTime;
         else
            gtype = GsqlItemType.Unknown;
         return gtype;
      }

      public static String ToString(GsqlItemType type)
      {
         return type.ToString();
      }

   }

}

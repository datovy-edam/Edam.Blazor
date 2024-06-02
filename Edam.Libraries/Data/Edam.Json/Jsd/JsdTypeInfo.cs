using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;

namespace Edam.Json.Jsd
{
   public class JsdTypeInfo
   {
      // TODO: Change method of type to type-text conversion ...

      public const String JSTRING = "string";
      public const String JOBJECT = "object";
      public const String JINTEGER = "number";
      public const String JINT = "number";
      public const String JBOOLEAN = "boolean";
      public const String JBYTE = "number";
      public const String JSHORT = "number";
      public const String JFLOAT = "number";
      public const String JLONG = "number";
      public const String JDECIMAL = "number";
      public const String JBASE64BINARY = "string";
      public const String JTIME = "string";
      public const String JDATE = "string";
      public const String JDATETIME = "string";
      public const String JANYURI = "object";
      public const String JGYEAR = "number";
      public const String JNULL = "null";
      public const String JEMPTY = "empty";
      public const String JNUMBER = "number";

      public const String OBJECT = "object";
      public const String STRING = "string";
      public const String BOOLEAN = "boolean";
      public const String BYTE = "byte";
      public const String SHORT = "short";
      public const String FLOAT = "float";
      public const String LONG = "long";
      public const String INTEGER = "integer";
      public const String DECIMAL = "decimal";
      public const String INT = "int";
      public const String BASE64BINARY = "base64Binary";
      public const String TIME = "time";
      public const String DATE = "date";
      public const String DATETIME = "dateTime";
      public const String ANYURI = "anyURI";
      public const String GYEAR = "gYear";

      public static JsdType DataTypeToJsonType(String type)
      {
         JsdType jtype;
         if (type == BOOLEAN)
            jtype = JsdType.Boolean;
         else if (type == STRING)
            jtype = JsdType.String;
         else if (type == OBJECT)
            jtype = JsdType.Object;
         else if (type == BYTE)
            jtype = JsdType.Byte;
         else if (type == SHORT)
            jtype = JsdType.Short;
         else if (type == FLOAT)
            jtype = JsdType.Float;
         else if (type == LONG)
            jtype = JsdType.Long;
         else if (type == INTEGER)
            jtype = JsdType.Integer;
         else if (type == DECIMAL)
            jtype = JsdType.Decimal;
         else if (type == INT)
            jtype = JsdType.Int;
         else if (type == BASE64BINARY)
            jtype = JsdType.Base64Binary;
         else if (type == TIME)
            jtype = JsdType.Time;
         else if (type == DATE)
            jtype = JsdType.Date;
         else if (type == DATETIME)
            jtype = JsdType.DateTime;
         else if (type == ANYURI)
            jtype = JsdType.AnyUri;
         else if (type == GYEAR)
            jtype = JsdType.GYear;
         else if (type == OBJECT)
            jtype = JsdType.Object;
         else
            jtype = JsdType.Unknown;
         return jtype;
      }

      public static String ToString(JsdType type)
      {
         String t;
         switch (type)
         {
            case JsdType.String:
               t = JSTRING;
               break;
            case JsdType.Boolean:
               t = JBOOLEAN;
               break;
            case JsdType.Byte:
               t = JBYTE;
               break;
            case JsdType.Short:
               t = JSHORT;
               break;
            case JsdType.Float:
               t = JFLOAT;
               break;
            case JsdType.Long:
               t = JLONG;
               break;
            case JsdType.Integer:
               t = JINTEGER;
               break;
            case JsdType.Decimal:
               t = JDECIMAL;
               break;
            case JsdType.Int:
               t = JINT;
               break;
            case JsdType.Base64Binary:
               t = JBASE64BINARY;
               break;
            case JsdType.Time:
               t = JTIME;
               break;
            case JsdType.Date:
               t = JDATE;
               break;
            case JsdType.DateTime:
               t = JDATETIME;
               break;
            case JsdType.AnyUri:
               t = JANYURI;
               break;
            case JsdType.Number:
               t = JNUMBER;
               break;
            case JsdType.Null:
               t = JNULL;
               break;
            case JsdType.Empty:
               t = JEMPTY;
               break;
            case JsdType.GYear:
               t = JGYEAR;
               break;
            case JsdType.Object:
            default:
               t = JOBJECT;
               break;
         }
         return t;
      }
   }
}

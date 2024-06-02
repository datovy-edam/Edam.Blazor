using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;

namespace Edam.Xml.Xsd
{
   public class XsdHelper
   {
      public const string XSD_PREFIX = "xsd";
      public const string XS_PREFIX = "xs";
      public const string DEFAULT_TYPE_NAME = "string";
      public const string XSD_NAMESPACE = "http://www.w3.org/2001/XMLSchema";

      public static string GetNamespace(string prefix, NamespaceInfo ns)
      {
         if (prefix == XSD_PREFIX || prefix == XS_PREFIX)
         {
            return XSD_NAMESPACE;
         }
         return ns.Uri.OriginalString;
      }

      public static String GetIndent(short block)
      {
         string b = "   ";
         string i = "";
         for (var c = 0; c < block; c++)
            i += b;
         return i;
      }

      public static String GetComplexTypeStart(
         short block, string name = null, bool isType = true)
      {
         var indent = GetIndent(block);
         var nm = name == null ? "" :
             " name =\"" + name + (isType ? "Type" : "") + "\"";
         var ct = new System.Text.StringBuilder();
         ct.AppendLine(indent + "<xs:complexType" + nm + ">");
         ct.AppendLine(indent + "   <xs:sequence>");
         return ct.ToString();
      }

      public static String GetComplexTypeEnd(short block)
      {
         var indent = GetIndent(block);
         var ct = new System.Text.StringBuilder();
         ct.AppendLine(indent + "   </xs:sequence>");
         ct.AppendLine(indent + "</xs:complexType>");
         return ct.ToString();
      }
      
      public static String GetElementComplexTypeStart(
         short block, string name = null, bool addMinMax = true)
      {
         var indent = GetIndent(block);
         var mx = addMinMax ? " minOccurs=\"0\" maxOccurs=\"unbounded\"" : "";
         var ct = new System.Text.StringBuilder();
         ct.AppendLine(indent + "<xs:element name=\"" + name + "\"" + mx + ">");
         ct.AppendLine(indent + "   <xs:complexType>");
         ct.AppendLine(indent + "      <xs:sequence>");
         return ct.ToString();
      }

      public static String GetElementComplexTypeEnd(short block)
      {
         var indent = GetIndent(block);
         var ct = new System.Text.StringBuilder();
         ct.AppendLine(indent + "      </xs:sequence>");
         ct.AppendLine(indent + "   </xs:complexType>");
         ct.AppendLine(indent + "</xs:element>");
         return ct.ToString();
      }

      public static String GetElementStart(short block, List<String> dic,
         string name, string type, string ns, bool nillable = true,
         bool isType = true, bool isClosed = false)
      {
         var indent = GetIndent(block);
         var t = type == null ? "" : type;
         string ty;
         switch (t)
         {
            case "varchar":
               ty = "xs:string";
               ns = "";
               break;
            case "bit":
               ty = "xs:boolean";
               ns = "";
               break;
            case "char":
               ty = "xs:string";
               ns = "";
               break;
            case "datetime2":
               ty = "xs:dateTime";
               ns = "";
               break;
            case "datetime":
               ty = "xs:dateTime";
               ns = "";
               break;
            case "date":
               ty = "xs:date";
               ns = "";
               break;
            case "bigint":
               ty = "xs:unsignedInt";
               ns = "";
               break;
            case "int":
               ty = "xs:unsignedInt";
               ns = "";
               break;
            case "smallint":
               ty = "xs:unsignedInt";
               ns = "";
               break;
            case "money":
               ty = "xs:decimal";
               ns = "";
               break;
            case "decimal":
               ty = "xs:decimal";
               ns = "";
               break;
            default:
               ty = name + (isType ? "Type" : "");
               ns = ns + ":";
               break;
         }

         if (dic != null)
         {
            var rtype = ty + "." + name;
            if (dic.Any(s => s.Contains(rtype)))
               return null;
            dic.Add(rtype);
         }

         return indent + "<xs:element name=\"" + name + "\" type=\"" + ns
            + ty + "\" " + (nillable ? "nillable=\"false\"" : "") 
            + (isClosed ? "/" : "") + ">";
      }

      public static String GetElementEnd(short block)
      {
         var indent = GetIndent(block);
         return indent + "</xs:element>";
      }

      public static String GetElementRef(
         short block, string name, string ns, bool nillable = true)
      {
         var indent = GetIndent(block);
         return indent
            + "<xs:element ref=\"" + ns + ":" + name
            + "\" minOccurs=\"0\" maxOccurs=\"1\"/>";
      }

      public static String GetSchemaStart(short block, string tn, string ns)
      {
         var indent = GetIndent(block);
         System.Text.StringBuilder rt = new StringBuilder();
         rt.AppendLine(indent + "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
         rt.AppendLine(indent
            + "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" ");
         rt.AppendLine(indent + "           xmlns:" + ns + "=\"" + tn + "\" ");
         rt.AppendLine(indent + "           targetNamespace=\"" + tn + "\" ");
         rt.AppendLine(indent
            + "           elementFormDefault=\"qualified\" "
            + "attributeFormDefault=\"unqualified\">");
         return rt.ToString();
      }

      public static String GetSchemaEnd(short block)
      {
         var indent = GetIndent(block);
         return indent + "</xs:schema>";
      }
   }
}

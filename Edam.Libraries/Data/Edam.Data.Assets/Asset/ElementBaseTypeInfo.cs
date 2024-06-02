using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------

namespace Edam.Data.Asset
{

   public class ElementBaseTypeInfo
   {
      public const string INTEGER = "integer";
      public const string FLOAT = "float";
      public const string DOUBLE = "double";
      public const string STRING = "string";
      public const string DECIMAL = "decimal";
      public const string DATE = "date";
      public const string TIME = "time";
      public const string TIMESTAMP = "timestamp";
      public const string DATETIME = "datetime";
      public const string OBJECT = "object";
      public const string NULL = "null";
      public const string BOOLEAN = "boolean";
      public const string CHAR = "char";
      public const string SHORT = "short";
      public const string INT = "int";
      public const string LONG = "long";
      public const string NCNAME = "ncname";
      public const string ANY_SIMPLE_TYPE = "anysimpletype";
      public const string ANY_TYPE = "anytype";

      public const string XSD = "xsd";
      public const string XS = "xs";

      public static List<string> BASE_TYPES = new List<string>()
      {
         STRING, BOOLEAN, DECIMAL, INTEGER, INT, DATE, DATETIME, FLOAT, DOUBLE,
         TIME, TIMESTAMP, LONG, SHORT, NCNAME, ANY_SIMPLE_TYPE, ANY_TYPE, CHAR
      };
      public static List<string> OBJECT_BASE_TYPES = new List<string>()
      {
         OBJECT
      };

      public string TypeName { get; set; }
      public ElementBaseType Type { get; set; }
      public ElementGroup Group { get; set; }
      public ConstraintType Constraint { get; set; }
      public BaseGrammarType GrammarType { get; set; }

      public bool IsBaseType
      {
         get
         {
            return IsBase(TypeName);
         }
      }

      public ElementBaseTypeInfo(BaseGrammarType grammarType)
      {
         Type = ElementBaseType.Unknown;
         Group = ElementGroup.NotApplicable;
         Constraint = ConstraintType.unknown;
         TypeName = string.Empty;
         GrammarType = grammarType;
      }

      public static Boolean IsBase(string type)
      {
         QualifiedNameInfo qn = GetBaseType(type);
         return qn != null;
      }

      public static Boolean IsBase(string type, BaseGrammarType grammarType)
      {
         QualifiedNameInfo qn = GetBaseType(type, grammarType);
         return qn != null;
      }

      public static Boolean IsObject(string type, BaseGrammarType grammarType)
      {
         QualifiedNameInfo qn = new QualifiedNameInfo(type);
         var r = OBJECT_BASE_TYPES.Find((x) => x == qn.OriginalName.ToLower());
         return !String.IsNullOrWhiteSpace(r);
      }

      /// <summary>
      /// It tries to prove that is a base type, if did it returned its
      /// qualified name;
      /// </summary>
      /// <param name="type">qualified name text</param>
      /// <returns>instance of QualifiedNameInfo is returned</returns>
      public static QualifiedNameInfo GetBaseType(
         string type, BaseGrammarType grammarType)
      {
         if (String.IsNullOrWhiteSpace(type))
         {
            return null;
         }

         // data base type...
         QualifiedNameInfo qn = new QualifiedNameInfo(type);
         string r = BASE_TYPES.Find((x) => x == qn.OriginalName.ToLower());
         bool found = !String.IsNullOrWhiteSpace(r);

         if (found)
         {
            qn = new QualifiedNameInfo(r);
         }

         // entity base type...
         if (!found)
         {
            r = OBJECT_BASE_TYPES.Find((x) => x == qn.OriginalName.ToLower());
            found = !String.IsNullOrWhiteSpace(r);
         }

         // TODO: XS and XSD are constants!!!
         // TODO: only relevant to XML...
         if (grammarType == BaseGrammarType.XSD &&
            !String.IsNullOrWhiteSpace(qn.Prefix))
         {
            if ((qn.Prefix == XS || qn.Prefix == XSD) && found)
            {
               return found ? qn : null;
            }
            else
            {
               return null;
            }
         }
         return found ? qn : null;
      }

      public static QualifiedNameInfo GetBaseType(string type)
      {
         return GetBaseType(type, BaseGrammarType.XSD);
      }

      /// <summary>
      /// If type is a base Get an instance of DataTypeInfo.
      /// </summary>
      /// <param name="type">qualified name text</param>
      /// <returns>instance of DataTypeInfo is returned</returns>
      public static DataTypeInfo GetDataTypeInfo(
         string type, BaseGrammarType grammarType)
      {
         QualifiedNameInfo qn = GetBaseType(type, grammarType);
         return qn == null ? null : new DataTypeInfo(qn, ElementBaseType.Base);
      }

      /// <summary>
      /// Given a text Type Name return its corresponding enumeration.
      /// </summary>
      /// <param name="typeName">type name text</param>
      /// <returns>the found DataType is returned, if not recognized Unknown is
      /// returned</returns>
      public static ElementBaseTypeInfo GetElementTypeInfo(
         String typeName, BaseGrammarType grammarType)
      {
         String tn = typeName.ToLower();
         ElementBaseTypeInfo einfo = new ElementBaseTypeInfo(grammarType);
         einfo.TypeName = typeName;
         if (tn == "base")
         {
            einfo.Type = ElementBaseType.Base;
         }
         else if (tn == "type")
         {
            einfo.Type = ElementBaseType.Type;
         }
         else if (tn == "key")
         {
            einfo.Type = ElementBaseType.Element;
            einfo.Constraint = ConstraintType.key;
         }
         else if (tn == "object")
         {
            einfo.Type = ElementBaseType.Object;
         }
         else if (tn == "element")
         {
            einfo.Type = ElementBaseType.Element;
         }
         else if (tn == "attribute")
         {
            einfo.Type = ElementBaseType.Attribute;
         }
         else if (tn == "reference")
         {
            einfo.Type = ElementBaseType.Type;
            einfo.Group = ElementGroup.Reference;
         }
         else if (tn == "optall")
         {
            einfo.Type = ElementBaseType.Type;
            einfo.Group = ElementGroup.OptionAll;
         }
         else if (tn == "optany")
         {
            einfo.Type = ElementBaseType.Type;
            einfo.Group = ElementGroup.OptionAny;
         }
         else if (tn == "optone")
         {
            einfo.Type = ElementBaseType.Type;
            einfo.Group = ElementGroup.OptionOne;
         }
         else if (tn == "subsgroup")
         {
            einfo.Type = ElementBaseType.Type;
            einfo.Group = ElementGroup.SubstitutionGroup;
         }
         else if (tn == "template")
         {
            einfo.Type = ElementBaseType.Type;
            einfo.Group = ElementGroup.Template;
         }
         return einfo;
      }

      public static ElementBaseTypeInfo 
         GetElementTypeInfo(ConstraintType type, BaseGrammarType grammarType)
      {
         ElementBaseTypeInfo einfo = new ElementBaseTypeInfo(grammarType);
         einfo.Constraint = type;
         switch(type)
         {
            case ConstraintType.autoGenerate:
               einfo.Type = ElementBaseType.Constraint;
               einfo.Constraint = ConstraintType.autoGenerate;
               break;
            //case ConstraintType.identity:
            //   break;
            case ConstraintType.key:
               einfo.Type = ElementBaseType.Constraint;
               einfo.Constraint = ConstraintType.key;
               break;
            //case ConstraintType.nonkey:
            //   break;
            //case ConstraintType.undefined:
            //   break;
            //case ConstraintType.unknown:
            //   break;
            default:
               einfo.Group = ElementGroup.NotApplicable;
               einfo.Type = ElementBaseType.Unknown;
               break;
         }
         return einfo;
      }

      public static ElementBaseTypeInfo GetElementTypeInfo(
         ElementType type, BaseGrammarType grammarType)
      {
         ElementBaseTypeInfo einfo = new ElementBaseTypeInfo(grammarType);
         switch (type)
         {
            case ElementType.attribute:
               einfo.Type = ElementBaseType.Attribute;
               break;
            case ElementType.element:
               einfo.Type = ElementBaseType.Element;
               break;
            case ElementType.enumerator:
               einfo.Type = ElementBaseType.Element;
               break;
            case ElementType.reference:
               einfo.Type = ElementBaseType.Element;
               break;
            case ElementType.root:
               einfo.Type = ElementBaseType.Element;
               break;
            case ElementType.procedure:
               einfo.Type = ElementBaseType.Type;
               break;
            case ElementType.function:
               einfo.Type = ElementBaseType.Type;
               break;
            case ElementType.view:
               einfo.Type = ElementBaseType.Type;
               break;
            case ElementType.type:
               einfo.Type = ElementBaseType.Type;
               break;
            case ElementType.undefined:
               einfo.Type = ElementBaseType.Unknown;
               break;
            case ElementType.unknown:
               einfo.Type = ElementBaseType.Unknown;
               break;
            default:
               einfo.Group = ElementGroup.NotApplicable;
               einfo.Type = ElementBaseType.Unknown;
               break;
         }
         return einfo;
      }

   }

}

using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetSchema;

namespace Edam.Json.JsonSchema
{

    public class JsonQualifiedNameInfo : QualifiedNameInfo
    {
        public static readonly string PROPERTIES = "properties";
        public static readonly string DEFINITIONS = "definitions";
        public static readonly string REF_ROOT = "#/";

        public static readonly string REF_PROPERTIES =
           REF_ROOT + PROPERTIES + "/";
        public static readonly string REF_DEFINITIONS =
           REF_ROOT + DEFINITIONS + "/";

        public string BasePrefix { get; set; }

        private JsonQualifiedNameType m_NameType = JsonQualifiedNameType.Unknown;
        public JsonQualifiedNameType NameType
        {
            get { return m_NameType; }
        }

        public JsonQualifiedNameInfo(string qname) : base(qname)
        {
            SetQualifiedName(qname);
        }

        private void SetQualifiedName(string qname)
        {
            // not qualified... try setting it...
            JsonQualifiedNameType type = GetNameType(
               Name, PROPERTIES, JsonQualifiedNameType.Property);
            if (type == JsonQualifiedNameType.Unknown)
                type = GetNameType(
                   Name, DEFINITIONS, JsonQualifiedNameType.Definition);
            m_NameType = type;
            GetNamespacePrefix(qname);
            switch (type)
            {
                case JsonQualifiedNameType.Definition:
                    BasePrefix = Prefix.Replace(REF_ROOT + PROPERTIES, string.Empty);
                    break;
                case JsonQualifiedNameType.Property:
                    BasePrefix =
                       Prefix.Replace(REF_ROOT + DEFINITIONS, string.Empty);
                    break;
                default:
                    BasePrefix = string.Empty;
                    break;
            }
        }

        public static string GetTypeName(string typeName)
        {
            if (!typeName.StartsWith(REF_ROOT))
                return typeName;
            else if (typeName.StartsWith(REF_PROPERTIES))
                return typeName.Replace(REF_PROPERTIES, string.Empty);
            else if (typeName.StartsWith(REF_DEFINITIONS))
                return typeName.Replace(REF_DEFINITIONS, string.Empty);
            return typeName;
        }

        public void SetPrefix(NamespaceList namespaces)
        {
            BasePrefix = namespaces.GetDefaultPrefix();
            Prefix = BasePrefix;
        }

        public static ElementType CheckQualifiedName(
           QualifiedNameInfo qname, NamespaceList namespaces)
        {
            if (qname == null)
                return ElementType.undefined;

            ElementType type = ElementType.undefined;
            switch (qname.OriginalName)
            {
                case JsonLabel.DOLLAR_ID:
                    type = ElementType.element;
                    qname.Prefix = JsonLabel.JSON_SCHEMA_PREFIX;
                    break;
                case JsonLabel.OBJECT:
                    type = ElementType.element;
                    qname.Prefix = JsonLabel.JSON_SCHEMA_PREFIX;
                    break;
                case JsonLabel.BOOL:
                    type = ElementType.element;
                    qname.Prefix = JsonLabel.JSON_SCHEMA_PREFIX;
                    break;
                case JsonLabel.STRING:
                    type = ElementType.element;
                    qname.Prefix = JsonLabel.JSON_SCHEMA_PREFIX;
                    break;
                case JsonLabel.NUMBER:
                    type = ElementType.element;
                    qname.Prefix = JsonLabel.JSON_SCHEMA_PREFIX;
                    break;
                case JsonLabel.INTEGER:
                    type = ElementType.element;
                    qname.Prefix = JsonLabel.JSON_SCHEMA_PREFIX;
                    break;
                case JsonLabel.TIME:
                    type = ElementType.element;
                    qname.Prefix = JsonLabel.JSON_SCHEMA_PREFIX;
                    break;
                case JsonLabel.URI:
                    type = ElementType.element;
                    qname.Prefix = JsonLabel.JSON_SCHEMA_PREFIX;
                    break;
                case JsonLabel.EMAIL:
                    type = ElementType.element;
                    qname.Prefix = JsonLabel.JSON_SCHEMA_PREFIX;
                    break;
                case JsonLabel.DATE:
                    type = ElementType.element;
                    qname.Prefix = JsonLabel.JSON_SCHEMA_PREFIX;
                    break;
                case JsonLabel.DATE_TIME:
                    type = ElementType.element;
                    qname.Prefix = JsonLabel.JSON_SCHEMA_PREFIX;
                    break;
                default:
                    type = ElementType.unknown;
                    qname.Prefix = namespaces.GetDefaultPrefix();
                    break;
            }

            return type;
        }

        public void GetNamespacePrefix(string name)
        {
            if (name == null)
                return;
            string nm = name.Replace('.', '/');
            string[] l = nm.Split('/');
            if (l.Length == 0)
                return;
            string token = l[^1];
            string[] t = token.Split(':');
            if (t.Length == 2)
            {
                Prefix = t[0];  // + ":*";
            }
        }

        private static JsonQualifiedNameType GetNameType(
           string text, string name, JsonQualifiedNameType type)
        {
            int indx = text.IndexOf(name);
            if (indx == -1)
                return JsonQualifiedNameType.Unknown;
            return type;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetManagement;
using Edam.Data.AssetSchema;
using Edam.Json.LinkData;

namespace Edam.Json.Jsd
{

   public class JsdSchema : ISchemaWriter
   {

      #region -- Static Strings...

      private const String CLASS_NAME = "JsdSchema";
      private const String CONTEXT = "@context";
      private const String SCHEMA = "$schema";
      private const String ID = "$id";
      private const String REF = "$ref";
      public const String DEFINITIONS = "definitions";
      public const String PROPERTIES = "properties";
      public const String COMPONENTS = "components";
      public const String SCHEMAS = "schemas";
      public const String TYPE = "type";
      public const String OBJECT = "object";
      public const String ARRAY = "array";
      public const String ITEMS = "items";

      public const String ALL_OF = "allOf";
      public const String ONE_OF = "oneOf";

      private const String FALSE = "false";
      //private const String TRUE = "true";

      public const String SEPARATOR = ":";

      public const String BLOCK_OPEN = "{";
      public const String BLOCK_CLOSE = "}";
      public const String ARRAY_OPEN = "[";
      public const String ARRAY_CLOSE = "]";
      public const String DEFAULT_OUT_EXTENSION = "jsd.json";

      #endregion
      #region -- Properties and Declarations...

      public NamespaceList Namespaces { get; } =
         new NamespaceList();
      public NamespaceInfo Namespace { get; set; }

      private readonly List<JsdItemInfo> m_Items = new List<JsdItemInfo>();

      private JsdDocumentInfo m_Document;

      private StringBuilder m_OutText = new StringBuilder();
      private readonly Edam.Text.IndentText m_Indent = new Text.IndentText();

      public JsdSchema Instance
      {
         get { return this; }
      }

      public Edam.Text.IndentText Indent
      {
         get { return m_Indent; }
      }

      public List<JsdItemInfo> Items
      {
         get { return m_Items; }
      }

      private bool m_ElementAsItemAndArrayIndicator = false;
      public bool ElementAsItemAndArrayIndicator
      {
         get { return m_ElementAsItemAndArrayIndicator; }
         set { m_ElementAsItemAndArrayIndicator = value; }
      }

      private bool m_JsonLinkLdContextIndicator = true;
      public bool JsonLinkLdContextIndicator
      {
         get { return m_JsonLinkLdContextIndicator; }
         set { m_JsonLinkLdContextIndicator = value; }
      }

      private string m_OutExtension = DEFAULT_OUT_EXTENSION;
      public string OutExtension
      {
         get { return m_OutExtension; }
         set { m_OutExtension = value; }
      }

      #endregion
      #region -- Delegate Properties...

      public BaseTypeResolverDelegate BaseTypeResolver { get; set; }
      public NamespaceResolverDelegate NamespaceResolver { get; set; }
      public TypeResolverDelegate TypeResolver { get; set; }
      public DataTransformResolverDelegate TransformResolver { get; set; }

      #endregion
      #region -- Constructor, Destructor...

      public void Initialize(NamespaceInfo ns, String title, JsdType type,
         JsdVersion version = JsdVersion.Latest,
         Boolean additionalProperties = true)
      {
         m_Document =
            new JsdDocumentInfo(ns, title, type, version, additionalProperties);
         Namespace = ns;
      }

      public JsdSchema(NamespaceInfo ns, String  title, JsdType type,
         JsdVersion version = JsdVersion.Latest, 
         Boolean additionalProperties = true)
      {
         Initialize(ns, title, type, version, additionalProperties);
      }

      public JsdSchema(JsdDocumentInfo document)
      {
         SetDocument(document);
      }

      public JsdSchema()
      {
         Initialize(null, null, JsdType.Object);
      }

      #endregion
      #region -- Support Methods

      public void SetDocument(JsdDocumentInfo document)
      {
         m_Document = document;
      }

      #endregion
      #region -- Namespace Support

      public void AddNamespace(NamespaceInfo ns)
      {
         if (ns == null)
            return;
         NamespaceInfo i = Namespaces.Find((x) => x.Prefix == ns.Prefix);
         if (i == null)
            Namespaces.Add(ns);
      }

      public void Add(string prefix, string uri)
      {
         NamespaceInfo ns = new NamespaceInfo(prefix, uri);
         if (Namespace.Prefix != prefix)
            AddNamespace(ns);
      }

      #endregion
      #region -- Property Support

      public void AddTitle(String title, Boolean continues)
      {
         // write: "description": "[property-description]"
         if (!String.IsNullOrWhiteSpace(title))
            m_OutText.AppendLine(m_Indent.Identation + "\"" +
               JsdItemInfo.TITLE + "\": \"" +
               title + "\"" + (continues ? "," : String.Empty));
      }

      public void AddDescription(String description, Boolean continues)
      {
         // write: "description": "[property-description]"
         if (!String.IsNullOrWhiteSpace(description))
            m_OutText.AppendLine(m_Indent.Identation + "\"" +
               JsdItemInfo.DESCRIPTION + "\": \"" +
               description + "\"" + (continues ? "," : String.Empty));
      }

      public void AddPropertyStart(String itemName = null)
      {
         string name = (String.IsNullOrWhiteSpace(itemName) ? String.Empty :
            "\"" + itemName + "\": ");
         m_OutText.AppendLine(m_Indent.Identation + name + BLOCK_OPEN);
         m_Indent.Push();
      }

      public void AddPropertyEnd(Boolean continues)
      {
         m_Indent.Pop();
         m_OutText.AppendLine(m_Indent.Identation + BLOCK_CLOSE +
            (continues ? "," : String.Empty));
      }

      public void AddPropertyValue(String propertyName, String value, 
         Boolean continues)
      {
         m_OutText.AppendLine(m_Indent.Identation 
            + "\"" + propertyName + "\": \"" + value + "\""
            + (continues ? "," : String.Empty));
      }

      /// <summary>
      /// Add Element (JSON-LD - Link Data) Context...
      /// </summary>
      /// <param name="elementName">qualified element name</param>
      public void AddElementContext(String elementName)
      {
         m_OutText.AppendLine(
            m_Indent.Identation + "\"@" + JsdItemInfo.CONTEXT + "\": {");
         AddPush();

         // write type
         AddPropertyValue(TYPE, JsdItemInfo.STRING, true);

         // write description
         AddDescription(elementName + " JSON-LD context", true);

         // write reference link-ld as enum
         AddPropertyValue(JsdItemInfo.ENUM, "../JSON-LD_Contexts/" + 
            elementName.Replace(":","") + "Context.jsonld", false);

         AddPop();
         m_OutText.AppendLine(m_Indent.Identation + "}");
      }

      #endregion
      #region -- Add Reference Properties Support

      /// <summary>
      /// Add Reference Property
      /// </summary>
      /// <param name="ns">namespace information</param>
      /// <param name="propertyName">property name</param>
      /// <param name="continues">true if there are other element declarations 
      /// that follow, else false</param>
      public void AddReferenceProperty(
         NamespaceInfo ns, String propertyName, Boolean continues)
      {
         AddPropertyStart(propertyName);

         string loc = (Namespace.Uri.OriginalString == ns.Uri.OriginalString) ?
            String.Empty : ns.NamePath.FullName;
         string locText = String.IsNullOrWhiteSpace(loc) ?
            String.Empty : loc + "." + m_OutExtension;
         string propName = JsonLinkLdContextIndicator ?
            propertyName.Replace(":","") : propertyName;
         m_OutText.AppendLine(m_Indent.Identation + "\"" + REF +
            "\": \"" + locText + "#/" + PROPERTIES + "/" + propName + "\"");

         AddPropertyEnd(continues);
      }

      /// <summary>
      /// Add reference ("$ref") link...
      /// </summary>
      /// <param name="ns">namespace information</param>
      /// <param name="sectionName">section name (i.e. PROPERTIES)</param>
      /// <param name="propertyName">property name</param>
      /// <returns></returns>
      public String GetReference(
         NamespaceInfo ns, String sectionName, String propertyName)
      {
         string loc = (Namespace.Uri.OriginalString == ns.Uri.OriginalString) ?
            String.Empty : ns.NamePath.FullName;
         string locText = String.IsNullOrWhiteSpace(loc) ?
            String.Empty : loc + "." + m_OutExtension;
         string propName = JsonLinkLdContextIndicator ?
            propertyName.Replace(":", "") : propertyName;
         return "\"" + REF + "\": \"" + locText + "#/" +
            sectionName + "/" + propName + "\"";
      }

      /// <summary>
      /// Add reference 
      /// </summary>
      /// <param name="ns">namespace information</param>
      /// <param name="propertyName">property name</param>
      /// <param name="continues">true if there are other element declarations 
      /// that follow, else false</param>
      public void AddReference(
         NamespaceInfo ns, String propertyName, Boolean continues)
      {
         AddPropertyStart(propertyName);
         m_OutText.AppendLine(m_Indent.Identation +
            GetReference(ns, PROPERTIES, propertyName));
         AddPropertyEnd(continues);
      }

      #endregion
      #region -- Indentation Support

      /// <summary>
      /// Indentation - Push Support
      /// </summary>
      public void AddPush()
      {
         m_Indent.Push();
      }

      /// <summary>
      /// Indentation - Pop Support
      /// </summary>
      public void AddPop()
      {
         m_Indent.Pop();
      }

      #endregion
      #region -- Combinations Support

      public void AddAllOf(NamespaceInfo ns, IDataElement element,
         String referenceName, List<AssetDataElement> children)
      {
         string section = JsonLinkLdContextIndicator ? 
            COMPONENTS + "/" + SCHEMAS : DEFINITIONS;

         // add "allOf": [
         m_OutText.AppendLine(m_Indent.Identation + "\"" + ALL_OF + "\": [");
         m_Indent.Push();

         // add reference to base type
         m_OutText.AppendLine(m_Indent.Identation + "{");
         m_Indent.Push();
         m_OutText.AppendLine(m_Indent.Identation +
            GetReference(ns, section, referenceName));
         m_Indent.Pop();
         m_OutText.AppendLine(m_Indent.Identation + "},");

         // write properties block
         m_OutText.AppendLine(m_Indent.Identation + "{");
         m_Indent.Push();

         // add "type": "object"
         AddPropertyValue(TYPE, OBJECT, true);

         // title
         AddTitle(element.ElementName, true);

         // description
         AddDescription(element.Description, true);

         AddChildren(element, children);
         AddPropertyEnd(false);
         m_Indent.Pop();
         m_OutText.AppendLine(m_Indent.Identation + "]");
      }

      public void AddOneOf(List<AssetDataElement> children)
      {
         int chilCount = children.Count - 1;
         // add "oneOf": [
         m_OutText.AppendLine(m_Indent.Identation + "\"" + ONE_OF + "\": [");
         m_Indent.Push();

         for (var i = 0; i < children.Count; i++)
         {
            AddPropertyStart();
            AddPropertyStart(PROPERTIES);
            NamespaceInfo ns = NamespaceResolver == null ?
               new NamespaceInfo("", children[i].NamespaceText) :
               NamespaceResolver(children[i].NamespaceText);
            AddReferenceProperty(children[i].ElementNamespace, 
               children[i].ElementName, i < chilCount);
            AddPropertyEnd(false);
            AddPropertyEnd((i < children.Count - 1));
         }

         m_Indent.Pop();
         m_OutText.AppendLine(m_Indent.Identation + "]");
      }

      public void AddOneOf(IDataElement element, Boolean continues)
      {
         // add "oneOf": [
         m_OutText.AppendLine(m_Indent.Identation + "\"" + ONE_OF + "\": [");
         m_Indent.Push();

         AddPropertyStart();
         NamespaceInfo ns = NamespaceResolver == null ?
            new NamespaceInfo("", element.NamespaceText) :
            NamespaceResolver(element.NamespaceText);
         AddReferenceProperty(ns, element.ElementName, false);
         AddPropertyEnd(false);

         m_Indent.Pop();
         m_OutText.AppendLine(m_Indent.Identation + "]");
      }

      public void AddElementArray(
         NamespaceInfo ns, String elementName, String description,
         String section, Boolean continues)
      {
         AddPropertyValue(TYPE, ARRAY, true);
         AddDescription(description, true);

         if (String.IsNullOrWhiteSpace(section))
         {
            m_OutText.AppendLine(m_Indent.Identation + "\"" + 
               ITEMS + "\": " + BLOCK_OPEN + " \"" + TYPE + "\": \"" + 
               elementName + "\" " + BLOCK_CLOSE);
         }
         else
         {
            AddPropertyStart(ITEMS);
            m_OutText.AppendLine(m_Indent.Identation +
               GetReference(ns, section, elementName));
            AddPropertyEnd(false);
         }
      }

      public void AddOneOfArrayOption(
         NamespaceInfo ns, String elementName, String description,
         String section, Boolean continues)
      {
         // add "oneOf": [
         m_OutText.AppendLine(m_Indent.Identation + "\"" + ONE_OF + "\": [");
         m_Indent.Push();

         AddPropertyStart();
         m_OutText.AppendLine(m_Indent.Identation +
            GetReference(ns, section, elementName));
         AddPropertyEnd(false);

         AddPropertyStart();
         AddElementArray(ns, elementName, description,  section, continues);
         AddPropertyEnd(false);

         m_Indent.Pop();
         m_OutText.AppendLine(m_Indent.Identation + "]");
      }

      #endregion
      #region -- Types Definitions Support

      public void AddEnum(
         ElementKeyValue [] items, Boolean groupAsOneOf = false)
      {
         
      }

      public String GetPrefixedName(NamespaceInfo ns, String name)
      {
         return ns.Prefix + ":" + name;
      }

      /// <summary>
      /// Add Childrent Elements...
      /// </summary>
      /// <param name="element"></param>
      /// <param name="children"></param>
      public void AddChildren(
         IDataElement element, List<AssetDataElement> children)
      {
         int chilCount = children.Count - 1;
         // add "properties": {
         AddPropertyStart(PROPERTIES);
         for (var i = 0; i < children.Count; i++)
         {
            if (JsonLinkLdContextIndicator)
            {
               AddElement(children[i], i < chilCount);
               continue;
            }

            string elementName = (JsonLinkLdContextIndicator) ?
               LinkData.LinkData.GetCamelCaseName(children[i]) :
               children[i].ElementName;

            AddReferenceProperty(children[i].ElementNamespace, elementName, 
               m_JsonLinkLdContextIndicator || i < chilCount);
         }

         if (m_JsonLinkLdContextIndicator)
         {
            AddElementContext(element.ElementName);
         }

         AddPropertyEnd(false);
      }

      /// <summary>
      /// Add Complex Type...
      /// </summary>
      /// <param name="type"></param>
      /// <param name="element"></param>
      /// <param name="children"></param>
      /// <param name="continues"></param>
      public void AddComplexType(
         DataTypeInfo type, IDataElement element, 
         List<AssetDataElement> children, Boolean continues)
      {
         NamespaceInfo ns = NamespaceResolver == null ?
            new NamespaceInfo("", element.NamespaceText) : 
            NamespaceResolver(element.NamespaceText);

         String typeName = type == null ? 
            element.ElementName : type.GetTypeName(element);

         // write: "property-name": {
         AddPropertyStart(JsonLinkLdContextIndicator ?
            typeName.Replace(":","") : typeName);

         // write: "title": ""
         AddTitle(element.ElementName, true);

         // write: "description": "[property-description]"
         string desc = String.IsNullOrWhiteSpace(element.Description) ?
            element.AnnotationText : element.Description;
         AddDescription(desc, true);

         var baseType = BaseTypeResolver(element);
         if (baseType == null || baseType.IsBase || 
            baseType.DataTypeName == element.ElementName)
         {
            if (children.Count == 0)
            {
               JsdType jtype =
                  JsdTypeInfo.DataTypeToJsonType(element.ElementDataType);
               String dtype = JsdTypeInfo.ToString(jtype);
               AddPropertyValue(TYPE, dtype, false);
            }
            else
            {
               ElementBaseTypeInfo elementType =
                  ElementBaseTypeInfo.GetElementTypeInfo(
                     element.ElementGroup.ToString(),
                     BaseGrammarType.Unknown);
               if (elementType.Group == ElementGroup.OptionOne)
                  AddOneOf(children);
               else
                  AddChildren(element, children);
            }
         }
         else
         {
            AddAllOf(baseType.Namespace, element,
               baseType.DataTypeName, children);
         }

         AddPropertyEnd(continues);
      }

      #endregion
      #region -- Element Properties Support

      public JsdItemInfo PrepareElement(IDataElement element)
      {
         NamespaceInfo ns1 = NamespaceResolver == null ?
            new NamespaceInfo("", element.NamespaceText) :
            NamespaceResolver(element.NamespaceText);
         return new JsdItemInfo(ns1, element.ElementName, JsdType.Object);
      }

      /// <summary>
      /// Add Element...
      /// </summary>
      /// <param name="element"></param>
      /// <param name="continues"></param>
      public void AddElement(IDataElement element, Boolean continues)
      {
         string section = JsonLinkLdContextIndicator ?
            COMPONENTS + "/" + SCHEMAS : DEFINITIONS;
         JsdItemInfo item = PrepareElement(element);
         JsdItemInfo i = JsonLinkLdContextIndicator ? null :
            m_Items.Find((x) => (x.ItemName == item.ItemName));
         if (i == null || JsonLinkLdContextIndicator)
         {
            string eName = JsonLinkLdContextIndicator ?
               LinkData.LinkData.GetCamelCaseName(element) :
               element.ElementName;
            DataTypeInfo baseType = BaseTypeResolver(element);
            AddPropertyStart(eName);
            AddDescription(String.IsNullOrWhiteSpace(element.Description) ?
               element.AnnotationText : element.Description, true);
            if (baseType != null)
            {
               bool isObject = baseType.ElementType == ElementBaseType.Object
                  || baseType.ElementType == ElementBaseType.Type;
               string tname = isObject ? baseType.DataTypeName :
                  baseType.BaseElement.TypeQualifiedName.OriginalName;
               if (ElementAsItemAndArrayIndicator)
               {
                  AddOneOfArrayOption(baseType.Namespace, tname,
                     baseType.BaseElement.Description,
                     isObject ? section : null, false);
               }
               else
               {
                  AddElementArray(baseType.Namespace, tname, 
                     baseType.BaseElement.Description, 
                     isObject ? section : null, false);
               }
            }
            AddPropertyEnd(continues);
            m_Items.Add(item);
         }
      }

      #endregion
      #region -- Document Support

      private void AddContext()
      {
         m_OutText.AppendLine(
            m_Indent.Identation + "\"" + CONTEXT + "\"" + 
            SEPARATOR + " " + BLOCK_OPEN);
         m_Indent.Push();

         for (var i=0; i<Namespaces.Count; i++)
         {
            AddPropertyValue(
               Namespaces[i].Prefix, Namespaces[i].ToReferenceUriText() + "#",
               (i<Namespaces.Count-1));
         }
         AddPropertyEnd(true);
      }

      private void AddHeader()
      {
         m_OutText.AppendLine("{");
         m_Indent.Push();

         if (JsonLinkLdContextIndicator)
         {
            // output schema components...
            AddPropertyStart(COMPONENTS);
            AddPropertyStart(SCHEMAS);

            return;
         }

         // output JSON Schema...
         String v = JsdVersionInfo.ToString(m_Document.Version);
         AddPropertyValue(SCHEMA, v, true);
         if (m_Document.Namespace.Uri != null && 
            !String.IsNullOrEmpty(m_Document.Namespace.Uri.OriginalString))
            AddPropertyValue(
               ID, m_Document.Namespace.ToReferenceUriText(), true);

         // AddContext();

         if (!m_Document.AdditionalProperties)
            AddPropertyValue(JsdItemInfo.ADDITIONAL_PROPERTIES, FALSE, true);
         if (m_Document.Type == JsdType.Supress)
            return;
         JsdType t = m_Document.Type == JsdType.Unknown ?
            JsdType.Object : m_Document.Type;
         String tt = JsdTypeInfo.ToString(t);
         AddPropertyValue(JsdItemInfo.TYPE, tt, true);
      }

      public void DocumentStart()
      {
         AddHeader();
      }

      public void WriteDocument()
      {
         StringBuilder body = m_OutText;

         m_OutText = new StringBuilder();
         m_Indent.Clear();
         DocumentStart();
         m_OutText.Append(body);
         DocumentEnd();
      }

      public void DocumentEnd()
      {
         string func = CLASS_NAME + ":" + "DocumentEnt::";

         if (JsonLinkLdContextIndicator)
         {
            m_OutText.AppendLine(BLOCK_CLOSE);
            m_Indent.Pop();
            m_OutText.AppendLine(BLOCK_CLOSE);
            m_Indent.Pop();
         }

         m_Indent.Pop();

         if (m_Indent.IdentCount != 0)
            throw new Exception(func + "INVALID IDENTATION.");

         m_OutText.AppendLine(BLOCK_CLOSE);
      }

      public new String ToString()
      {
         return m_OutText.ToString();
      }

      #endregion

   }

}

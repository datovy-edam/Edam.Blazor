using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Edam.Xml.XmlExplore;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetManagement;
using Edam.Data.AssetSchema;

namespace Edam.Xml.Xsd
{

   public class XsdItem
   {
      public String ItemName { get; set; }
      public String UriPrefix { get; set; }
      public XmlSchemaElement Element { get; set; }
   }

   public class XsdSchema : ISchemaWriter
   {
      private Dictionary<string, List<string>> m_Registry = 
         new Dictionary<string, List<string>>();
      public List<XsdItem> Items = new List<XsdItem>();

      public DataTextMapResolverDelegate DataTextMapResolver { get; set; }
      public BaseTypeResolverDelegate BaseTypeResolver { get; set; }
      public NamespaceResolverDelegate NamespaceResolver { get; set; }
      public TypeResolverDelegate TypeResolver { get; set; }
      public DataTransformResolverDelegate TransformResolver { get; set; }

      /// <summary>
      /// List of all namespaces
      /// </summary>
      public List<NamespaceInfo> Namespaces { get; } = 
         new List<NamespaceInfo>();

      public NamespaceInfo Namespace { get; set; }

      public XmlSchema Instance { get; } = new XmlSchema();

      public XsdSchema()
      {

      }

      #region -- Helper Methods

      /// <summary>
      /// Get Schema Type using given IDataElement details.
      /// </summary>
      /// <param name="element">element details</param>
      /// <param name="ns">namespace URI</param>
      /// <returns>instance of XmlQualifiedName is returned</returns>
      public XmlQualifiedName GetQualifiedName(
         String typeName, String ns = null)
      {
         XmlQualifiedName t = new XmlQualifiedName(
            typeName ?? XsdHelper.DEFAULT_TYPE_NAME, 
            ns ?? XsdHelper.XSD_NAMESPACE);
         return t;
      }

      public object FindItem(string itemName)
      {
         if (m_Registry.TryGetValue(itemName, out var registry))
         {
            return registry;
         }
         return null;
      }

      #endregion
      #region -- Manage Annotations

      public static XmlSchemaAnnotation GetAnnotation(
         String description, String resourceId)
      {
         // TODO: remove hardcoded label...
         String srcText = String.IsNullOrWhiteSpace(resourceId) ? String.Empty :
            "http://" + resourceId;
         XmlSchemaAnnotation a = new XmlSchemaAnnotation();
         XmlSchemaDocumentation d = new XmlSchemaDocumentation();
         //{
         //   Source = srcText
         //};
         a.Items.Add(d);
         d.Markup = TextToNodeArray(description ?? String.Empty);
         return a;
      }

      public static XmlNode[] TextToNodeArray(string text)
      {
         XmlDocument doc = new XmlDocument();
         return new XmlNode[1] { doc.CreateTextNode(text) };
      }

      #endregion
      #region -- Manage Elements

      private XmlQualifiedName GetType(
         QualifiedNameInfo name, DataTypeInfo dataType, NamespaceInfo ns)
      {
         XmlQualifiedName qname = null;
         if (DataTextMapResolver != null)
         {
            var txtMap = DataTextMapResolver(name.Name) as DataTextElementInfo;
            if (txtMap != null)
            {
               qname = GetQualifiedName(txtMap.BaseType.TypeName,
                  Xsd.XsdHelper.GetNamespace(
                     txtMap.BaseType.NamespacePrefix, ns));
            }
         }
         if (qname == null)
         {
            var fns = Namespaces.Find((x) => x.Prefix == name.Prefix);
            if (fns == null && (name.Prefix == NamespaceInfo.W3C_PREFIX || 
               name.Prefix == NamespaceInfo.W3C_PREFIX_XSD))
            {
               fns = new NamespaceInfo(name.Prefix, NamespaceInfo.W3C_URI);
            }
            string uriText = fns == null ? ns.Uri.OriginalString :
               fns.Uri.OriginalString;
            qname = GetQualifiedName(name.OriginalName,
               dataType.ElementType == ElementBaseType.Base ?
                  null : uriText);
         }

         return qname;
      }

      /// <summary>
      /// Prepare an Element.
      /// </summary>
      /// <param name="element">element details</param>
      /// <param name="referenceName">reference name; if not provided then the
      /// element will be added inline and not as a reference</param>
      /// <param name="ns">element namespace</param>
      /// <param name="isElementDeclaration">true if it is an element 
      /// declaration</param>
      /// <returns>instance of XmlSchemaElement is returned</returns>
      public XsdItem PrepareElement(
         IDataElement element, String referenceName = null, 
         NamespaceInfo ns = null, bool isElementDeclaration = true)
      {
         XmlSchemaElement e = new XmlSchemaElement();

         if (!isElementDeclaration)
         {
            if (element.MinOccurrence.HasValue)
            {
               e.MinOccurs = element.MinOccurrence.Value;
            }
            if (element.MaxOccurrence.HasValue && 
               element.MaxOccurrence.Value > 0)
            {
               e.MaxOccursString = element.MaxOccurrence.Value >= 2147483647 ?
                  "unbounded" : element.MaxOccurrence.Value.ToString();
            }
         }

         //var r = BaseTypeResolver == null ? new DataTypeInfo(element) :
         //   BaseTypeResolver(element);
         var r = new DataTypeInfo(element);
         if (!String.IsNullOrEmpty(referenceName))
         {
            e.RefName = new XmlQualifiedName(
               referenceName, ns.Uri.OriginalString);
         }
         else
         {
            e.Name = element.ElementQualifiedName.OriginalName;

            //e.SchemaTypeName = GetQualifiedName(
            //   element.TypeQualifiedName.OriginalName, 
            //   r.ElementType == ElementBaseType.Base ? 
            //      null : ns.Uri.OriginalString);
            e.SchemaTypeName = GetType(element.TypeQualifiedName, r, ns);

            if (!String.IsNullOrWhiteSpace(element.AnnotationText) && 
               element.AnnotationText.Replace(" ","") != 
               element.ElementQualifiedName.OriginalName)
            {
               e.Annotation = GetAnnotation(
                  element.AnnotationText, element.ResourceId);
            }
         }

         XsdItem item = new XsdItem
            {
               ItemName = element.ElementName,
               UriPrefix = element.ElementQualifiedName.Prefix,
               Element = e
            };
         return item;
      }

      /// <summary>
      /// Add Element given an IDataElement.
      /// </summary>
      /// <param name="element">element details</param>
      /// <param name="referenceName">reference name; if not provided then the
      /// element will be added inline and not as a reference</param>
      /// <param name="ns">element namespace</param>
      public void AddElement(
         IDataElement element, String referenceName = null, 
         NamespaceInfo ns = null, bool isElementDeclaration = true)
      {
         XsdItem item = PrepareElement(
            element, referenceName, ns, isElementDeclaration);
         var i = Items.Find((x) => (x.ItemName == item.ItemName &&
                                    x.UriPrefix == item.UriPrefix));
         if (i == null)
         {
            Instance.Items.Add(item.Element);
            Items.Add(item);
         }
      }

      #endregion
      #region -- Manage Complex Types

      /// <summary>
      /// Prepare Complex Type.
      /// </summary>
      /// <param name="type"></param>
      /// <param name="element"></param>
      /// <param name="children"></param>
      public XmlSchemaComplexType PrepareComplexType(
         DataTypeInfo type, IDataElement element, 
         List<AssetDataElement> children)
      {

         if (!m_Registry.TryGetValue(element.ElementName, out var citem))
         {
            citem = new List<string>();
            m_Registry.Add(element.ElementName, citem);
         }

         String baseTypeName = element.ElementDataType;
         String description = element.Description;
         NamespaceInfo ns = type.Namespace;
         ElementBaseTypeInfo eTypeInfo = 
            ElementBaseTypeInfo.GetElementTypeInfo(
               ElementType.element.ToString(), BaseGrammarType.Unknown);

         // TODO: figure how to specify Base Grammar...
         Boolean isSimpleType = ElementBaseInfo.IsBaseElementType(
               element, BaseGrammarType.Unknown);
         Boolean isObject = ElementBaseInfo.IsBaseObjectType(
            element, BaseGrammarType.Unknown);

         XmlSchemaComplexType t = new XmlSchemaComplexType
         {
            //Name = isSimpleType ? element.ElementQualifiedName.OriginalName : 
            //   element.TypeQualifiedName.OriginalName
            Name = element.ElementQualifiedName.OriginalName
         };

         t.Namespaces.Add(ns.Prefix, ns.UriText);

         if (!String.IsNullOrWhiteSpace(description))
         {
            t.Annotation = GetAnnotation(description, element.ResourceId);
         }

         // is this a simple type?  If so create a simple context...
         if (isSimpleType && !isObject)
         {
            XmlSchemaSimpleContent sc = new XmlSchemaSimpleContent();

            XmlSchemaSimpleContentExtension scextension =
               new XmlSchemaSimpleContentExtension
               {
                  BaseTypeName = GetQualifiedName(element.ElementDataType)
               };
            sc.Content = scextension;
            t.ContentModel = sc;
            return t;
         }

         // (complex type) particle
         XmlSchemaParticle particle;

         // choice particle
         if (eTypeInfo.Group == ElementGroup.OptionOne)
         {
            XmlSchemaChoice choice = new XmlSchemaChoice();
            particle = choice;
            foreach (var i in children)
            {
               var uri = NamespaceResolver == null ?
                  new NamespaceInfo("", "") :
                  NamespaceResolver(i.NamespaceText);
               AddNamespace(uri);

               var item = PrepareElement(i, i.ElementName, uri, false);
               choice.Items.Add(item.Element);
            }
         }

         // sequence particle [default]
         else
         {
            XmlSchemaSequence sequence = new XmlSchemaSequence();
            particle = sequence;
            foreach (var i in children)
            {
               // if visited continue ...
               var iItem = citem.Find((x) => x == i.ElementName);
               if (iItem != null)
               {
                  continue;
               }

               // prepare child element
               var uri = NamespaceResolver == null ?
                  new NamespaceInfo("", "") :
                  NamespaceResolver(i.NamespaceText);
               AddNamespace(uri);

               var item = PrepareElement(
                  i, i.ElementQualifiedName.OriginalName, uri, false);
               sequence.Items.Add(item.Element);

               // remember visited this item
               citem.Add(i.ElementName);

               // create corresponding element if it is an element of schema
               if (i.Namespace == Namespace.Uri.OriginalString)
               {
                  AddElement(i, null, ns, true);
               }
            }
         }

         if (isObject)
         {
            particle.Namespaces.Add(ns.Prefix, ns.UriText);
            t.Particle = particle;
            return t;
         }

         // examine the base type and see if we are extending the element
         // (IsBase || IsObject) = NO
         var baseType = BaseTypeResolver(element);
         if (baseType.IsBase || baseType.IsObject)
         {
            t.Particle = particle;
         }

         // complex content and element extension...
         else
         {
            XmlSchemaComplexContent c = new XmlSchemaComplexContent();
            t.ContentModel = c;

            // complex extension
            DataTypeInfo btype = BaseTypeResolver == null ?
               new DataTypeInfo(element) : BaseTypeResolver(element);

            QualifiedNameInfo qname = btype.DataTypeQualifiedName;

            XmlSchemaComplexContentExtension e =
               new XmlSchemaComplexContentExtension
               {
                  BaseTypeName = new XmlQualifiedName(
                     qname.OriginalName,
                     btype.Namespace.Uri.OriginalString)
               };
            c.Content = e;
            e.Particle = particle;
         }

         return t;
      }

      /// <summary>
      /// Add complex type.
      /// </summary>
      /// <param name="type"></param>
      /// <param name="element"></param>
      /// <param name="children"></param>
      public void AddComplextType(
         DataTypeInfo type, IDataElement element,
         List<AssetDataElement> children)
      {
         // first see if element was already registered in schema...


         // go ahead and add new complex type...
         XmlSchemaComplexType e = PrepareComplexType(type, element, children);
         Instance.Items.Add(e);
      }

      #endregion
      #region -- Manage Namespaces

      public void AddNamespace(NamespaceInfo ns)
      {
         if (ns == null)
            return;
         NamespaceInfo i = Namespaces.Find((x) => x.Prefix == ns.Prefix);
         if (i == null)
            Namespaces.Add(ns);
      }

      #endregion

   }

}

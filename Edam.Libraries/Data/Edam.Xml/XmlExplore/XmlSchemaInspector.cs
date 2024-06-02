using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using System.Xml;

// -----------------------------------------------------------------------------
using Edam.Xml.XmlAsset;
using Edam.Data.Asset;
using Edam.Data.AssetManagement;
using Edam.Data.AssetSchema;
using Edam.DataObjects.DataCodes;
using Edam.Data.AssetConsole;
using DocumentFormat.OpenXml.Office2016.Presentation.Command;

namespace Edam.Xml.XmlExplore
{

   public class XmlSchemaInspector
   {

      #region -- Declarations

      private int m_ExtraNamespaceCount = 0;
      private bool m_AttributeUsesIndicator = false;
      private XmlSchemaSet m_SchemaSet;
      private object m_LastVisitedNode = null;
      private List<string> m_VisitedNodes = new List<string>();
      private List<string> m_VisitedElement = new List<string>();

      public AssetData Asset;
      public NamespaceInfo DefaultNamespace { get; set; }

      /// <summary>
      /// When true, while inspecting objects add the collection of all
      /// compiled attributes of the complex-type and its base...
      /// </summary>
      public bool AttributeUsesIndicator
      {
         get { return m_AttributeUsesIndicator; }
         set { m_AttributeUsesIndicator = value; }
      }

      private AssetConsoleArgumentsInfo m_Arguments;
      public AssetConsoleArgumentsInfo Arguments
      {
         get { return m_Arguments; }
      }

      #endregion
      #region -- Constructor / Destructor

      public XmlSchemaInspector(
         AssetConsoleArgumentsInfo arguments, XmlSchemaSet schemaSet)
      {
         if (schemaSet == null || schemaSet.Count == 0)
            throw new Exception(
               "Provided Schema set is empty. Xml cannot be generated.");
         m_SchemaSet = schemaSet;
         schemaSet.ValidationEventHandler +=
            new ValidationEventHandler(ValidationCallBack);
         Asset = new AssetData(arguments.Namespace, AssetType.Schema, 
            arguments.ProjectVersionId);
      }

      #endregion
      #region -- XML Validation support

      private static void ValidationCallBack(
         object sender, ValidationEventArgs args)
      {
         Console.WriteLine("Error in Schema - ");
         Console.WriteLine(args.Message);
      }

      #endregion
      #region -- Manage Namespaces

      public static NamespaceList SetNamespaces(
         NamespaceList namespaces, XmlSchemaSet schemaSet, 
         NamespaceInfo defaultNamespace)
      {
         var list = namespaces ?? new NamespaceList();
         var slist = schemaSet.Schemas();
         foreach (var z in slist)
         {
            var l = ((XmlSchema)z).Namespaces.ToArray();
            foreach (var i in l)
            {
               var nitem = list.Find(
                  (x) => x.Uri.OriginalString == i.Namespace);
               if (nitem != null)
               {
                  continue;
               }

               string pfix = string.IsNullOrWhiteSpace(i.Name) &&
                  defaultNamespace.Uri.OriginalString == i.Namespace ?
                  defaultNamespace.Prefix : i.Name;

               var fitem = list.Find((x) => x.Prefix == pfix);
               if (fitem == null)
               {
                  var e = new NamespaceInfo(pfix, new Uri(i.Namespace));
                  list.Add(e);
               }
            }
         }

         var dns = list.Find((x) => x.Prefix == defaultNamespace.Prefix);
         if (dns == null)
         {
            list.Add(defaultNamespace);
         }

         return list;
      }

      #endregion
      #region -- Annotations support

      public static void AssetSchemaAnnotation(
         XmlSchemaAnnotation annotation, IAssetElement element)
      {
         if (annotation == null || annotation.Items == null)
            return;

         foreach (var i in annotation.Items)
         {
            XmlSchemaDocumentation doc = i as XmlSchemaDocumentation;
            if (doc == null || doc.Markup == null)
               continue;
            for (var c = 0; c < doc.Markup.Length; c++)
            {
               XmlNode node = doc.Markup[c];
               if (node == null)
                  continue;
               if (element.Annotation == null)
                  element.Annotation = new List<string>();
               var txt = 
                  node.InnerText.Replace("\r", "").Replace("\n", "").Trim();
               element.Annotation.Add(txt);
            }
         }
      }

      public static string GetSchemaAnnotation(XmlSchemaAnnotation annotation)
      {
         if (annotation == null || annotation.Items == null)
            return String.Empty;

         System.Text.StringBuilder sb = new System.Text.StringBuilder();
         foreach (var i in annotation.Items)
         {
            XmlSchemaDocumentation doc = i as XmlSchemaDocumentation;
            if (doc == null || doc.Markup == null)
               continue;
            for (var c = 0; c < doc.Markup.Length; c++)
            {
               XmlNode node = doc.Markup[c];
               if (node == null)
                  continue;
               sb.Append(node.InnerText + " ");
            }
         }
         return sb.ToString();
      }

      #endregion
      #region -- Complex/Simple Type, Element and Attribute support

      /// <summary>
      /// Elements as complex-types will not have qualified names, try parent...
      /// </summary>
      /// <param name="type"></param>
      /// <returns></returns>
      private QualifiedNameInfo GetQualifiedName(XmlSchemaSimpleType type)
      {
         QualifiedNameInfo eqname;
         if (type == null)
         {
            eqname = new QualifiedNameInfo(
               DefaultNamespace.Prefix, DefaultNamespace.Uri.OriginalString);
            return eqname;
         }

         // elements as complex-types will not have qualified names, try parent
         if (type.QualifiedName.IsEmpty)
         {
            var e = type.Parent as XmlSchemaElement;
            if (e != null)
            {
               eqname = GetQualifiedName(e.QualifiedName);
            }
            else
            {
               eqname = GetQualifiedName(type.QualifiedName);
            }
         }
         else
         {
            eqname = GetQualifiedName(type.QualifiedName);
         }
         return eqname;
      }

      /// <summary>
      /// Elements as complex-types will not have qualified names, try parent...
      /// </summary>
      /// <param name="type"></param>
      /// <returns></returns>
      private QualifiedNameInfo GetQualifiedName(XmlSchemaComplexType type)
      {
         QualifiedNameInfo eqname;
         if (type == null)
         {
            eqname = new QualifiedNameInfo(
               DefaultNamespace.Prefix, DefaultNamespace.Uri.OriginalString);
            return eqname;
         }

         // elements as complex-types will not have qualified names, try parent
         if (type.QualifiedName.IsEmpty)
         {
            var e = type.Parent as XmlSchemaElement;
            if (e != null)
            {
               eqname = GetQualifiedName(e.QualifiedName);
            }
            else
            {
               eqname = GetQualifiedName(type.QualifiedName);
            }
         }
         else
         {
            eqname = GetQualifiedName(type.QualifiedName);
         }
         return eqname;
      }

      private void CompleteAssetUpdate(AssetDataElement asset)
      {
         NamespaceInfo ns = null;
         if (String.IsNullOrWhiteSpace(asset.Namespace))
         {
            ns = Asset.Namespaces.Find((x) =>
               x.Prefix == asset.ElementQualifiedName.Prefix);
            asset.Namespace = ns == null ? String.Empty : ns.Uri.AbsoluteUri;
         }
         else
         {
            ns = Asset.Namespaces.Find((x) =>
               x.Uri.AbsoluteUri == asset.Namespace);
            if (ns == null)
            {
               ns = new NamespaceInfo(
                  asset.ElementQualifiedName.Prefix, asset.Namespace);
               Asset.Namespaces.Add(ns);
            }
         }

         var nsp = ns == null ? 
            new NamespaceInfo(String.Empty, String.Empty) : ns;
         if (asset.Namespaces == null && ns != null)
         {
            asset.Namespaces = new NamespaceList();
            asset.Namespaces.Add(ns);
         }
         AssetDataElement.CompleteElementUpdate(asset, nsp, null);
      }

      /// <summary>
      /// Add Simple Type
      /// </summary>
      /// <param name="type">simple type instance</param>
      /// <returns>Asset Data Element instance is returned</returns>
      private AssetDataElement AddSimpleType(XmlSchemaSimpleType type)
      {
         m_LastVisitedNode = type;

         QualifiedNameInfo typeQName = null;
         string typeName = AssetDataElement.OBJECT;
         if (type.BaseXmlSchemaType != null)
         {
            typeQName = GetQualifiedName(type.BaseXmlSchemaType.QualifiedName);
            typeName = type.BaseXmlSchemaType.QualifiedName.Name;
         }

         QualifiedNameInfo eqname = GetQualifiedName(type);

         AssetDataElement a = new AssetDataElement
         {
            DataType = (typeName == "anyType") ?
               AssetDataElement.OBJECT : typeQName.Name,
            Occurs = String.Empty,
            Namespace = type.QualifiedName.Namespace,
            EntityQualifiedName = null,
            ElementQualifiedName = eqname,
            ElementType = ElementType.enumerator,
            KeyType = ConstraintType.nonkey,
            AutoGenerateType = ConstraintType.none,
            TypeQualifiedName = typeQName
         };

         AssetSchemaAnnotation(type.Annotation, a);
         a.AddQualifiedTypeName(a.ElementQualifiedName);

         CompleteAssetUpdate(a);

         Asset.Add(a);

         a.Parent = null;

         return a;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="type"></param>
      private AssetDataElement AddComplexType(
         XmlSchemaComplexType type, XmlSchemaComplexType parent)
      {
         m_LastVisitedNode = type;

         QualifiedNameInfo typeQName = null;
         string typeName = AssetDataElement.OBJECT;
         if (type.BaseXmlSchemaType != null)
         {
            typeQName = GetQualifiedName(type.BaseXmlSchemaType.QualifiedName);
            typeName = typeQName.Name; //type.BaseXmlSchemaType.QualifiedName.Name;
         }

         // elements as complex-types will not have qualified names, try parent
         QualifiedNameInfo eqname = GetQualifiedName(type);
         QualifiedNameInfo ename = GetQualifiedName(parent);

         if (typeQName == null && typeName == AssetDataElement.OBJECT)
         {
            return null;
         }

         AssetDataElement a = new AssetDataElement
         {
            DataType = (typeName == "xs:anyType") ?
               AssetDataElement.OBJECT : typeName,
            Occurs = String.Empty,
            Namespace = type.QualifiedName.Namespace,
            EntityQualifiedName = parent == null ? null : ename,
            ElementQualifiedName = eqname,
            ElementType = ElementType.type,
            KeyType = ConstraintType.nonkey,
            AutoGenerateType = ConstraintType.none,
            TypeQualifiedName = typeQName
         };

         string note = Text.Convert.ToProperCase(
            a.ElementQualifiedName.OriginalName).Trim();
         a.AddAnnotation(note);

         AssetSchemaAnnotation(type.Annotation, a);
         a.AddQualifiedTypeName(a.ElementQualifiedName);

         CompleteAssetUpdate(a);

         Asset.Add(a);
         AddAttributes(type, a);

         a.Parent = null;

         return a;
      }

      private void SetElementLength(
         XmlSchemaSimpleType type, AssetDataElement element)
      {
         if (type == null || type.Content == null)
         {
            return;
         }
         int value;
         XmlSchemaSimpleTypeRestriction r = 
            type.Content as XmlSchemaSimpleTypeRestriction;
         if (r == null)
         {
            return;
         }
         foreach(var f in r.Facets)
         {
            XmlSchemaMinLengthFacet mn = f as XmlSchemaMinLengthFacet;
            if (mn != null)
            {
               if (int.TryParse(mn.Value, out value))
               {
                  element.MinLength = value;
               }
            }
            else
            {
               XmlSchemaMaxLengthFacet mx = f as XmlSchemaMaxLengthFacet;
               if (mx != null)
               {
                  if (int.TryParse(mx.Value, out value))
                  {
                     element.MaxLength = value;
                  }
               }
            }
         }
      }

      private AssetDataElement AddVisitedElement(AssetDataElement element)
      {
         // check if the element was added before, if so return null
         string vElement = (element.EntityQualifiedName == null ? String.Empty :
            element.EntityQualifiedName.Name) + "::" +
            element.ElementQualifiedName.Name;
         var visited = m_VisitedElement.Find((x) => x == vElement);
         if (visited != null)
         {
            return null;
         }
         else
         {
            m_VisitedElement.Add(vElement);
         }
         return element;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="element"></param>
      /// <param name="parent"></param>
      /// <param name="isElement">true if is an element and should be process
      /// as a complext type</param>
      /// <returns></returns>
      private AssetDataElement AddElement(
         XmlSchemaElement element, XmlSchemaComplexType parent, 
         bool isElement = false)
      {
         m_LastVisitedNode = element;

         XmlSchemaSimpleType type =
            element.ElementSchemaType as XmlSchemaSimpleType;
         QualifiedNameInfo typeQualifiedName;
         QualifiedNameInfo baseTypeQualifiedName = null;
         XmlSchemaComplexType cmpxType = null;

         if (type != null)
         {
            if (String.IsNullOrWhiteSpace(type.Name))
            {
               if (String.IsNullOrWhiteSpace(
                  element.ElementSchemaType.QualifiedName.Name))
               {
                  typeQualifiedName = GetQualifiedName(
                     element.ElementSchemaType.BaseXmlSchemaType.QualifiedName);
                  baseTypeQualifiedName = typeQualifiedName;
               }
               else
               {
                  typeQualifiedName = GetQualifiedName(
                     element.ElementSchemaType.QualifiedName);
                  baseTypeQualifiedName = typeQualifiedName;
               }
            }
            else
            {
               typeQualifiedName = GetQualifiedName(type.QualifiedName);
               baseTypeQualifiedName = GetQualifiedName(
                  element.ElementSchemaType.BaseXmlSchemaType.QualifiedName);
            }
         }
         else if (element.ElementSchemaType is XmlSchemaComplexType)
         {
            cmpxType = (XmlSchemaComplexType)element.ElementSchemaType;
            if (String.IsNullOrWhiteSpace(cmpxType.Name))
            {
               typeQualifiedName = GetQualifiedName(element.QualifiedName);
            }
            else
            {
               typeQualifiedName = GetQualifiedName(cmpxType.QualifiedName);
            }

            // if complex type was already visited, just add element...
            if ((cmpxType.Name == null ||
               m_VisitedNodes.Find((x)=>x == cmpxType.Name) == null) &&
               cmpxType.Name == null)
            {
               if (cmpxType.Name != null)
                  m_VisitedNodes.Add(cmpxType.Name);
               InspectComplexType(cmpxType, parent);
               return null;
            }
            isElement = true;
         }
         else
         {
            return null;
         }

         // elements as complex-types will not have qualified names, try parent
         QualifiedNameInfo eqname = GetQualifiedName(parent);

         AssetDataElement a = new AssetDataElement
         {
            Namespace = element.QualifiedName.Namespace,
            DataType = typeQualifiedName.Name,
            TypeQualifiedName = typeQualifiedName,
            MinOccurrence = (int)element.MinOccurs,
            MaxOccurrence = (int)
            OccuranceInfo.GetInt32MaxOccurence(element.MaxOccurs),
            EntityQualifiedName = parent == null ? null : eqname,
            ElementQualifiedName = GetQualifiedName(element.QualifiedName),
            KeyType = ConstraintType.nonkey,
            AutoGenerateType = ConstraintType.none,
            IsNillable = element.IsNillable,
            ElementType = ElementType.element
         };

         // check if the element was added before, if so return null
         if (AddVisitedElement(a) == null)
         {
            return null;
         }

         // examine type...
         if (typeQualifiedName.OriginalName == DataElement.BASE_KEY)
         {
            a.KeyType = ConstraintType.key;
         }
         else if (typeQualifiedName.OriginalName == DataElement.BASE_AUTO_PK)
         {
            a.KeyType = ConstraintType.key;
            a.AutoGenerateType = ConstraintType.autoGenerate;
            typeQualifiedName = new QualifiedNameInfo(
               "xs", ElementBaseTypeInfo.INTEGER);
            a.DataType = typeQualifiedName.Name;
         }
         else if (typeQualifiedName.OriginalName == DataElement.BASE_IDENTITY)
         {
            a.AutoGenerateType = ConstraintType.autoGenerate;
            typeQualifiedName = new QualifiedNameInfo(
               "xs", ElementBaseTypeInfo.INTEGER);
            a.DataType = typeQualifiedName.Name;
         }

         a.AddQualifiedTypeName(a.ElementQualifiedName);
         SetElementLength(type, a);

         if (baseTypeQualifiedName != null &&
            typeQualifiedName.OriginalName != DataElement.BASE_CHAR)
         {
            a.DataType = baseTypeQualifiedName.Name;
            a.TypeQualifiedName = baseTypeQualifiedName;
         }

         AssetSchemaAnnotation(element.Annotation, a);

         CompleteAssetUpdate(a);

         if (!isElement && cmpxType != null)
            AddAttributes(cmpxType, a);

         Asset.Add(a);
         return a;
      }

      /// <summary>
      /// Get information about the base type of a simple type.  Supports for
      /// union types.
      /// </summary>
      /// <param name="type">simple type instance</param>
      /// <param name="name">qualified name details</param>
      private void UpdateBaseType(
         XmlSchemaSimpleType type, QualifiedNameInfo name)
      {
         if (type.Datatype is System.Xml.Schema.XmlSchemaDatatype dtype)
         {
            if (dtype.Variety == XmlSchemaDatatypeVariety.Union &&
               String.IsNullOrWhiteSpace(name.OriginalName))
            {
               name.OriginalName = "[]";
            }
            else
            {
               return;
            }
         }
         else
         {
            return;
         }

         System.Text.StringBuilder sb = new System.Text.StringBuilder();
         var t = type.BaseXmlSchemaType as XmlSchemaSimpleType;
         var c = t.Content as XmlSchemaSimpleTypeUnion;
         int cnt = 0;
         if (c != null)
         {
            foreach(var a in c.BaseMemberTypes)
            {
               if (cnt != 0)
               {
                  sb.Append(";");
               }
               sb.Append(a.Datatype.TypeCode.ToString());
               cnt++;
            }
            string types = sb.ToString();
            if (!String.IsNullOrWhiteSpace(types))
            {
               name.OriginalName = "[" + types + "]";
            }
         }
      }

      private QualifiedNameInfo GetQualifiedName(XmlSchemaType type)
      {
         if (type.Datatype == null || type.Datatype.ValueType == null)
         {
            return GetQualifiedName(type.QualifiedName);
         }
         return new QualifiedNameInfo("xs",
            type.Datatype.ValueType.Name.ToLower());
      }

      /// <summary>
      /// Add attribute.
      /// </summary>
      /// <param name="attribute">attribute to add</param>
      /// <param name="parent">parent data element</param>
      /// <returns>instance of asset data element is returned</returns>
      private AssetDataElement AddAttribute(
         XmlSchemaAttribute attribute, AssetDataElement parent)
      {
         QualifiedNameInfo qname = null;

         XmlSchemaSimpleType type =
            attribute.AttributeSchemaType as XmlSchemaSimpleType;
         XmlSchemaType btype = type.BaseXmlSchemaType as XmlSchemaType;
         QualifiedNameInfo baseTypeQualifiedName = btype == null ? null :
            GetQualifiedName(btype);

         if (attribute.AttributeSchemaType.QualifiedName.IsEmpty)
         {
            qname = new QualifiedNameInfo(
               attribute.AttributeSchemaType.TypeCode.ToString());
         }
         else
         {
            qname = GetQualifiedName(
               attribute.AttributeSchemaType.QualifiedName);
         }

         UpdateBaseType(type, baseTypeQualifiedName);

         var qns = String.IsNullOrWhiteSpace(
            attribute.QualifiedName.Namespace) ? parent.Namespace :
            attribute.QualifiedName.Namespace;

         AssetDataElement a = new AssetDataElement
         {
            DataType = qname.Name,
            Occurs = XmlDataAsset.GetOccurance(attribute.Use),
            Namespace = qns,
            EntityQualifiedName = parent == null ? null :
            new QualifiedNameInfo(parent.ElementName),
            ElementQualifiedName =
               GetQualifiedName(attribute.QualifiedName),
            ElementType = ElementType.attribute,
            KeyType = ConstraintType.nonkey,
            AutoGenerateType = ConstraintType.none,
            TypeQualifiedName = qname
         };

         if (qname.OriginalName == DataElement.BASE_KEY)
         {
            a.KeyType = ConstraintType.key;
         }

         if (baseTypeQualifiedName != null &&
            qname.OriginalName != DataElement.BASE_CHAR)
         {
            a.DataType = baseTypeQualifiedName.Name;
            a.TypeQualifiedName = baseTypeQualifiedName;
         }

         a.AddQualifiedTypeName(a.ElementQualifiedName);
         SetElementLength(type, a);

         if (AddVisitedElement(a) == null)
         {
            return null;
         }

         AssetSchemaAnnotation(attribute.Annotation, a);

         CompleteAssetUpdate(a);

         Asset.Add(a);
         a.Parent = parent;
         return a;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="element"></param>
      /// <param name="asset"></param>
      private void AddAttributes(
         XmlSchemaComplexType element, AssetDataElement asset)
      {
         foreach (object e in element.Attributes)
         {
            if (e.GetType() == typeof(XmlSchemaAttribute))
            {
               AddAttribute(e as XmlSchemaAttribute, asset);
            }
            else
            {

            }
         }
      }

      #endregion
      #region -- Schema to DataElement Registration

      /// <summary>
      /// 
      /// </summary>
      /// <param name="qname"></param>
      /// <param name="isAttribute">true if it is an attribute</param>
      /// <returns></returns>
      private QualifiedNameInfo GetQualifiedName(XmlQualifiedName qname)
      {
         string nspace = (string.IsNullOrWhiteSpace(qname.Namespace) &&
            DefaultNamespace != null) ?
            DefaultNamespace.Uri.AbsoluteUri : qname.Namespace;
         string prefix = DefaultNamespace != null ?
            DefaultNamespace.Prefix : string.Empty;

         NamespaceInfo ns = Asset.Namespaces.Find(
            (x) => x.Uri.OriginalString == qname.Namespace);
         if (ns == null)
         {
            prefix = "ns" + (m_ExtraNamespaceCount++).ToString();
            ns = new NamespaceInfo(prefix, nspace);
         }

         QualifiedNameInfo qn = new QualifiedNameInfo(String.Empty)
         {
            OriginalName = qname.Namespace + ":" + qname.Name,
            Name = qname.Name,
            Prefix = string.IsNullOrWhiteSpace(ns.Prefix) ? prefix : ns.Prefix
         };

         return qn;
      }

      public bool IsEmptyParticle(XmlSchemaComplexType type)
      {
         return type.ContentTypeParticle != null &&
            type.ContentType == XmlSchemaContentType.Empty;
      }

      /// <summary>
      /// Add all choice elements...
      /// </summary>
      /// <param name="choice"></param>
      /// <param name="parent"></param>
      /// <param name="asset"></param>
      private void InspectChoices(
         XmlSchemaChoice choice, XmlSchemaComplexType parent, 
         AssetDataElement asset, ElementSequence elementSequence)
      {
         if (choice.Items.Count == 0)
         {
            return;
         }

         elementSequence.GroupNo++;
         var eseq = new ElementSequence(
            elementSequence, ElementGroup.Choise);
         eseq.Occurance = new OccuranceInfo(
            choice.MinOccurs, choice.MaxOccurs);
         eseq.Comment = GetSchemaAnnotation(choice.Annotation);

         foreach (var c in choice.Items)
         {
            if (c is XmlSchemaElement element)
            {
               var child = AddElement(element, parent);
               if (child != null)
               {
                  child.Parent = asset;
                  child.ElementSequence = eseq;
               }
            }
            else
            {
               continue;
            }
         }
      }

      /// <summary>
      /// Inspect Simple Type.
      /// </summary>
      /// <param name="simpleType">simple type to inspect</param>
      private void InspectSimpleType(XmlSchemaSimpleType simpleType)
      {
         if (simpleType == null)
         {
            return;
         }

         if (simpleType is XmlSchemaSimpleType)
         {
            AssetDataElement type = AddSimpleType(simpleType);
            if (m_VisitedNodes.Find((x) => x ==
               type.ElementQualifiedName.Name) == null)
            {
               m_VisitedNodes.Add(type.ElementQualifiedName.Name);
            }

            if (simpleType.Content == null)
            {
               return;
            }

            // TODO: add enumerator key/values here
            var facets = ((XmlSchemaSimpleTypeRestriction)
               simpleType.Content).Facets;
            if (facets == null)
            {
               return;
            }

            List<DataCodeInfo> codes = new List<DataCodeInfo>();
            foreach(XmlSchemaEnumerationFacet i in facets)
            {
               DataCodeInfo c = new DataCodeInfo
               {
                  CodeId = i.Value,
                  Description = GetSchemaAnnotation(i.Annotation)
               };
               codes.Add(c);
            }
            type.EnumCodeSetAsJsonText = DataCodeHelper.ToJson(codes);
         }
      }

      /// <summary>
      /// Given a complex-type inspect it.
      /// </summary>
      /// <param name="complexType"></param>
      private void InspectComplexType(
         XmlSchemaComplexType complexType, XmlSchemaComplexType parent)
      {
         ElementSequence elementSequence = new ElementSequence();
         if (complexType is XmlSchemaComplexType)
         {
            var type = AddComplexType(complexType, parent);
            if (type == null)
            {
               return;
            }

            if (m_VisitedNodes.Find((x) => x == 
               type.ElementQualifiedName.Name) == null)
            {
               m_VisitedNodes.Add(type.ElementQualifiedName.Name);
            }
            else
            {
               return;
            }

            // Collection of all compiled attributes of complex-type and base...
            if (AttributeUsesIndicator && complexType.AttributeUses != null)
            {
               foreach (XmlSchemaAttribute att 
                  in complexType.AttributeUses.Values)
               {
                  AddAttribute(att, type);
               }
            }

            if (IsEmptyParticle(complexType))
               return;

            if (complexType.ContentTypeParticle is XmlSchemaChoice choice)
            {
               InspectChoices(choice, complexType, type, elementSequence);
               return;
            }
            else if (!(complexType.ContentTypeParticle is XmlSchemaSequence))
               return;

            foreach (object i in
               ((XmlSchemaSequence)complexType.ContentTypeParticle).Items)
            {
               elementSequence.SequenceNo++;
               if (i is XmlSchemaElement element)
               {
                  if (element.Parent.Parent 
                     is XmlSchemaComplexContentExtension cextension)
                  {
                     if (cextension.BaseTypeName.Name != 
                        complexType.BaseXmlSchemaType.Name)
                     {
                        continue;
                     }
                  }
                  //else
                  //{
                  //   continue;
                  //}

                  var child = AddElement(element, complexType);
                  if (child != null)
                  {
                     child.Parent = type;
                     child.ElementSequence =
                        new ElementSequence(elementSequence.SequenceNo);
                  }
               }
               else if (i is XmlSchemaChoice choices)
               {
                  var clist = i as XmlSchemaChoice;
                  if (clist.Parent.Parent
                     is XmlSchemaComplexContentExtension cextension)
                  {
                     if (cextension.BaseTypeName.Name !=
                        complexType.BaseXmlSchemaType.Name)
                     {
                        continue;
                     }
                  }
                  InspectChoices(choices, complexType, type, elementSequence);
               }
               else
               {
                  continue;
               }
            }
         }
      }

      /// <summary>
      /// Inspect all Complexy Types
      /// </summary>
      private void InspectComplexTypes()
      {
         int cnt = 0;
         // go through all registered types, attributes, and elements
         foreach (object t in m_SchemaSet.GlobalTypes.Values)
         {
            if (t is XmlSchemaComplexType)
            {
               InspectComplexType(t as XmlSchemaComplexType, null);
            }
            else if (t is XmlSchemaSimpleType)
            {
               InspectSimpleType(t as XmlSchemaSimpleType);
            }
            else
            {

            }
            cnt++;
         }
      }

      /// <summary>
      /// Inspect complext types, attributes and elements.
      /// </summary>
      public void Inspect()
      {
         if (Asset.DefaultNamespace == null)
         {
            Asset.SetDefaultNamespace(DefaultNamespace);
         }
         Asset.Namespaces =
            XmlSchemaInspector.SetNamespaces(
               null, m_SchemaSet, DefaultNamespace);
         m_SchemaSet.Compile();

         try
         {
            InspectComplexTypes();
         }
         catch (Exception ex)
         {
            System.Console.WriteLine(ex.Message);
         }

         foreach (XmlSchemaAttribute a in m_SchemaSet.GlobalAttributes.Values)
         {
            AddAttribute(a, null);
         }
         foreach (XmlSchemaElement e in m_SchemaSet.GlobalElements.Values)
         {
            //System.Console.WriteLine(e.Name);
            AddElement(e, null, true);
         }
      }

      #endregion

   }

}

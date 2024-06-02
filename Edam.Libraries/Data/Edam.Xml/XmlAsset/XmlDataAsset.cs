using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;

// -----------------------------------------------------------------------------
using Edam.Xml.XmlExplore;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;

namespace Edam.Xml.XmlAsset
{

   public class XmlDataAsset : AssetData
   {
      //private static readonly string CLASS_NAME = "XmlDataAsset";
      //private static readonly string COMMA = ",";
      //private static readonly string SEMICOLUMN = ";";

      //public string RootTargetNamespace { get; set; }
      //public List<ElementInfo> Items = new List<ElementInfo>();
      //public List<NamespaceInfo> Namespaces = new List<NamespaceInfo>();

      public void SetNamespace(XmlElementInfo element)
      {
         element.Namespaces = new NamespaceList(Namespaces);
         if (String.IsNullOrWhiteSpace(element.Namespace))
         {
            element.Namespace = DefaultNamespace.Uri.OriginalString;
         }
      }

      public XmlDataAsset(AssetDataElementList list) : base(list)
      {

      }
      public XmlDataAsset(NamespaceInfo ns, AssetType type, 
         string versionId) : base(ns, type, versionId)
      {

      }

      #region -- Instance Element Support

      public XmlElementInfo AddElement(InstanceElement element)
      {
         XmlElementInfo e = new XmlElementInfo
         {
            IsMixed = false,
            ElementInstance = element,
            ElementQualifiedName =
               new QualifiedNameInfo(element.QualifiedName.Name),
            Namespace = element.QualifiedName.Namespace,
            Occurs = String.Empty,
            ElementType = ElementType.element
         };
         SetNamespace(e);
         if (element.ValueGenerator?.Datatype == null)
            e.DataType = "object";
         else
            e.DataType = element.ValueGenerator?.Datatype.ValueType.Name;
         Items.Add(e);
         return e;
      }

      public static string GetOccurance(XmlSchemaUse use)
      {
         String occurs;
         switch (use)
         {
            case XmlSchemaUse.None:
               occurs = "(0:1)";
               break;
            case XmlSchemaUse.Optional:
               occurs = "(0:1)";
               break;
            case XmlSchemaUse.Prohibited:
               occurs = "(0:0)";
               break;
            case XmlSchemaUse.Required:
               occurs = "(1:1)";
               break;
            default:
               occurs = "(0:1)";
               break;
         }
         return occurs;
      }

      public XmlAttributeInfo AddAttribute(
         XmlElementInfo element, InstanceAttribute attribute)
      {
         XmlAttributeInfo a = new XmlAttributeInfo
         {
            Instance = attribute,
            ElementQualifiedName =
               new QualifiedNameInfo(attribute.QualifiedName.Name),
            Namespace = attribute.QualifiedName.Namespace,
            ElementType = ElementType.attribute
         };
         a.DataType = attribute.ValueGenerator?.Datatype.ValueType.Name;
         a.Occurs = GetOccurance(attribute.AttrUse);

         element.Attributes.Add(a);
         return a;
      }

      #endregion

   }

}

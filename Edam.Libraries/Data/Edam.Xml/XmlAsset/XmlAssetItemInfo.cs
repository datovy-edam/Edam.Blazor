using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

// -----------------------------------------------------------------------------
using Edam.Data.AssetManagement;
using Edam.Data.AssetSchema;
using Edam.Data.Asset;

namespace Edam.Xml.XmlAsset
{

   public class XmlAssetItemInfo
   {
      public const string XMLNS = "xmlns";

      private NameTable m_NameTable;
      private XmlNamespaceManager m_NameManager;
      private NamespaceInfo m_DefaultNamespace;

      private List<AssetDataElement> m_Assets;
      private Stack<string> m_PathStack = new Stack<string>();
      private int m_SequenceNo = 0;

      public List<AssetDataElement> Items
      {
         get { return m_Assets; }
      }

      public XmlAssetItemInfo(List<AssetDataElement> items,
         NamespaceInfo defaultNamespace)
      {
         m_NameTable = new NameTable();
         m_NameManager = new XmlNamespaceManager(m_NameTable);
         m_Assets = items ?? new List<AssetDataElement>();
         m_DefaultNamespace = defaultNamespace;
      }

      private string GetFullPath(string[] items, bool includePrefix = true)
      {
         StringBuilder sb = new StringBuilder();
         for (var i = items.Length - 1; i >= 0; i--)
         {
            sb.Append(sb.Length == 0 ? String.Empty : "/");
            sb.Append(
               AssetDataElement.GetTypeFromPathItem(items[i], includePrefix));
         }
         return sb.ToString();
      }

      public void Push(XmlNode node)
      {
         m_PathStack.Push(node.Name);
      }

      public void Pop()
      {
         m_PathStack.Pop();
      }

      private void SetupDataElement(
         AssetDataElement element, string prefix, bool isLeafNode,
         AssetDataElement parent)
      {
         var stck = m_PathStack.ToArray();

         var type = AssetDataElement.GetTypeFromPathItem(
            stck[stck.Length - 1], false);
         AssetDataElement.ToDataElement(element, prefix, isLeafNode, type);

         if (m_Assets.Count == 0)
         {
            // TODO: please put this in MACROS...
            element.Type = String.Empty;
            element.KeyType = ConstraintType.nonkey;
            element.AutoGenerateType = ConstraintType.none;
            element.ElementDataType = "object";
            element.ElementType = ElementType.root;
         }
         else if (!isLeafNode)
         {
            element.KeyType = ConstraintType.nonkey;
            element.AutoGenerateType = ConstraintType.none;
            element.ElementDataType = "object";
            element.ElementType = ElementType.type;
         }

         if (m_NameManager.LookupNamespace(prefix) == null)
         {
            m_NameManager.AddNamespace(prefix, element.ElementUri);
         }

         var fpath = GetFullPath(stck, true);
         element.ElementPath = fpath;
         //element.ElementName = parent == null ||
         //   parent.ElementQualifiedName == null ?
         //      String.Empty : parent.ElementQualifiedName.Name;

         // setup processing sequence id / no.
         element.SequenceId = m_SequenceNo.ToString();
         m_SequenceNo++;

         element.Namespaces = new NamespaceList();
         element.Namespaces.Add(prefix, element.Namespace);

         m_Assets.Add(element);
      }

      private string GetQualifiedName(string name, string uriText)
      {
         string[] l = name.Split(':');
         if (l.Length > 1)
         {
            return name;
         }
         if (m_DefaultNamespace.Uri.AbsoluteUri == uriText)
         {
            return m_DefaultNamespace.Prefix + ":" + name;
         }
         return name;
      }

      public AssetDataElement ToDataElement(
         XmlNode node, AssetDataElement parent)
      {
         if (node == null || node.NodeType != XmlNodeType.Element)
            return null;

         string qname = GetQualifiedName(node.Name, node.NamespaceURI);

         AssetDataElement e = new AssetDataElement
         {
            KeyType = ConstraintType.nonkey,
            AutoGenerateType = ConstraintType.none,
            ElementUri = node.NamespaceURI,
            ElementType = ElementType.element,
            Description = Text.Convert.ToTitleCase(node.LocalName),
            SampleValue = node.Value,
            EntityQualifiedName = parent == null ||
               parent.ElementQualifiedName == null ? null :
                  new QualifiedNameInfo(parent.ElementQualifiedName.Name),
            ElementQualifiedName = new QualifiedNameInfo(qname)
         };

         bool isLeafNode = (node.HasChildNodes && node.ChildNodes.Count == 1 &&
            node.ChildNodes[0].NodeType == XmlNodeType.Text) ||
            (!node.HasChildNodes && String.IsNullOrWhiteSpace(node.InnerText));

         SetupDataElement(e, node.Prefix, isLeafNode, parent);
         return e;
      }

      public AssetDataElement ToDataElement(
         XmlAttribute attribute, AssetDataElement parent)
      {
         if (attribute.Prefix == XMLNS)
         {
            m_NameManager.AddNamespace(attribute.LocalName, attribute.Value);
            return null;
         }

         string nsUri = String.IsNullOrWhiteSpace(attribute.NamespaceURI) ?
            m_DefaultNamespace.Uri.AbsoluteUri : attribute.NamespaceURI;
         string qname = GetQualifiedName(attribute.Name, nsUri);

         AssetDataElement e = new AssetDataElement
         {
            KeyType = ConstraintType.nonkey,
            AutoGenerateType = ConstraintType.none,
            ElementUri = nsUri,
            ElementType = ElementType.attribute,
            Description = Text.Convert.ToTitleCase(attribute.LocalName),
            SampleValue = attribute.Value,
            EntityQualifiedName = parent == null ||
               parent.ElementQualifiedName == null ? null :
                  new QualifiedNameInfo(parent.ElementQualifiedName.Name),
            ElementQualifiedName = new QualifiedNameInfo(qname)
         };

         SetupDataElement(e, attribute.Prefix, true, parent);

         return e;
      }

   }

}

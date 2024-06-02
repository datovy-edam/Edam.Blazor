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

   public class XmlElementInfo : AssetElementInfo<InstanceElement>, IAssetElement
   {

      public InstanceElement ElementInstance
      {
         get { return m_ElementInstance; }
         set { m_ElementInstance = value; }
      }
      public XmlSchemaElement SequenceInstance { get; set; }
      public new List<XmlAttributeInfo> Attributes { get; set; }

      public XmlElementInfo() : base()
      {
         Attributes = new List<XmlAttributeInfo>();
      }

      private String GetElementName(XmlQualifiedName qname)
      {

         return GetPrefix(qname.Namespace) + ":" + qname.Name;
      }

      public void SetAttributes()
      {
         List<AssetDataElement> l = new List<AssetDataElement>();
         foreach(var a in Attributes)
         {
            l.Add(a);
         }
         base.Attributes = l;
      }

      private String UpdateFullPath()
      {
         NamespaceInfo d = AssetDataElement.GetDefaultNamespace(Namespaces);
         if (d == null)
            return m_FullPath;
         if (String.IsNullOrWhiteSpace(d.Prefix))
            return m_FullPath;
         return m_FullPath.Replace("/:", "/" + d.Prefix + ":");
      }

      public new String GetFullPath()
      {
         if (ElementInstance == null)
            return null;
         if (!String.IsNullOrEmpty(m_FullPath))
            return m_FullPath;

         System.Text.StringBuilder sb = new StringBuilder();
         Stack<XmlQualifiedName> stck = new Stack<XmlQualifiedName>();
         XmlQualifiedName item;

         InstanceGroup inst = ElementInstance.Parent;
         while (inst != null)
         {
            if (inst.QualifiedName != null)
               stck.Push(inst.QualifiedName);
            inst = inst.Parent;
         }

         if (stck.Count == 0)
         {
            m_FullPath = GetElementName(ElementInstance.QualifiedName);
            m_FullPath = UpdateFullPath();
            return m_FullPath;
         }

         int cnt = 0;
         while (true)
         {
            if (stck.Count == 0)
               break;
            item = stck.Pop();
            if (item == null)
               break;
            if (cnt > 0)
               sb.Append("/");
            sb.Append(GetElementName(item));
            cnt++;
         }
         m_FullPath =
            sb.ToString() + "/" + GetElementName(ElementInstance.QualifiedName);
         m_FullPath = UpdateFullPath();
         return m_FullPath;
      }
   }

}

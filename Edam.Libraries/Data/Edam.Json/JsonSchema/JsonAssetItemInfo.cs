using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Newtonsoft.Json.Linq;

namespace Edam.Json.JsonSchema
{

    public class JsonAssetItemInfo
    {

        private int m_SequenceNo = 0;
        private List<AssetDataElement> m_Assets;
        public AssetData Assets { get; set; }

        public NamespaceInfo DefaultNamespace { get; set; }
        public NamespaceList Namespaces { get; set; }
        public List<JsonComplexType> Definitions { get; set; }
        public List<JsonPropertyInfo> Properties { get; set; }

        public string VesionId { get; set; }

        public List<AssetDataElement> Items
        {
            get { return m_Assets; }
        }

        public JsonAssetItemInfo(NamespaceInfo ns, string versionId)
        {
            Assets = new AssetData(ns, AssetType.Schema, versionId);
            m_Assets = new List<AssetDataElement>();
            Namespaces = new NamespaceList();
            DefaultNamespace = ns;
            VesionId = versionId;
        }

        public void SetDefaultNamespace(NamespaceInfo ns)
        {
            if (DefaultNamespace == null ||
               DefaultNamespace.Uri.OriginalString ==
                  NamespaceInfo.DEFAULT_UNKNOWN_URI)
            {
                DefaultNamespace = ns;
            }
        }

        public JsonPropertyInfo FindProperty(string propertyName)
        {
            if (Properties == null)
                return null;
            return Properties.Find((x) => x.EntityName == propertyName);
        }

        public List<NamespaceInfo> Add(List<NamespaceInfo> list)
        {
            if (list == null)
                return null;
            foreach (var n in list)
            {
                Namespaces.Add(n);
            }
            return list;
        }

        public void Add(NamespaceInfo ns)
        {
            Namespaces.Add(ns);
            SetDefaultNamespace(ns);
        }

        private void SetupDataElement(
           AssetDataElement element, string prefix, bool isLeafNode,
           AssetDataElement parent)
        {
            //var stck = m_PathStack.ToArray();
            //var type = AssetDataElement.GetTypeFromPathItem(
            //   stck[stck.Length - 1], false);
            var type = "string";

            AssetDataElement.ToDataElement(element, prefix, isLeafNode, type);

            if (m_Assets.Count == 0)
            {
                // TODO: please put this in MACROS...
                element.Type = string.Empty;
                element.KeyType = ConstraintType.nonkey;
                element.AutoGenerateType = ConstraintType.autoGenerate;
                element.ElementDataType = "object";
                element.ElementType = ElementType.root;
            }
            else if (!isLeafNode)
            {
                element.KeyType = ConstraintType.nonkey;
                element.AutoGenerateType = ConstraintType.autoGenerate;
                element.ElementDataType = "object";
                element.ElementType = ElementType.type;
            }

            //if (m_NameManager.LookupNamespace(prefix) == null)
            //{
            //   m_NameManager.AddNamespace(prefix, element.ElementUri);
            //}

            var fpath = ""; // GetFullPath(stck, true);
            element.ElementPath = fpath;
            //element.ElementName = parent == null ||
            //   parent.ElementQualifiedName == null ?
            //      String.Empty : parent.ElementQualifiedName.Name;

            // setup processing sequence id / no.
            element.SequenceId = m_SequenceNo.ToString();
            m_SequenceNo++;

            m_Assets.Add(element);
        }

        public AssetDataElement ToDataElement(
           JProperty property, AssetDataElement parent)
        {
            if (property == null)
                return null;

            var ns = DefaultNamespace;

            var childCount = property.Children().Children().Count();

            AssetDataElement e = new AssetDataElement
            {
                KeyType = ConstraintType.nonkey,
                AutoGenerateType = ConstraintType.autoGenerate,
                ElementUri = ns.Uri.AbsoluteUri,
                ElementType = ElementType.element,
                Description = Text.Convert.ToTitleCase(property.Name),
                SampleValue = null,
                EntityQualifiedName = parent == null ||
                  parent.ElementQualifiedName == null ? null :
                     new QualifiedNameInfo(parent.ElementQualifiedName.Name),
                ElementQualifiedName = new QualifiedNameInfo(property.Name)
            };

            bool isLeafNode = childCount <= 1;

            SetupDataElement(e, ns.Prefix, isLeafNode, parent);
            return e;
        }

    }

}

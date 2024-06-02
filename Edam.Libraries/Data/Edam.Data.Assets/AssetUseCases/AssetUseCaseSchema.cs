using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetManagement;
using Edam.Data.AssetManagement.Resources;
using Edam.Data.AssetSchema;

namespace Edam.Data.AssetUseCases
{

    public class AssetUseCaseSchemaItem
    {
        public QualifiedNameInfo ElementQualifiedName { get; set; }
        public QualifiedNameInfo TypeQualifiedName { get; set; }
        public ElementTypeInfo AssetType { get; set; }
        public List<AssetDataElement> ElementChildren { get; set; }

        public string ElementNamespacePrefix
        {
            get { return ElementQualifiedName.Prefix; }
        }
        public string ElementName
        {
            get { return ElementQualifiedName.OriginalName; }
        }

        public string TypeNamespacePrefix
        {
            get { return TypeQualifiedName.Prefix; }
        }
        public string TypeName
        {
            get { return TypeQualifiedName.OriginalName; }
        }

        public void AddChild(AssetDataElement child)
        {
            var c = ElementChildren.Find((x) => x.ElementName == child.ElementName);
            if (c == null)
            {
                child.SequenceId = null;
                ElementChildren.Add(child);
            }
        }
    }

    public class AssetUseCaseSchema
    {
        public NamespaceInfo Namespace { get; set; }
        public Dictionary<string, AssetUseCaseSchemaItem> Registry { get; set; }

        public AssetUseCaseSchema()
        {
            Registry = new Dictionary<string, AssetUseCaseSchemaItem>();
        }

        /// <summary>
        /// Prepare a Schema Item with given element.
        /// </summary>
        /// <param name="element">asset data element to use</param>
        /// <returns>instance of SchemaItem is returned</returns>
        public static SchemaItem GetSchemaItem(AssetDataElement element)
        {
            var eTypeInfo = new ElementTypeInfo
            {
                Element = element,
                Key = element.ElementDataType,
                Children = null
            };

            var sitem = new SchemaItem
            {
                Type = eTypeInfo,
                Children = null
            };

            return sitem;
        }

        #region -- 4.00 - Find / Add Use Case Asset items from/to schema

        /// <summary>
        /// Try to find type.
        /// </summary>
        /// <param name="typeName">type name to find</param>
        /// <returns>if found the instance of AssetUseCaseSchemaItem is returned 
        /// else null</returns>
        public AssetUseCaseSchemaItem Find(string typeName)
        {
            if (Registry.TryGetValue(typeName, out var registryItem))
            {
                return registryItem;
            }
            return null;
        }

        /// <summary>
        /// Add Use Case item to Schema
        /// </summary>
        /// <param name="item">use case schema item to add</param>
        public List<AssetDataElement> Add(AssetUseCaseSchemaItem item)
        {
            // register first time found item
            AssetUseCaseSchemaItem registryItem =
               Find(item.TypeQualifiedName.Name);
            if (registryItem == null)
            {
                Registry.Add(item.TypeQualifiedName.Name, item);
                registryItem = item;
            }
            else
            {
                // update children list adding element not previously registered
                int cnt = 0;
                foreach (var c in item.ElementChildren)
                {
                    var e = registryItem.ElementChildren.Find(
                       (x) => x.ElementName == c.ElementName);
                    if (e == null)
                    {
                        registryItem.ElementChildren.Add(c);
                        cnt++;
                    }
                }
                //if (cnt == 0)
                //{
                //   return new List<AssetDataElement>();
                //}
            }

            // always keep children list in expected order...
            List<AssetDataElement> children = new List<AssetDataElement>();
            List<AssetDataElement> pchildren = item.ElementChildren;
            foreach (var c in registryItem.AssetType.Children)
            {
                var e = pchildren.Find(
                   (x) => x.ElementName == c.ElementName);
                if (e != null)
                {
                    children.Add(e);
                    pchildren.RemoveAll(
                       (x) => x.ElementName == e.ElementName);
                }
            }

            item.ElementChildren = children;
            registryItem.ElementChildren = children;
            return pchildren;
        }

        /// <summary>
        /// Find element Use Case Schema based on the element namespace.
        /// </summary>
        /// <param name="element">element to explore</param>
        /// <param name="parentContext">parrent context</param>
        /// <param name="schemas">list of schemas to search</param>
        /// <returns>if found the schema instance is returned else null</returns>
        public static AssetUseCaseSchema FindSchema(AssetDataElement element,
           IResourceContext parentContext, List<AssetUseCaseSchema> schemas)
        {
            NamespaceInfo ns = parentContext.FindNamespace(
               element.Namespace);
            var schema = schemas.Find((x) =>
               x.Namespace.Uri.AbsoluteUri == ns.Uri.AbsoluteUri);
            return schema;
        }

        #endregion
        #region -- 4.00 - Use Case Asset Schema management...

        public class SchemaItem
        {
            public ElementTypeInfo Type { get; set; }
            public List<AssetDataElement> Children { get; set; }

            public QualifiedNameInfo BaseTypeQualifiedName
            {
                get { return Type.Element.TypeQualifiedName; }
            }
            public string BaseTypeName
            {
                get { return Type.Element.TypeQualifiedName.Name; }
            }
        }

        /// <summary>
        /// Add an item (type definition) to the schema.
        /// </summary>
        /// <param name="context">Use Case context</param>
        /// <param name="typeItem">item to inspect/add</param>
        /// <param name="parentContext">Parent (full) context</param>
        /// <param name="schemas">schema collection</param>
        private static void AddSchemaItem(
           IResourceContext context, SchemaItem typeItem,
           IResourceContext parentContext, List<AssetUseCaseSchema> schemas)
        {
            // add item
            AssetUseCaseSchemaItem item = new AssetUseCaseSchemaItem
            {
                ElementQualifiedName = typeItem.Type.Element.ElementQualifiedName,
                TypeQualifiedName = typeItem.Type.Element.TypeQualifiedName
            };

            string typeName = typeItem.Type.Element.TypeQualifiedName.Name;

            // if this a base type?
            if (ElementBaseTypeInfo.IsBase(typeName))
            {
                return;
            }

            // not a base type, so prepare to register in schema
            var tinfo = parentContext.FindType(typeName);
            if (tinfo != null)
            {
                item.AssetType = tinfo;
            }

            item.ElementChildren = typeItem.Children == null ?
               context.GetTypeChildren(typeItem.Type.Element) : typeItem.Children;

            // find schema and add item, also check types within children
            var schema = FindSchema(
               item.AssetType.Element, parentContext, schemas);

            List<AssetDataElement> pchildren = null;
            if (schema != null)
            {
                pchildren = schema.Add(item);

                // now inspect types within children...
                //foreach (var child in item.ElementChildren)
                //{
                //   var ciSchema = FindSchema(child, parentContext, schemas);
                //   if (ciSchema != null)
                //   {
                //      continue;
                //   }

                //   var sitem = GetSchemaItem(child);
                //   AddSchemaItem(context, sitem, parentContext, schemas);
                //}
            }

            // add item base
            var btinfo = parentContext.FindType(typeName);
            if (btinfo == null)
            {
                return;
            }

            var citem = new SchemaItem
            {
                Type = btinfo,
                Children = pchildren
            };

            AddSchemaItem(context, citem, parentContext, schemas);
        }

        /// <summary>
        /// Prepare Use Case Schema based on given context.
        /// </summary>
        /// <param name="context">data context to use</param>
        /// <param name="ns">URI context</param>
        /// <returns>instance of a AssetUseCaseSchema is returned</returns>
        public static AssetUseCaseSchema PrepareSchema(IResourceContext context,
           NamespaceInfo ns, IResourceContext parentContext,
           List<AssetUseCaseSchema> schemas)
        {
            AssetUseCaseSchema schema = new AssetUseCaseSchema
            {
                Namespace = ns
            };
            schemas.Add(schema);

            // gather Use Case Schema details
            var cntxtTypes = context.GetUriTypes(ns.Uri.AbsoluteUri);
            foreach (var type in cntxtTypes)
            {
                AddSchemaItem(context, GetSchemaItem(type), parentContext, schemas);
            }

            return schema;
        }

        #endregion

    }

}

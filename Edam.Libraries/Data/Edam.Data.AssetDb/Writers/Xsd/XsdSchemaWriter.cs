using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Edam.Xml.Xsd;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Data.AssetManagement;
using Edam.Data.AssetUseCases;

namespace Edam.Data.AssetManagement.Writers.Xsd
{

    public class XsdSchemaWriter
   {

      readonly Resources.ResourceContext m_Context;
      readonly Uri m_Namespace;
      readonly XsdSchema m_Schema = new XsdSchema();
      readonly List<DataTransformItemInfo> m_TransformItems =
         new List<DataTransformItemInfo>();

      public XsdSchema Schema
      {
         get { return m_Schema; }
      }

      public Uri Namespace 
      {
         get { return m_Namespace; }
      }

      public XsdSchemaWriter(
         Resources.ResourceContext context, NamespaceInfo ns)
      {
         m_Context = context;
         m_Namespace = ns.Uri;
         //Schema.Instance.Namespaces.Add(String.Empty, ns.OriginalString);
         m_Schema.Instance.TargetNamespace = ns.Uri.OriginalString;
         m_Schema.Instance.Namespaces.Add(ns.Prefix, ns.Uri.OriginalString);

         m_Schema.Namespace = new NamespaceInfo(ns.Prefix, ns.Uri);

         m_Schema.DataTextMapResolver = m_Context.DataTextMapResolver;
         m_Schema.BaseTypeResolver =
            new BaseTypeResolverDelegate(m_Context.BaseTypeResolver);
         m_Schema.TypeResolver =
            new TypeResolverDelegate(m_Context.TypeResolver);
         m_Schema.NamespaceResolver =
            new NamespaceResolverDelegate(m_Context.NamespaceResolver);
         m_Schema.TransformResolver =
            new DataTransformResolverDelegate(TransformResolver);

         m_Context.TransformResolver =
            new DataTransformResolverDelegate(TransformResolver);

         LoadTransformations();
      }

      public void LoadTransformations()
      {
         List<DataTransformItemInfo> items = new List<DataTransformItemInfo>()
         {
             new DataTransformItemInfo("uid", "string")
         };
         m_TransformItems.AddRange(items);
      }

      public Object TransformResolver(Object item)
      {
         Object result;
         if (item is String)
         {
            String id = item as String;
            var rule = m_TransformItems.Find((x) => x.SourceId == id);
            result = rule == null ? id : rule.TargetId;
         }
         else
            result = null;
         return result;
      }

      public void WriteComplexTypes(Resources.ResourceContext context)
      {
         var typeDictionary = context.GetTypeDictionary();
         foreach (var p in typeDictionary)
         {
            DataTypeInfo type = m_Context.TypeResolver(p.Value.Element);
            if (type == null || type.Namespace.Uri != m_Namespace)
            {
               continue;
            }
            m_Schema.AddNamespace(type.Namespace);
            m_Schema.AddComplextType(type, p.Value.Element, p.Value.Children);
         }
      }

      /// <summary>
      /// Add elements based on those within the context types/elements.
      /// </summary>
      /// <param name="context">data context</param>
      private void WriteElementAndAttributes(Resources.ResourceContext context)
      {
         List<string> registry = new List<string>();
         List<AssetDataElement> elements =
            m_Context.GetAllUriElements(m_Namespace.OriginalString);
         NamespaceInfo ns = context.DefaultNamespace;

         foreach (var element in elements)
         {
            var item = registry.Find((x) => x == element.ElementName);
            if (item != null)
            {
               continue;
            }

            m_Schema.AddElement(element, null, ns, true);
            registry.Add(element.ElementName);
         }
      }

      /// <summary>
      /// Add elements based on those within the context types/elements.
      /// </summary>
      /// <param name="context">data context</param>
      public void WriteElements(Resources.ResourceContext context)
      {
         List<string> registry = new List<string>();
         List<AssetDataElement> types =
            m_Context.GetAllUriTypes(m_Namespace.OriginalString);
         NamespaceInfo ns = context.DefaultNamespace;

         foreach (var type in types)
         {
            // find in parent types already registered in schema
            var pitem = m_Schema.FindItem(type.ElementName);
            if (pitem != null)
            {
               continue;
            }

            // find in local list...
            var item = registry.Find((x)=>x == type.ElementName);
            if (item != null)
            {
               continue;
            }

            var children = context.GetTypeChildren(type);
            NamespaceInfo typeNs = context.FindNamespace(type.Namespace);
            m_Schema.AddNamespace(typeNs);
            DataTypeInfo dtype = 
               new DataTypeInfo(type, typeNs, context.TypeNameSuffix);
            m_Schema.AddComplextType(dtype, type, children);
            registry.Add(type.ElementName);
         }

         // write element and attribute declarations
         WriteElementAndAttributes(context);
      }

      /// <summary>
      /// Add elements based on Use Case subset context types/elements.
      /// </summary>
      /// <param name="schema">use case schema</param>
      public void WriteElements(AssetUseCaseSchema schema)
      {
         List<string> registry = new List<string>();

         foreach (var item in schema.Registry)
         {
            var type = item.Value.AssetType;
            var entry = registry.Find((x) => x == type.Key);
            if (entry != null)
            {
               continue;
            }

            m_Schema.AddNamespace(schema.Namespace);
            DataTypeInfo dtype =
               new DataTypeInfo(type.Element, schema.Namespace, null);
            m_Schema.AddComplextType(
               dtype, type.Element, item.Value.ElementChildren);
            registry.Add(type.Key);
         }
      }

      /// <summary>
      /// Write assets xsd according to the given assets list and use context
      /// dictionary to solve types and other resources
      /// </summary>
      /// <param name="assets">list of assets to write</param>
      /// <param name="context">parent base context where the full type 
      /// definitions are found</param>
      /// <returns>instance of XsdSchema is returned</returns>
      public static List<XsdSchema> WriteAssetsSchema(
         AssetDataElementList assets, Resources.ResourceContext context)
      {
         List<AssetData> adata = new List<AssetData>();
         var d = new AssetData(
            assets.Namespace, AssetType.Schema, assets.VersionId);
         d.Items = assets;
         adata.Add(d);

         // this is the Use Case context
         List<XsdSchema> schemas = new List<XsdSchema>();
         var cntxt = new Resources.ResourceContext(
            context.DefaultNamespace.Prefix, context.DefaultNamespace.Uri,
            context.OrganizationDomainId, context.UriExtension,
            adata, assets.VersionId);
         cntxt.SetContext(adata);

         // first prepare the Use Case target Schema
         List<AssetUseCaseSchema> ucSchemas = new List<AssetUseCaseSchema>();
         foreach (var ns in context.Namespaces)
         {
            AssetUseCaseSchema schema = 
               AssetUseCaseSchema.PrepareSchema(cntxt, ns, context, ucSchemas);

            // if no elements were registered in the schema registry, don't add
            if (schema.Registry.Count > 0)
            {
               ucSchemas.Add(schema);
            }
         }

         // write generate Use Case schema
         cntxt.BaseContext = context;
         foreach(var item in ucSchemas)
         {
            if (item.Registry.Count == 0)
            {
               continue;
            }
            var sw = new XsdSchemaWriter(context, item.Namespace);
            //sw.WriteComplexTypes(cntxt);
            sw.WriteElements(item);

            schemas.Add(sw.Schema);
         }

         return schemas;
      }

   }

}

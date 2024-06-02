using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Json.Jsd;

namespace Edam.Data.AssetManagement.Writers.Jsd
{

   public class JsdSchemaWriter
   {

      readonly Resources.ResourceContext m_Context;
      readonly NamespaceInfo m_Namespace;
      readonly JsdSchema m_Schema;
      readonly List<DataTransformItemInfo> m_TransformItems =
         new List<DataTransformItemInfo>();

      public JsdSchema Schema
      {
         get { return m_Schema; }
      }

      public NamespaceInfo Namespace
      {
         get { return m_Namespace; }
      }

      public JsdSchemaWriter(
         Resources.ResourceContext context, NamespaceInfo ns,
         bool jsonLdIndicator = false)
      {
         m_Context = context;
         m_Namespace = ns;
         m_Schema = new JsdSchema(ns, null, JsdType.Object);
         m_Schema.JsonLinkLdContextIndicator = jsonLdIndicator;

         NamespaceInfo n = m_Context.FindNamespace(ns.Uri.OriginalString);
         m_Schema.Namespace = 
            new NamespaceInfo(n.Prefix, n.Uri, context.OrganizationDomainId);

         m_Schema.BaseTypeResolver =
            new BaseTypeResolverDelegate(m_Context.BaseTypeResolver);
         m_Schema.TypeResolver =
            new TypeResolverDelegate(m_Context.TypeResolver);
         m_Schema.NamespaceResolver =
            new NamespaceResolverDelegate(m_Context.NamespaceResolver);
         m_Schema.TransformResolver =
            new DataTransformResolverDelegate(TransformResolver);

         LoadTransformations();
      }

      #region -- Transformations Support

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

      #endregion
      #region -- Writer Methods

      public void WriteStart()
      {
         m_Schema.AddPush();
         if (m_Schema.JsonLinkLdContextIndicator)
         {
            m_Schema.AddPush();
            m_Schema.AddPush();
         }
      }

      public void WriteComplexTypes()
      {
         var dic = m_Context.GetTypeDictionary();
         List<JsdComplexType> types = new List<JsdComplexType>();
         List<Resources.ElementTypeInfo> items = 
            new List<Resources.ElementTypeInfo>();

         foreach(var i in dic)
         {
            items.Add(i.Value);
         }

         foreach (var p in items)
         {
            if (m_Namespace.Uri.OriginalString != p.Element.Namespace)
            {
               continue;
            }
            DataTypeInfo type = m_Context.TypeResolver(p.Element);
            if (type != null)
            {
               m_Schema.AddNamespace(type.Namespace);
            }
            types.Add(new JsdComplexType
            {
               Type = type,
               Element = p.Element,
               Children = p.Children
            });
         }

         if (!m_Schema.JsonLinkLdContextIndicator)
         {
            m_Schema.AddPropertyStart(JsdSchema.DEFINITIONS);
         }

         for (var i=0; i < types.Count; i++)
         {
            m_Schema.AddComplexType(
               types[i].Type, types[i].Element, types[i].Children,
               i < types.Count - 1);
         }

         if (!m_Schema.JsonLinkLdContextIndicator)
         {
            m_Schema.AddPropertyEnd(true);
         }
      }

      public void WriteElements(Boolean continues = false)
      {
         if (m_Schema.JsonLinkLdContextIndicator)
         {
            return;
         }

         List<DataElement> elements = new List<DataElement>();

         foreach(var itm in m_Context.Elements)
         {
            if (itm.IsType)
               continue;
            ElementBaseInfo element = 
               m_Context.ElementResolver(itm);
            if (element.Namespace.Uri != m_Namespace.Uri)
               continue;

            // add needed namespaces...
            m_Schema.AddNamespace(element.Namespace);
            DataTypeInfo dataType =
               m_Context.BaseTypeResolver(element.BaseElement);
            element.DataType = dataType;
            if (dataType != null)
            {
               m_Schema.AddNamespace(dataType.Namespace);
            }

            // add element
            // TODO: use the ElementBaseInfo to add the schema element instead
            elements.Add(itm);
         }

         m_Schema.AddPropertyStart(JsdSchema.PROPERTIES);

         for (var i=0; i < elements.Count; i++)
         {
            m_Schema.AddElement(elements[i], i < elements.Count - 1);
         }

         m_Schema.AddPropertyEnd(continues);
      }

      public void WriteDocument()
      {
         m_Schema.WriteDocument();
      }

      public void WriteEnd()
      {
         m_Schema.AddPop();
      }

      #endregion

   }

}

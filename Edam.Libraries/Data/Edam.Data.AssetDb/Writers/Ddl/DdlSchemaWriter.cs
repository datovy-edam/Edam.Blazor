using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Data.Schema.DataDefinitionLanguage;
using Edam.Data.AssetManagement.Resources;
using Edam.Data.Schema.SchemaObject;

namespace Edam.Data.AssetManagement.Writers.Ddl
{

   public class DdlSchemaWriter
   {

      #region -- Static Strings...

      #endregion
      #region -- Properties and Declarations...

      private readonly Resources.ResourceContext m_Context;
      readonly List<DataTransformItemInfo> m_TransformItems =
         new List<DataTransformItemInfo>();

      public ResourceContext Context
      {
         get { return m_Context; }
      }

      private DdlSchema m_Schema;
      public DdlSchema Schema
      {
         get { return m_Schema; }
      }

      private readonly NamespaceInfo m_Namespace;
      public NamespaceInfo Namespace
      {
         get { return m_Namespace; }
      }

      #endregion
      #region -- Constructor, Destructor...

      public DdlSchemaWriter (
         ResourceContext context, NamespaceInfo ns)
      {
         m_Context = context;
         m_Namespace = ns;
      }

      public DdlSchemaWriter(
         AssetData data, NamespaceInfo ns = null)
      {
         m_Context = new ResourceContext(data);
         m_Namespace = ns ?? data.DefaultNamespace;
      }

      public void Initialize(DdlSchema schema)
      {
         m_Schema = schema;
         m_Context.DataTextMapResolver = schema.DataTextMapResolver;
         SetupContext();
         LoadTransformations();
      }

      private void SetupContext()
      {
         if (m_Context == null)
         {
            return;
         }

         //if (m_Namespace != null)
         //{
         //   NamespaceInfo n = m_Context.FindNamespace(
         //      m_Namespace.Uri.OriginalString);
         //   m_Schema.Namespace = new NamespaceInfo(
         //      n.Prefix, n.Uri, m_Context.OrganizationDomainId);
         //}

         m_Schema.DataBaseTypeResolver = m_Context.DataBaseTypeResolver;
         m_Schema.DataTypeResolver = m_Context.DataTypeResolver;
         m_Schema.DataNamespaceResolver = m_Context.DataNamespaceResolver;
         m_Schema.DataTextMapResolver = null;
         m_Schema.DataTransformResolver =
            new DataTransformResolverDelegate(TransformResolverHelper);
         m_Schema.ElementTypeResolver = m_Context.ElementTypeResolver;

         m_Schema.DataFindKeyCandidates = m_Context.DataFindKeyCandidates;
         m_Schema.DataGetChildrenOfChildren =
            m_Context.DataGetChildrenOfChildren;

         m_Context.RegisterContext();
      }

      #endregion
      #region -- Transformations Support

      public void LoadTransformations()
      {
         List<DataTransformItemInfo> items = new List<DataTransformItemInfo>()
         {
             new DataTransformItemInfo("uid", "string")
         };
         m_TransformItems.AddRange(items);
      }

      public Object TransformResolverHelper(Object item)
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

   }

}

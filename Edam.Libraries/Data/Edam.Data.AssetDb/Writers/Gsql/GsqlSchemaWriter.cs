using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Gsql;

namespace Edam.Data.AssetManagement.Writers.Gsql
{

   public class GsqlSchemaWriter
   {

      #region -- Properties and Defifnitions

      readonly Resources.ResourceContext m_Context;
      readonly NamespaceInfo m_Namespace;
      readonly GsqlSchema m_Schema;
      readonly List<DataTransformItemInfo> m_TransformItems =
         new List<DataTransformItemInfo>();
      private DataTextMap m_DataTextMap = new DataTextMap();

      public GsqlSchema Schema
      {
         get { return m_Schema; }
      }

      public NamespaceInfo Namespace
      {
         get { return m_Namespace; }
      }

      #endregion
      #region -- Constructors / Destructures

      public GsqlSchemaWriter(
         Resources.ResourceContext context, NamespaceInfo ns)
      {
         m_Context = context;
         m_Namespace = ns;
         m_Schema = new GsqlSchema(ns, null, GsqlItemType.Graph);

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

      #endregion
      #region -- Transformations Support

      public void SetDataTextMap(DataTextMap map)
      {
         m_Schema.SetDataTextMap(map);
         if (map == null)
         {
            m_DataTextMap = new DataTextMap();
            return;
         }
         m_DataTextMap = map;
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

      #endregion
      #region -- Writer Methods

      public void WriteStart()
      {
         //m_Schema.AddPush();
      }

      public void WriteElements(Boolean continues = false)
      {
         List<AssetDataElement> elements = m_Context.GetAllTypes(
           m_Context.DefaultNamespace.NamePath.Root);

         foreach (var itm in elements)
         {
            //if (itm.Element == null)
            //   continue;
            ElementBaseInfo element =
               m_Context.ElementResolver(itm);
            if (element.Namespace.Uri != m_Namespace.Uri)
               continue;

            // add needed namespaces...
            m_Schema.AddNamespace(element.Namespace);
            DataTypeInfo dataType =
               m_Context.BaseTypeResolver(element.BaseElement);
            element.DataType = dataType;
            m_Schema.AddNamespace(dataType.Namespace);

            // write children
            List<AssetDataElement> children = m_Context.GetTypeChildren(itm);
            m_Schema.AddVertex(itm, children, false);
         }
      }

      public void WriteDocument()
      {
         m_Schema.WriteDocument();
      }

      public void WriteEnd()
      {
         //m_Schema.AddPop();
      }

      #endregion

   }

}

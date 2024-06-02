using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

// -----------------------------------------------------------------------------
using Edam.Data.AssetSchema;
using Edam.Data.Asset;
using Edam.Data.AssetConsole;
using Edam.Diagnostics;
using Edam.InOut;
using Edam.Data.Schema.SchemaObject;
using Edam.Data.AssetManagement;

namespace Edam.Data.Schema.DataDefinitionLanguage
{

   public delegate void InitializeSchemaDelegate(DdlSchema schema);

   /// <summary>
   /// The Schema Set holds information about:
   ///    - Catalogs
   ///       - Schemas
   ///          - Resources
   ///             - Elements
   /// </summary>
   public class DdlSchemaSet
   {

      #region -- fields and properties...

      private SchemaSet m_SchemaSet;
      private List<DdlSchema> m_Schemas;
      public int OrdinalNo { get; set; }

      public object Catalogs
      {
         get { return m_SchemaSet.Catalogs; }
      }

      private readonly IWriter m_Writer;
      public List<DdlSchema> Items
      {
         get { return m_Schemas; }
      }

      private DataTextMap m_DataTextMap;
      public DataTextMap DataTextMap
      {
         get { return m_DataTextMap; }
      }

      public InitializeSchemaDelegate IntializeSchema { get; set; }
      public DomainResolverDelegate DataDomainResolver { get; set; }

      public ElementTransform DefaultElementTransform { get; set; }

      private AssetConsoleArgumentsInfo m_Arguments;
      public AssetConsoleArgumentsInfo Arguments
      {
         get { return m_Arguments; }
      }

      public DdlSchemaSet(AssetConsoleArgumentsInfo arguments)
      {
         m_Arguments = arguments;
      }

      #endregion
      #region -- Constructor

      public DdlSchemaSet(List<DdlSchema> schema)
      {
         m_Schemas = schema;
      }
      public DdlSchemaSet(
         SchemaSet schemaSet, AssetConsoleArgumentsInfo arguments)
      {
         m_Schemas = new List<DdlSchema>();
         if (schemaSet == null)
            return;
         Set(schemaSet);
         m_DataTextMap = DataTextMap.FromFile(arguments);
      }

      public DdlSchemaSet(IWriter writer, DataTextMap textMap,
         InitializeSchemaDelegate initializeSchema,
         IResourceCallHelpers callbackHelper,
         ElementTransform defaultElementTypeTransform)
      {
         m_Schemas = new List<DdlSchema>();
         DefaultElementTransform = defaultElementTypeTransform;
         IntializeSchema = initializeSchema;
         if (callbackHelper != null)
            DataDomainResolver = callbackHelper.DataDomainResolver;
         m_Writer = writer;
         Set(writer.DataContext as SchemaSet);
         m_DataTextMap = textMap;
      }

      #endregion
      #region -- Set and Add Sets and Schema methods...

      public void SetTextMap(AssetConsoleArgumentsInfo arguments)
      {
         m_DataTextMap = DataTextMap.FromFile(arguments);
      }

      private object DataMapResolver(string item)
      {
         if (m_DataTextMap == null)
            return new DataTextMapItem(
               item, item, DataTextMapDirection.To);

         // try finding the type as an element
         var element = m_DataTextMap.FindElement(item);
         if (element as DataTextElementInfo != null)
         {
            return element;
         }

         // try to map the text
         var itm =
            m_DataTextMap.MapText(item, DataTextMapDirection.To);
         return new DataTextMapItem(
            item, itm, DataTextMapDirection.To);
      }

      private DataTextTypeInfo DataTypeMapResolver(IDataElement element,
         DataTextTypeInfo defaultType)
      {
         if (m_DataTextMap == null)
            return defaultType;
         var itm = m_DataTextMap.MatchElementType(element, defaultType);
         return itm;
      }

      public void Set(SchemaSet schemaSet)
      {
         m_SchemaSet = schemaSet;

         foreach (var c in m_SchemaSet.Catalogs)
         {
            foreach (var s in c.Schemas)
            {
               DataDomain domain = (DataDomainResolver == null) ? null :
                  DataDomainResolver(s.Namespace.Uri.OriginalString);
               var i = new DdlSchema(c, s, s.Namespace)
               {
                  ElementTypeTransform = DefaultElementTransform,
                  DataTextMapResolver = DataMapResolver,
                  Domain = domain ?? null
               };
               IntializeSchema?.Invoke(i);
               i.DataTextMapResolver = DataMapResolver;
               i.DataTextTypeResolver = DataTypeMapResolver;
               i.DataDomainResolver = DataDomainResolver;
               m_Schemas.Add(i);
            }
         }
      }

      public void Add(DdlSchema schema)
      {
         m_Schemas.Add(schema);
      }

      #endregion
      #region -- Compille Schema and write to selecte output...

      /// <summary>
      /// Set call back function... then you should call GenerateSchemas...
      /// </summary>
      /// <param name="errorNotification"></param>
      public void CompileSchemas(Func<Exception, string> errorNotification)
      {
         foreach(var s in m_Schemas)
         {
            s.PrepareOutput(m_DataTextMap);
            s.PrepareAnnotations();
         }
      }

      public void GenerateSchemas(List<NamespaceInfo> namespaces)
      {
         foreach (var s in m_Schemas)
         {
            String fname = s.Namespace.ToFileName();
            String outText = s.ToString();
            if (String.IsNullOrWhiteSpace(outText))
               continue;
            m_Writer.Write(fname, outText);
            m_Writer.Write(fname + "_properties", s.OutPropertiesText);
         }
      }

      public void GenerateDocumentation()
      {

      }

      #endregion

   }

}

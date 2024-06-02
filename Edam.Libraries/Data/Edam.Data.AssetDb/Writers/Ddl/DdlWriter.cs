using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

// -----------------------------------------------------------------------------
using Edam.Data.AssetManagement.Resources;
using Edam.Data.AssetManagement.Writers.Ddl;
using Edam.InOut;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Data.AssetConsole;
using Edam.Data.Schema.SchemaObject;
using Edam.Data.Schema.DataDefinitionLanguage;

namespace Edam.Data.AssetManagement.Writers.Ddl
{
   
   public class DdlWriter : SchemaWriterAbstract<AssetConsoleArgumentsInfo>
   {

      private readonly List<DdlSchema> m_Schemas = new List<DdlSchema>();

      public DdlWriter(AssetConsoleArgumentsInfo context) : base(context)
      {
         m_Context = context;
      }

      public void PrepareSchemas()
      {
      }

      /// <summary>
      /// The "Context" data loaded root is treated as the "Catalog" and then
      /// the rest is returned as a Schema Set as follows:
      ///    - Catalogs          - DB
      ///       - Schemas        - DB Schema
      ///          - Resources   - DB Schema.Table
      ///             - Elements - DB Schema.Table.Column
      /// </summary>
      /// <returns>instance of SchemaSet is returned...</returns>
      private static SchemaSet ToSchemaSet(AssetConsoleArgumentsInfo context,
         DataTextMap textMap)
      {
         SchemaSet schemaSet = new SchemaSet();
         if (schemaSet.Catalogs == null)
            schemaSet.Catalogs = new List<CatalogInfo>();

         // get Assets Data
         List<AssetData> assets = context.AssetDataItems;

         // add catalog based on the root URI...
         String root = context.Namespace.Prefix;
         CatalogInfo cat = new CatalogInfo(context.ProjectVersionId)
         {
            Namespace = new NamespaceInfo(root, context.Namespace.Uri),
            Name = root
         };
         schemaSet.Catalogs.Add(cat);

         // assets are assumed to have types with children in order...
         foreach (var asset in assets)
         {
            NamespaceInfo ns = asset.DefaultNamespace ?? cat.Namespace;
            if (ns == null)
               continue;

            SchemaInfo schm = null;
            SchemaInfo resourceSchema = null;

            // add resources based on schema types - DB Schema.Tables
            ResourceInfo resource = new ResourceInfo();
            String typeId = String.Empty;

            // go through each asset element type > add those as table columns
            string typeText = String.Empty;
            foreach(var i in asset.Items)
            {
               // find item namespace
               NamespaceInfo nspace = asset.GetNamespace(i);

               // find or add schema...
               bool entityNameEmpty = String.IsNullOrWhiteSpace(i.EntityName);
               if (i.IsType || i.IsRoot || entityNameEmpty)
               {
                  schm = cat.Find(nspace, true);
               }
               else if (schm == null && !entityNameEmpty)
               {
                  throw new Exception("Element with no-namespace found.");
               }

               // get type name
               var typeName = String.IsNullOrWhiteSpace(i.Type) ?
                  i.EntityQualifiedNameText : i.Type;

               // new type is found? > add resource, and move to next...
               if ((String.IsNullOrWhiteSpace(typeName) || typeName != typeId))
               {
                  if (resource.Resources.Count > 0)
                  {
                     if (resourceSchema == null)
                     {
                        resourceSchema = schm;
                     }
                     resourceSchema.Items.Add(resource);
                  }

                  if (String.IsNullOrWhiteSpace(i.OriginalName))
                  {
                     i.OriginalName = i.ElementQualifiedName.OriginalName;
                  }

                  resourceSchema = schm;
                  resource = new ResourceInfo
                  {
                     Entity = i,
                     Name = i.OriginalName,
                     Namespace = nspace
                  };

                  typeId = i.Element;
                  continue;
               }

               if (!i.MaxLength.HasValue)
               {
                  i.MaxLength = (int?)i.Length;
               }

               // add elements based on type element - DB Schema.Table.Column
               ElementInfo e = new ElementInfo
               {
                  Name = i.ElementQualifiedName.OriginalName,
                  DataType = i.TypeQualifiedName.OriginalName,
                  DataSize = i.MaxLength.ToString(),
                  Element = i
               };
               resource.Items.Add(e);
               resource.Resources.Add(i);
            }
         }
         return schemaSet;
      }
      
      /// <summary>
      /// 
      /// </summary>
      /// <param name="writer"></param>
      /// <param name="arguments">It is assumed that it contains the needed
      /// AssetDataItems with only one merged AssetData</param>
      public void WriteSet(IWriter writer, AssetConsoleArgumentsInfo arguments)
      {
         ElementTransform elementTypeTransform = arguments == null |
            String.IsNullOrWhiteSpace(arguments.ElementTransform) ?
               ElementTransform.Unknown :
               Enum.Parse<ElementTransform>(arguments.ElementTransform);

         DdlSchemaWriter schemaWriter = 
            new DdlSchemaWriter(arguments.AssetDataItems[0], null);
         DataTextMap textMap = DataTextMap.FromFile(arguments);

         // Prepare context Resources with arguments Asset list...
         writer.DataContext = ToSchemaSet(m_Context, textMap);

         // Prepare schema set...
         DdlSchemaSet s = new DdlSchemaSet(writer, textMap,
            schemaWriter.Initialize, null, elementTypeTransform);

         s.CompileSchemas((e) =>
         {
            return e.Message;
         });

         // add all namespaces to be proccessed
         List<NamespaceInfo> ns = new List<NamespaceInfo>();
         foreach(var i in arguments.AssetDataItems)
         {
            foreach(var n in i.Namespaces)
            {
               ns.Add(n);
            }
         }

         s.GenerateSchemas(ns);
      }

      public static void WriteCodeSets(AssetConsoleArgumentsInfo arguments)
      {
         IWriter writer = new FolderWriter(
            arguments.OutFilePath, arguments.OutFileName + ".codes",
            arguments.OutFileExtension);

      }

      /// <summary>
      /// Prepare a DDL file using given Assets Dictionary context.
      /// </summary>
      /// <param name="context">Assets Data Dictionary DB context</param>
      /// <param name="arguments">Details needed to process request</param>
      public static void WriteSchema(AssetConsoleArgumentsInfo arguments)
      {
         IWriter writer = new FolderWriter(
            arguments.OutFilePath, arguments.OutFileName,
            arguments.OutFileExtension);

         DdlWriter wr = new DdlWriter(arguments);
         wr.PrepareSchemas();

         writer.Open();
         wr.WriteSet(writer, arguments);
         writer.Close();
      }

   }

}

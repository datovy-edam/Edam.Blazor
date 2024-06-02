using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.AssetConsole;
using Edam.Xml.OpenXml;
using Edam.Data.Asset;
using Edam.Application;
using Edam.Diagnostics;
using Edam.Data.AssetSchema;
using Edam.Data.AssetManagement;
using Edam.DataObjects.Models;
using Edam.Data.Schema.DataDefinitionLanguage;
using ObjAssets = Edam.DataObjects.Assets;
using Edam.Data.Assets.AssetSchema;
using reader = Edam.Text.StringReader;
using System.Data.SqlTypes;

namespace Edam.Data.Schema.ImportExport
{

   public class ImportValues
   {
      public List<string> Values { get; set; }
      public List<string> Header { get; set; }
   }

   public class ImportReader : IDataAssets
   {
      private const string CLASS_NAME = "ImportReader";

      private DataTextMap m_Mapper;
      private AssetConsoleArgumentsInfo m_Arguments;

      private string m_VersionId
      {
         get { return m_Arguments.Project.VersionId; }
      }

      /// <summary>
      /// Import an Item (Table, Function, View, or Stored-Procedure)
      /// </summary>
      /// <param name="values">list of values</param>
      /// <returns>instance of ImportItemInfo is returned with info</returns>
      /// <exception cref="ArgumentException"></exception>
      public static ImportItemInfo ImportItem(ImportValues values)
      {
         String func = "SetValues";
         if (values.Values.Count > 21)
         {
            throw new ArgumentException(CLASS_NAME + "::" + func +
               ": Expected no more than 13, 15 or 19/21 columns got (" +
               values.Values.Count.ToString() + ")");
         }

         ImportItemInfo item = new ImportItemInfo();

         if (values.Values.Count <= 15 && values.Header[11] != null &&
             values.Header[11].ToUpper() == "CONSTRAINT_TYPE")
         {
            item.Dbms = reader.GetString(values.Values[0]);
            item.TableCatalog = reader.GetString(values.Values[1]);
            item.TableSchema = reader.GetString(values.Values[2]);
            item.TableName = reader.GetString(values.Values[3]);
            item.ColumnName = reader.GetString(values.Values[4]);
            item.OrdinalPosition = reader.GetLong(values.Values[5]);
            item.DataType = reader.GetString(values.Values[6]);
            item.CharacterMaximumLength = reader.GetDecimal(values.Values[7]);

            item.Precision = reader.GetInteger(values.Values[8]);
            item.Scale = reader.GetInteger(values.Values[9]);
            item.IsNullable = reader.GetBool(values.Values[10]);

            item.ConstraintType = reader.GetString(values.Values[11]);
            item.ConstraintTableSchema = reader.GetString(values.Values[12]);
            item.ConstraintTableName = reader.GetString(values.Values[13]);
            item.ConstraintColumnName = reader.GetString(values.Values[14]);
         }
         else if (values.Values.Count <= 15)
         {
            item.Dbms = reader.GetString(values.Values[0]);
            item.TableCatalog = reader.GetString(values.Values[1]);
            item.TableSchema = reader.GetString(values.Values[2]);
            item.TableName = reader.GetString(values.Values[3]);
            item.ColumnName = reader.GetString(values.Values[4]);
            item.OrdinalPosition = reader.GetLong(values.Values[5]);
            item.DataType = reader.GetString(values.Values[6]);
            item.CharacterMaximumLength = reader.GetDecimal(values.Values[7]);
            item.ConstraintType = reader.GetString(values.Values[8]);
            item.ConstraintTableSchema = reader.GetString(values.Values[9]);
            item.ConstraintTableName = reader.GetString(values.Values[10]);
            item.ConstraintColumnName = reader.GetString(values.Values[11]);

            item.IsIdentity = (values.Values.Count > 12) ?
               reader.GetBool(values.Values[12]) : false;
            item.Tags = (values.Values.Count > 13) ?
               reader.GetString(values.Values[13]) : String.Empty;
            item.ColumnDescription = (values.Values.Count > 14) ?
               reader.GetString(values.Values[14]) : String.Empty;
         }

         // the following support an enhace schema with all object types
         else if (values.Values.Count >= 19)
         {
            item.Dbms = reader.GetString(values.Values[0]);
            item.TableCatalog = reader.GetString(values.Values[1]);
            item.TableSchema = reader.GetString(values.Values[2]);
            item.ObjectName = reader.GetString(values.Values[3]);
            item.ColumnName = reader.GetString(values.Values[4]);
            item.OrdinalPosition = reader.GetLong(values.Values[5]);
            item.DataType = reader.GetString(values.Values[6]);
            item.CharacterMaximumLength = reader.GetDecimal(values.Values[7]);

            item.Precision = reader.GetInteger(values.Values[8]);
            item.Scale = reader.GetInteger(values.Values[9]);

            item.IsOutput = reader.GetBool(values.Values[10]);
            item.IsReadOnly = reader.GetBool(values.Values[11]);
            item.IsNullable = reader.GetBool(values.Values[12]);
            item.IsIdentity = reader.GetBool(values.Values[13]);

            string otype = reader.GetString(values.Values[14]).ToUpper();
            switch (otype)
            {
               case "PROCEDURE":
                  item.ObjectType = ElementType.procedure;
                  break;
               case "FUNCTION":
                  item.ObjectType = ElementType.function;
                  break;
               case "VIEW":
                  item.ObjectType = ElementType.view;
                  break;
               case "TABLE":
               default:
                  item.ObjectType = ElementType.type;
                  break;
            }

            item.ConstraintType = reader.GetString(values.Values[15]);
            item.ConstraintTableSchema = reader.GetString(values.Values[16]);
            item.ConstraintTableName = reader.GetString(values.Values[17]);
            item.ConstraintColumnName = reader.GetString(values.Values[18]);

            if (item.ObjectType == ElementType.procedure ||
                item.ObjectType == ElementType.function)
            {
               item.ColumnName = item.ColumnName.Replace("@", "");
            }

            if (values.Values.Count == 21)
            {
               item.Tags = reader.GetString(values.Values[19]);
               item.ColumnDescription = reader.GetString(values.Values[20]);
            }
         }

         if (item.ConstraintType == "P")
            item.ConstraintType = AssetElementConstraintInfo.KEY;
         else if (item.ConstraintType == "R")
            item.ConstraintType = AssetElementConstraintInfo.FOREIGN_KEY;

         return item;
      }

      public List<string> GetFileList(AssetConsoleArgumentsInfo arguments)
      {
         var uriList = UriResourceInfo.GetUriList(
           arguments.UriList, UriResourceType.xsd);
         return null;
      }

      public IResultsLog ToDatabase(AssetConsoleArgumentsInfo arguments)
      {
         var uriList = UriResourceInfo.GetUriList(
           arguments.UriList, UriResourceType.xsd);
         foreach (var i in uriList)
         {
            
         }

         return null;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="assetProperties"></param>
      /// <param name="items"></param>
      /// <param name="namespaces"></param>
      /// <param name="rootName"></param>
      /// <returns></returns>
      public List<AssetData> ToAssetData(
         AssetProperties assetProperties, List<ImportItemInfo> items,
         NamespaceList namespaces, string rootName)
      {
         if (items == null || items.Count == 0)
         {
            return null;
         }

         var header = items[0];

         String guidText = Guid.NewGuid().ToString();
         NamespaceInfo ns = namespaces == null || namespaces.Count == 0 ?
            new NamespaceInfo(guidText, "http://" + guidText) : namespaces[0];

         SortedDictionary<string, DdlAsset> dassets = 
            new SortedDictionary<string, DdlAsset>();

         DdlAsset dasset = null, previousAsset = null;

         ElementPropertyInfo xproperty;
         ElementPropertyInfo entityProperty;

         int resourceCount = 0;
         int schemaCount = 1;

         foreach (var item in items)
         {
            item.OrdinalNo = ++resourceCount;

            entityProperty = assetProperties.Find(
               item.TableSchema, item.TableName, String.Empty,
               ElementPropertyType.Description);

            // is element mapped to an external property?
            xproperty = assetProperties.Find(
               item.TableSchema, item.TableName, item.ColumnName,
               ElementPropertyType.ExternalReference);

            if (!dassets.TryGetValue(item.TableSchema, out dasset))
            {
               if (previousAsset != null)
               {
                  previousAsset.PrepareAdditionalColumns();
               }

               dasset = new DdlAsset(header, namespaces, ns, m_Mapper,
                  item.TableSchema, m_VersionId, schemaCount);
               dasset.asset.CatalogName = item.TableCatalog;
               previousAsset = dasset;

               dassets.Add(item.TableSchema, dasset);
               schemaCount++;
            }

            // this is a new table definition
            if (dasset == null || 
               item.TableName != previousAsset.originalTableName)
            {
               previousAsset.PrepareAdditionalColumns();
               dasset.PrepareTableDefinition(item, item.TableName);
               dasset.originalTableName = item.TableName;

               if (entityProperty != null)
               {
                  dasset.UpdateEntityProperty(entityProperty);
               }
               //continue;
            }

            // prepare/add table column definition
            var celement = dasset.PrepareColumnDefinition(item);
            if (xproperty != null)
            {
               DdlAsset.UpdateEntityProperty(xproperty, celement);
            }
         }

         if (dasset != null)
         {
            dasset.PrepareAdditionalColumns();
         }

         // add property bags of last items for each ddl asset...
         List<AssetData> assets = new List<AssetData>();
         foreach (var i in dassets)
         {
            i.Value.AddPropertyBag();
            assets.Add(i.Value.asset);
         }

         // merge assets...
         AssetData assetData = AssetData.Merge(assets, header.TableCatalog,
            header.TableCatalog, "schemaName", "description", "title", ns,
            AssetType.Schema, m_VersionId);

         // prepare catalog document...
         var documentItems = PrepareCatalogDocument(header, dassets, ns);
         foreach (var item in documentItems)
         {
            assetData.Add(item);
         }

         assets.Clear();
         assets.Add(assetData);

         return assets;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="header"></param>
      /// <param name="asset"></param>
      /// <param name="ns"></param>
      /// <returns></returns>
      private AssetDataElementList PrepareCatalogDocument(
         ImportItemInfo header,
         SortedDictionary<string, DdlAsset> asset, NamespaceInfo ns)
      {
         var appSettings = AppAssembly.FetchInstance(
            AssetResourceHelper.ASSET_APP_SETTINGS) as IAppSettings;
         string typePostfix = appSettings == null ? 
            String.Empty : appSettings.GetTypePostfix();

         // prepare root element
         AssetDataElementList documentList =
            new AssetDataElementList(ns, AssetType.Schema, m_VersionId);

         // add document type
         string rootElementName = header.TableCatalog + typePostfix;
         var documentSchema = DdlAsset.PrepareTypeDefinition(ElementType.type,
            String.Empty, rootElementName, header.TableCatalog, ns.Prefix,
            Session.OrganizationId, ns);
         documentSchema.InclusionType = DataElementInclusionType.Exclude;
         documentList.Add(documentSchema);
         AssetDataElementList list = 
            new AssetDataElementList(ns, AssetType.Schema, m_VersionId);

         // prepare type that include all registered schemas... add child types
         foreach (var schema in asset.Values)
         {
            // add schema type
            string tName = schema.asset.SchemaName + typePostfix;
            var parentSchema = DdlAsset.PrepareTypeDefinition(
               ElementType.type,
               String.Empty, tName, schema.asset.SchemaName, schema.ns.Prefix,
               Session.OrganizationId, schema.ns);
            parentSchema.InclusionType = DataElementInclusionType.Exclude;
            list.Add(parentSchema);

            // prepare an entry per schema table... as an element
            foreach (var table in schema.Tables)
            {
               var item = schema.PrepareElementDefinition(
                  parentSchema, table.Item as ImportItemInfo, true);
               item.MaxOccurrence = int.MaxValue;
               item.InclusionType = DataElementInclusionType.Exclude;
               list.Add(item);
            }

            ImportItemInfo itm = new ImportItemInfo();
            itm.TableSchema = String.Empty;
            itm.ColumnName = schema.ns.Prefix + ":" + schema.asset.SchemaName;
            itm.DataType = parentSchema.DataType;
            itm.TableName = schema.asset.SchemaName;
            itm.CharacterMaximumLength = 0;

            var ditem = schema.PrepareElementDefinition(
               documentSchema, itm, true);
            ditem.Domain = ns.Prefix;
            ditem.InclusionType = DataElementInclusionType.Exclude;
            documentList.Add(ditem);
         }

         // prepare root and document elements...
         DdlAsset rootAsset = new DdlAsset(
            header, null, ns, null, null, m_VersionId, 0);

         // add schema type
         var rootSchema = DdlAsset.PrepareTypeDefinition(ElementType.type,
            String.Empty, header.TableCatalog, header.TableCatalog, ns.Prefix,
            Session.OrganizationId, ns);
         //list.Add(rootSchema);

         // add schema as a child of document
         ImportItemInfo ritm = new ImportItemInfo();
         ritm.TableSchema = String.Empty;
         ritm.ColumnName = rootSchema.ElementName;
         ritm.DataType = rootSchema.DataType;
         ritm.TableName = rootSchema.ElementQualifiedName.OriginalName;
         ritm.CharacterMaximumLength = 0;

         var rootDocument = rootAsset.PrepareElementDefinition(
            null, ritm, true, true);
         rootDocument.Domain = ns.Prefix;
         documentList.Add(rootDocument);

         // add document element declaration
         //fitm.TableName = rootAsset.asset.CatalogName;
         //var root = rootAsset.PrepareElementDefinition(null, fitm, true, true);
         //documentList.Add(root);

         list.AddRange(documentList);

         return list;
      }

      /// <summary>
      /// Read input file as specified in arguments and convert it to a Data 
      /// Asset (collection of Data Elements).
      /// </summary>
      /// <param name="arguments">arguments</param>
      /// <returns>results log</returns>
      public IResultsLog ToAsset(AssetConsoleArgumentsInfo arguments)
      {
         m_Arguments = arguments;

         IResultsLog resultsLog = new ResultLog();

         List<ImportItemInfo> rows = new List<ImportItemInfo>();
         NamespaceList namespaces = new NamespaceList();
         namespaces.Add(arguments.Namespace);

         m_Mapper = DataTextMap.FromFile(arguments);

         var uriList = UriResourceInfo.GetUriList(
           arguments.UriList, UriResourceType.xlsx);
         foreach (var fname in uriList)
         {
            var docList = ExcelDocumentReader.ReadDocument(
               fname, "Documentation");
            AssetProperties doc = docList.Success ?
               AssetProperties.GetInstance(docList.Data) : 
                  new AssetProperties(
                     new List<ElementPropertyInfo>());

            ImportValues values = new ImportValues();
            var results = ExcelDocumentReader.ReadDocument(
               fname, arguments.Domain.DomainId);
            if (results.Success)
            {

               values.Header = results.Data[0];
               foreach (var list in results.Data)
               {
                  // skip empty rows
                  if (ExcelDocumentReader.IsEmptyList(list))
                  {
                     continue;
                  }
                  values.Values = list;
                  rows.Add(ImportItem(values));
               }

               // remove header row
               if (rows.Count > 0)
               {
                  rows.RemoveAt(0);
               }

               // prepare Asset Data definitions (one per schema)
               var assets = ToAssetData(doc, rows, namespaces,
                  arguments.RootElementName);

               if (assets != null)
               {
                  if (arguments.AssetDataItems == null)
                  {
                     arguments.AssetDataItems = new AssetDataList();
                  }
                  foreach(var a in assets)
                  {
                     a.Namespaces.Add(NamespaceInfo.GetW3CNamespace());
                  }
                  arguments.AssetDataItems.AddRange(assets);
               }
            }
         }
         resultsLog.Succeeded();
         return resultsLog;
      }

   }

}

using System;
using System.Collections.Generic;
using System.Linq;

// -----------------------------------------------------------------------------
using Edam.Data.AssetSchema;
using Edam.Data.Asset;
using Edam.Data.AssetConsole;
using Edam.Diagnostics;
using Edam.Data.Schema.SchemaObject;
using Edam.Data.AssetManagement;
using Edam.DataObjects.Models;
using Edam.DataObjects.Assets;
using Edam.Application;
using ObjAssets = Edam.DataObjects.Assets;

namespace Edam.Data.Schema.DataDefinitionLanguage
{

   public class DdlSchemaInstance
   {
      public static readonly string CLASS_NAME = "DdlSchemaInstance";

      private AssetConsoleArgumentsInfo m_Arguments;
      public AssetConsoleArgumentsInfo Arguments
      {
         get { return m_Arguments; }
      }

      public DdlSchemaInstance(AssetConsoleArgumentsInfo arguments)
      {
         m_Arguments = arguments;
      }

      /// <summary>
      /// Giving a set of arguments, prepare an AssetData if possible.
      /// </summary>
      /// <param name="arguments"></param>
      /// <returns></returns>
      public static ResultsLog<AssetData> ToDataAsset(
         AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<DdlSchemaSet> results1 = FromRepository(arguments);
         ResultsLog<AssetData> results2 = new ResultsLog<AssetData>();
         if (!results1.Success)
         {
            results2.Failed(EventCode.Failed);
            return results2;
         }
         NamespaceList l = new NamespaceList();
         l.Add(arguments.Namespace);
         results2.Data = ToDataAsset(results1.Data, l, arguments.Name, true);
         if (arguments.AssetDataItems == null)
         {
            arguments.AssetDataItems = new AssetDataList();
         }
         arguments.AssetDataItems.Add(results2.Data);
         results2.Succeeded();
         return results2;
      }

      /// <summary>
      /// Map a DDL entity into a Data Asset.
      /// </summary>
      /// <param name="set">DDL Schema Set to map</param>
      /// <param name="namespaces">associate namespaces (if any)</param>
      /// <param name="rootName"></param>
      /// <returns>mapped schema set as Datat Asset entries</returns>
      public static AssetData ToDataAsset(
         DdlSchemaSet set, NamespaceList namespaces, string rootName,
         bool prepareTemplates = false)
      {
         DdlAssetInfo dassets = new DdlAssetInfo(set.Arguments.Namespace,
            AssetType.Schema, set.Arguments.Project.VersionId);

         var cats = set.Catalogs as List<SchemaObject.CatalogInfo>;
         dassets.Assets.Name = cats[0].Name;
         dassets.Assets.Title = Edam.Text.Convert.ToProperCase(cats[0].Name);
         dassets.Assets.Description = dassets.Assets.Title;

         String guidText = Guid.NewGuid().ToString();
         NamespaceInfo ns = namespaces == null || namespaces.Count == 0 ?
            new NamespaceInfo(guidText, "http://" + guidText) : namespaces[0];

         String nsText = ns.Uri.OriginalString;
         String dataOwnerId = Session.OrganizationId;
         //String catalogUri, schemaUri;
         int schemaCount = 0;
         int resourceCount = 0;
         foreach(var schema in set.Items)
         {
            var constraints = schema.Catalog.Constraints;
            //catalogUri = nsText + schema.Name;
            //schemaUri = catalogUri + "/" + schema.Name;

            schema.OrdinalNo = ++schemaCount;
            foreach (var resource in schema.Items)
            {
               resource.OrdinalNo = ++resourceCount;

               // add resources (tables)
               var ritem = AssetDataElement.ToAssetElement(ns, resource.Name);
               ritem.DataOwnerId = dataOwnerId;
               ritem.Root = ns.Prefix;
               ritem.Domain = schema.Name;
               ritem.SetFullPath(rootName + "." + schema.Name + "." +
                  resource.Name);
               ritem.Namespace = ns.Uri.OriginalString;
               ritem.AddAnnotation(Text.Convert.ToProperCase(resource.Name));
               dassets.Assets.Items.Add(ritem);

               ritem.Namespaces = namespaces;

               // add map/links to properties bag (if any)...
               PreparePropertiesBag(schema, resource, ritem);

               // add elements (columns)
               foreach (var element in resource.Items)
               {
                  if (set.DataTextMap != null)
                  {
                     element.DataType = set.DataTextMap.MapText(
                        element.DataType, DataTextMapDirection.From);
                     element.Name = set.DataTextMap.MapName(element.Name);
                  }

                  var eitem = element.ToAsset(resource, ns);
                  eitem.DataOwnerId = dataOwnerId;
                  eitem.Root = ns.Prefix;
                  eitem.Domain = schema.Name;
                  eitem.SetFullPath(ritem.FullPath + "." + element.Name);
                  eitem.Namespace = ns.Uri.OriginalString;
                  eitem.Namespaces = namespaces;
                  eitem.AddAnnotation(Text.Convert.ToProperCase(element.Name));
                  eitem.IsNillable = element.IsNullable;
                  eitem.EntityName = ritem.ElementName;
                  dassets.Assets.Items.Add(eitem);
               }
            }
         }

         if (prepareTemplates)
         {
            // preparing asset/s templates properties bag gets updated
            AssetDataTemplate.AssetToTemplate(dassets.Assets.Items);
         }

         return dassets.Assets;
      }

      /// <summary>
      /// Prepare Properties Bag...
      /// </summary>
      /// <param name="schema">table schema</param>
      /// <param name="resource">table details</param>
      /// <param name="element">table asset-data-element</param>
      public static void PreparePropertiesBag(
         DdlSchema schema, ResourceInfo resource, AssetDataElement element)
      {
         List<MapInfo> maps = GetMaps(schema, resource);
         ObjAssets.PropertiesBag bag = (ObjAssets.PropertiesBag)
            (element.PropertiesBag ?? new ObjAssets.PropertiesBag());
         if (bag.AssetTemplate == null)
         {
            bag.AssetTemplate = new ElementNodeInfo
            {
               ResourceName = schema.Name + "." + resource.Name,
               Title = Text.Convert.ToProperCase(resource.Name),
               TemplateType = ResourceType.View,
               GroupNo = schema.OrdinalNo,
               TemplateNo = resource.OrdinalNo
            };
            bag.AssetTemplate.Description = bag.AssetTemplate.Title;

            foreach (var item in resource.Items)
            {
               short.TryParse(item.DataSize, out var size);
               bag.AssetTemplate.Items.Add(new ElementItemInfo(
                  item.Name, DdlAssetInfo.GetType(item.DataType),
                  ResourceType.Column, item.IsKey ?
                     KeyType.Key : KeyType.NonKey, size));
            }
         }
         bag.AssetTemplate.Maps = maps;
         element.PropertiesBag = bag;
      }

      /// <summary>
      /// Given a schema and resource (table) find related foreign key
      /// constraints.
      /// </summary>
      /// <param name="schema">table schema</param>
      /// <param name="resource">table details</param>
      /// <returns>MapInfo instance is returned</returns>
      public static List<MapInfo> GetMaps(
         DdlSchema schema, ResourceInfo resource)
      {
         List<MapInfo> maps = new List<MapInfo>();

         if (schema.Catalog.Constraints == null)
         {
            return maps;
         }

         // find constraints related to the table
         var consts = schema.Catalog.Constraints.Where(
            x => x.TableSchema == schema.Name &&
               x.TableName == resource.Name).Select(x => x).
                  ToList<SchemaResourceConstraint>();

         string descriptionField = string.Empty;
         foreach (SchemaResourceConstraint constraint in consts)
         {
            MapInfo map = new MapInfo();
            map.Context.Namespace = schema.Namespace;

            map.Name = constraint.ConstraintName;
            map.ParentNodeName = constraint.ParentNodeName;
            map.ChildNodeName = constraint.ChildNodeName;

            map.AddLink(constraint.ReferencedColumnName, constraint.ColumnName,
               Text.Convert.ToProperCase(constraint.ColumnName),
               descriptionField);
            maps.Add(map);
         }

         return maps;
      }
      
      public static void ToFile(
         DdlSchemaSet set, AssetConsoleArgumentsInfo arguments,
         NamespaceList namespaces)
      {
         set.SetTextMap(arguments);
         AssetData asset = ToDataAsset(set, namespaces, arguments.Name);
         asset.Namespaces = namespaces;
         arguments.AssetDataItems.Add(asset);
         if (!arguments.ToFile)
         {
            return;
         }

         InOut.FileInfo f = new InOut.FileInfo();

         f.Path = arguments.OutFilePath;
         f.Name = String.Empty;
         f.Extension = InOut.FileExtension.CSV;

         asset.ToOutput(f, null);
      }

      public static void ToFile(AssetConsoleArgumentsInfo arguments)
      {
         var results = DdlSchemaInstance.FromRepository(arguments);
         if (!results.Success)
            return;
         var l = new NamespaceList();
         l.Add(new NamespaceInfo(arguments.UriPrefix, arguments.NamespaceUri));
         DdlSchemaInstance.ToFile(results.Data, arguments, l);
      }

      public static ResultsLog<DdlSchemaSet> FromFile(String fileFullPath)
      {
         String func = "FromFile";
         ResultsLog<DdlSchemaSet> results =
            new ResultsLog<DdlSchemaSet>();
         results.Failed(
            CLASS_NAME + "::" + func, EventCode.ClassOrObjectHasNotBeenDefined);
         return results;
      }

      /// <summary>
      /// Connect to a Database using given connection-string and get the
      /// schema.
      /// </summary>
      /// <param name="arguments"></param>
      /// <returns></returns>
      public static ResultsLog<DdlSchemaSet> FromRepository(
         AssetConsoleArgumentsInfo arguments)
      {
         String func = "FromRepository";
         ResultsLog<DdlSchemaSet> results =
            new ResultsLog<DdlSchemaSet>();
         try
         {
            var d = SchemaReader.GetCatalogs(arguments.ConnectionString,
               arguments.ProjectVersionId);
            if (d.Success)
            {
               results.Data = new DdlSchemaSet(d.Data, arguments);
               results.Succeeded();
            }
         }
         catch(Exception ex)
         {
            results.Failed(CLASS_NAME + "::" + func, ex);
         }
         return results;
      }

   }

}

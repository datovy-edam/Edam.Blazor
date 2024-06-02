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
using DocumentFormat.OpenXml.Wordprocessing;
using Edam.Data.Assets.AssetSchema;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

namespace Edam.Data.Schema.ImportExport
{

   public class DdlAsset
   {

      public AssetData asset { get; set; }
      public string tableName = null;
      public string originalTableName = null;
      private DataTextMap m_TextMapper { get; set; }

      public NamespaceInfo ns { get; set; }

      String nsText;
      String dataOwnerId = Session.OrganizationId;
      //String catalogUri, schemaUri;
      int schemaCount = 0;
      int resourceCount = 0;

      string catalogName;
      string schemaName = null;
      string prefix = String.Empty;

      ImportItemInfo assetTable = null;
      AssetElementInfo<IAssetElement> ritem = null;
      List<AssetElementInfo<IAssetElement>> children = null;
      public ImportItemInfo ImportItem { get; set; }

      public List<AssetElementInfo<IAssetElement>> Tables = 
         new List<AssetElementInfo<IAssetElement>>();

      public DdlAsset(ImportItemInfo header, NamespaceList namespaces,
         NamespaceInfo ns, DataTextMap mapper, string schemaName, 
         string versionId, int schemaCount = 0)
      {
         asset = new AssetData(ns, AssetType.Schema, versionId);

         asset.Name = header.TableCatalog;
         asset.Title = Edam.Text.Convert.ToProperCase(header.TableCatalog);
         asset.Description = asset.Title;
         asset.Namespaces = namespaces;

         nsText = ns.Uri.OriginalString;
         catalogName = asset.Name;
         m_TextMapper = mapper;

         // the following will be replaced upon registration
         this.ns = ns;

         if (!String.IsNullOrWhiteSpace(schemaName))
         {
            RegisterNamespace(schemaCount, schemaName);
         }
      }

      #region -- 4.00 - Manage Namespace

      /// <summary>
      /// Register Namespace
      /// </summary>
      /// <param name="schemaCount"></param>
      /// <param name="schemaName"></param>
      private void RegisterNamespace(int schemaCount, string schemaName)
      {
         this.schemaCount = schemaCount;
         prefix = ns.Prefix + schemaCount.ToString();
         asset.SchemaName = schemaName;

         string sUri = ns.Uri.OriginalString + "/" + schemaName;

         NamespaceInfo schemaNs = new NamespaceInfo(prefix, sUri);
         this.ns = schemaNs;

         asset.Namespaces.Add(schemaNs);
      }

      #endregion
      #region -- 4.00 - Properties Bag Support

      public void PreparePropertyBag(int schemaOrdinalNo,
         ImportItemInfo item, List<AssetElementInfo<IAssetElement>> children,
         AssetDataElement element)
      {
         if (children == null)
         {
            return;
         }

         List<MapInfo> maps = GetMaps(item);
         ObjAssets.PropertiesBag bag = (ObjAssets.PropertiesBag)
            (element.PropertiesBag ?? new ObjAssets.PropertiesBag());
         if (bag.AssetTemplate == null)
         {
            bag.AssetTemplate = new ElementNodeInfo
            {
               ResourceName = item.TableSchema + "." + item.TableName,
               Title = Text.Convert.ToProperCase(item.TableName),
               TemplateType = ResourceType.View,
               GroupNo = schemaOrdinalNo,
               TemplateNo = element.OrdinalNo
            };
            bag.AssetTemplate.Description = bag.AssetTemplate.Title;

            foreach (var child in children)
            {
               // element already added? if so merge key info...
               var citem = bag.AssetTemplate.Items.Find(
                  (x) => x.Name == child.ElementName);
               if (citem != null)
               {
                  if (child.KeyType == ConstraintType.key)
                  {
                     citem.KeyType = KeyType.Key;
                  }
                  continue;
               }

               bag.AssetTemplate.Items.Add(new ElementItemInfo(
                  child.ElementName, DdlAssetInfo.GetType(item.DataType),
                  ResourceType.Column, child.KeyType == ConstraintType.key ?
                     KeyType.Key : KeyType.NonKey, child.MaxLength));
            }
         }
         bag.AssetTemplate.Maps = maps;
         element.PropertiesBag = bag;
      }

      private static List<MapInfo> GetMaps(ImportItemInfo item)
      {
         List<MapInfo> maps = new List<MapInfo>();

         if (!item.HasForeignKey)
         {
            return maps;
         }

         MapInfo map = new MapInfo();
         map.Context.Namespace = item.SchemaNamespace;

         map.Name = "fk_" + item.ConstraintTableName;
         map.ParentNodeName = item.TableName;
         map.ChildNodeName = item.TableName;

         string descriptionField = string.Empty;
         map.AddLink(item.ColumnName, item.ConstraintColumnName,
            Text.Convert.ToProperCase(item.ConstraintColumnName),
            descriptionField);
         maps.Add(map);

         return maps;
      }

      #endregion

      public void UpdateEntityProperty(ElementPropertyInfo property)
      {
         if (property != null && 
            !String.IsNullOrWhiteSpace(property.PropertyValue))
         {
            ritem.Annotation.Clear();
            ritem.AddAnnotation(property.PropertyValue);
         }
      }

      public static void UpdateEntityProperty(ElementPropertyInfo property,
         AssetElementInfo<IAssetElement> item)
      {
         if (property != null &&
            !String.IsNullOrWhiteSpace(property.PropertyValue))
         {
            item.Kind = DataElementKind.ExternalReference;
            string text = item.Annotation.Count > 0 ?
               item.AnnotationText : String.Empty;

            item.Annotation.Clear();
            item.AddAnnotation(text + "... " + property.PropertyValue);
         }
      }

      #region -- 4.00 - Prepare Type Definition

      public static AssetElementInfo<IAssetElement> PrepareTypeDefinition(
         ElementType objectType,
         string schemaName, string tableName, string tableOriginalName,
         string domain, string dataOwnerId, NamespaceInfo ns)
      {
         // prepare table resource
         AssetElementInfo<IAssetElement> ritem = 
            AssetDataElement.ToAssetElement(ns, tableName);

         string entityPath;
         string schemaPath;
         if (String.IsNullOrWhiteSpace(schemaName))
         {
            entityPath = "";
            schemaPath = String.Empty;
         }
         else
         {
            entityPath = ns.Prefix + ":" + schemaName;
            schemaPath = entityPath + "/";
         }

         ritem.DataOwnerId = dataOwnerId;
         ritem.Root = ns.NamePath.Root;
         ritem.Domain = domain;
         ritem.SetFullPath(schemaPath + ns.Prefix + ":" + tableName);
         ritem.EntityPath = entityPath;
         ritem.Namespace = ns.Uri.OriginalString;
         ritem.AddAnnotation(Text.Convert.ToProperCase(tableName));
         ritem.OriginalName = tableOriginalName;

         switch(objectType)
         {
            case ElementType.view:
               ritem.ElementType = ElementType.view;
               break;
            case ElementType.procedure:
               ritem.ElementType = ElementType.procedure;
               break;
            case ElementType.function:
               ritem.ElementType = ElementType.function;
               break;
            default:
               break;
         }

         return ritem;
      }

      public void PrepareTableDefinition(ImportItemInfo item,
         string tableOriginalName)
      {
         ImportItem = item;
         item.SchemaNamespace = ns;
         assetTable = item;

         var appSettings = AppAssembly.FetchInstance(
            AssetResourceHelper.ASSET_APP_SETTINGS) as IAppSettings;
         string typePostfix = appSettings == null ?
            String.Empty : appSettings.GetTypePostfix();

         string tName = item.TableName + typePostfix;
         ritem = PrepareTypeDefinition(item.ObjectType,
            item.TableSchema, tName, tableOriginalName, item.TableSchema, 
            dataOwnerId, ns);
         ritem.Item = item;
         ritem.OriginalName = tableOriginalName;

         asset.Add(ritem);
         tableName = tName;
         resourceCount++;
         Tables.Add(ritem);
         children = new List<AssetElementInfo<IAssetElement>>();
      }

      #endregion

      private AssetElementInfo<IAssetElement> ToAssetElement(
         AssetElementInfo<IAssetElement> parent, ImportItemInfo item, 
         NamespaceInfo ns, bool useItemType = false,
         bool isRootDocument = false)
      {
         string dataType;
         string originalDataType;
         string elementName;
         string elementOriginalName;

         var appSettings = AppAssembly.FetchInstance(
            AssetResourceHelper.ASSET_APP_SETTINGS) as IAppSettings;
         string typePostfix = appSettings == null ?
            String.Empty : appSettings.GetTypePostfix();
         string docPostfix = appSettings == null ?
            String.Empty : appSettings.GetDocumentPostfix();

         if (!useItemType)
         {
            var dtype = m_TextMapper.MapText(
               item.DataType, DataTextMapDirection.From);
            dataType = ns.MapItem(dtype);
            originalDataType = item.DataType;
            elementName = item.ColumnName;
            elementOriginalName = item.ColumnName;
         }
         else if (isRootDocument)
         {
            dataType = ns.Prefix + ":" + item.TableName + typePostfix;
            originalDataType = item.TableName;
            elementName = ns.Prefix + ":" + item.TableName + docPostfix;
            elementOriginalName = item.TableName;
         }
         else
         {
            elementName = ns.Prefix + ":" + item.TableName;
            elementOriginalName = item.TableName;
            dataType = elementName + typePostfix;
            originalDataType = elementName;
         }

         // TODO: store AssetDataElement precision and scale in the database

         // prepare asset element...
         QualifiedNameInfo typeQName = new QualifiedNameInfo(dataType);
         AssetElementInfo<IAssetElement> a = new AssetElementInfo<IAssetElement>
         {
            DataType = dataType,
            Namespace = ns.Uri.OriginalString,
            EntityQualifiedName = parent == null ?
               null : new QualifiedNameInfo(
                  parent.ElementQualifiedName.Prefix, parent.ElementName),
            MinOccurrence = 0,
            MaxOccurrence = 1,
            Occurs = "(" + (item.IsPrimaryKey ? "1" : "0") + ":1)",
            ElementQualifiedName =
               new QualifiedNameInfo(ns.Prefix, elementName),
            ElementType = ElementType.element,
            KeyType = item.IsPrimaryKey ?
               ConstraintType.key : ConstraintType.nonkey,
            AutoGenerateType = item.IsIdentity ?
               ConstraintType.autoGenerate : ConstraintType.none,
            IsNillable = !item.IsPrimaryKey,
            TypeQualifiedName = typeQName,
            OriginalName = elementOriginalName,
            OriginalDataType = originalDataType,
            Tags = item.Tags == null ? String.Empty : item.Tags,
            Precision = item.Precision.HasValue ? item.Precision.Value : 0,
            Scale = item.Scale.HasValue ? item.Scale.Value : 0
         };

         a.Annotation.Clear();
         if (!String.IsNullOrWhiteSpace(item.ColumnDescription))
         {
            a.AddAnnotation(item.ColumnDescription);
         }
         else
         {
            a.AddAnnotation(Text.Convert.ToProperCase(
               a.ElementQualifiedName.OriginalName));
         }

         a.QualifiedTypeNames.Add(typeQName);
         a.Length = item.CharacterMaximumLength;

         // manage constraints...
         if (item.HasForeignKey)
         {
            a.Constraints.Add("fk_" + (a.Constraints.Count + 1).ToString(),
               AssetElementContraintType.ForeignKey, "", 
               item.ConstraintTableSchema,
               item.ConstraintTableName, item.ConstraintColumnName);
         }

         a.Item = item;

         return a;
      }

      /// <summary>
      /// Prepare Element Definition using given info.
      /// </summary>
      /// <param name="ritem"></param>
      /// <param name="item"></param>
      /// <param name="useItemType"></param>
      /// <param name="isRootDocument"></param>
      /// <returns></returns>
      public AssetElementInfo<IAssetElement> PrepareElementDefinition(
         AssetElementInfo<IAssetElement> ritem, ImportItemInfo item,
         bool useItemType = false, bool isRootDocument = false)
      {
         var eitem = ToAssetElement(
            ritem, item, ns, useItemType, isRootDocument);

         eitem.DataOwnerId = dataOwnerId;
         eitem.Root = ns.NamePath.Root;
         eitem.Domain = item.TableSchema;
         eitem.SetFullPath((ritem == null ? String.Empty : ritem.FullPath + "/")
            + eitem.ElementName.Trim());
         eitem.EntityPath = ritem == null ? String.Empty : ritem.FullPath;
         eitem.Namespace = ns.Uri.OriginalString;
         eitem.Namespaces = new NamespaceList();
         eitem.Namespaces.AddRange(asset.Namespaces);
         eitem.IsNillable = !item.IsPrimaryKey;

         if (item.IsIdentity)
         {
            eitem.AutoGenerateType = ConstraintType.autoGenerate;
         }

         if (!useItemType)
         {
            eitem.EntityName = ritem.ElementName.Trim();
         }

         return eitem;
      }

      /// <summary>
      /// Prepare Column Definition.
      /// </summary>
      /// <param name="item"></param>
      /// <returns></returns>
      public AssetElementInfo<IAssetElement> PrepareColumnDefinition(
         ImportItemInfo item)
      {
         item.TableName = item.TableName.Trim();
         item.ColumnName = item.ColumnName.Trim();

         var eitem = PrepareElementDefinition(ritem, item);

         asset.Add(eitem);
         var citem = children.Find((x) => x.ElementName == eitem.ElementName);
         if (citem == null)
         {
            children.Add(eitem);
         }
         return eitem;
      }

      /// <summary>
      /// Prepare Additional Columns (like Diagnostics columns [if any])
      /// </summary>
      public void PrepareAdditionalColumns()
      {
         if (m_TextMapper == null || m_TextMapper.ElementProperty == null)
         {
            return;
         }

         if (ritem == null)
         {
            return;
         }

         if (ritem.ElementType == ElementType.function ||
            ritem.ElementType == ElementType.procedure ||
            ritem.ElementType == ElementType.view)
         {
            return;
         }

         ImportItemInfo item = new ImportItemInfo();
         decimal? maxLength = null;

         item.Dbms = "";
         item.TableCatalog = asset.Name;
         item.TableSchema = ritem.Domain;
         item.TableName = tableName;

         foreach(var eprop in m_TextMapper.ElementProperty)
         {
            foreach(var eitem in eprop.RecordTrackingItem)
            {
               maxLength = null;
               if (decimal.TryParse(eitem.DataSize, out var dataSize))
               {
                  maxLength = dataSize;
               }

               // does the child exists? if so update info and continue...
               var child = children.Find((x) => x.OriginalName == eitem.Name);
               if (child != null)
               {
                  if (String.IsNullOrWhiteSpace(eitem.Description))
                  {
                     child.Annotation.Clear();
                     child.AddAnnotation(eitem.Description);
                  }
                  child.Kind = eitem.Kind;
                  continue;
               }

               // item was not found, add it...
               item.ColumnName = eitem.Name;
               item.DataType = eitem.TypeName;
               if (maxLength.HasValue)
               {
                  item.CharacterMaximumLength = maxLength.Value;
               }
               var element = PrepareColumnDefinition(item);
               element.Description = eitem.Description;
               element.Kind = eitem.Kind;
            }
         }
      }

      /// <summary>
      /// Add Property Bag...
      /// </summary>
      public void AddPropertyBag()
      {
         if (ritem != null)
         {
            PreparePropertyBag(schemaCount, assetTable, children, ritem);
         }
      }
   }

}

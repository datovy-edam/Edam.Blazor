using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

// -----------------------------------------------------------------------------
using Edam.Application;
using Edam.Data.Asset;
using Edam.Data.AssetManagement;
using Edam.Data.AssetSchema;

namespace Edam.Data.DataItem
{

   public class AssetItem
   {
      public string BusinessArea { get; set; }
      public List<ItemInstanceTypeInfo> Children { get; set; } = 
         new List<ItemInstanceTypeInfo>();
   }

   public class AssetItemCollection
   {

      #region -- 1.00 - Properties and Fields

      private AssetType m_AssetType;

      private string m_VersionId;
      public string VersionId
      {
         get { return m_VersionId; }
      }

      private string m_TypePostfix;
      public string TypePostfix
      {
         get { return m_TypePostfix; }
      }

      private string m_DocumentPostfix;
      public string DocumentPostfix
      {
         get { return m_DocumentPostfix; }
      }

      public DataTextMap TextMapper { get; set; }

      private string m_DataOwnerId;
      public string DataOwnerId
      {
         get { return m_DataOwnerId; }
      }

      public NamespaceList Namespaces
      {
         get { return Asset.Namespaces; }

      }
      public NamespaceInfo DefaultNamespace
      {
         get { return Asset.DefaultNamespace; }
      }

      public AssetData Asset { get; set; }
      public AssetDataElementList Items { get; set; }
      public int BusinessAreaCount { get; set; } = 0;

      #endregion
      #region -- 1.50 - Constructor and Initialization

      public AssetItemCollection(string assetName,
         NamespaceInfo ns, AssetType type = AssetType.Asset,
         NamespaceList namespaces = null, DataTextMap mapper = null, 
         string versionId = null)
      {
         Asset = new AssetData(ns, AssetType.Asset, versionId);

         Asset.Name = assetName;
         Asset.Title = Edam.Text.Convert.ToProperCase(assetName);
         Asset.Description = Asset.Title;

         Asset.DefaultNamespace = ns;
         Asset.Namespaces = namespaces == null ?
            new NamespaceList() : namespaces;
         Asset.Namespaces.Add(ns);

         m_DataOwnerId = Session.OrganizationId;
         m_AssetType = type;
         m_VersionId = versionId;
         TextMapper = mapper;

         // set postfixes... for type and document declarations
         var appSettings = AppAssembly.FetchInstance(
            AssetResourceHelper.ASSET_APP_SETTINGS) as IAppSettings;

         m_TypePostfix = appSettings == null ?
            String.Empty : appSettings.GetTypePostfix();
         m_DocumentPostfix = appSettings == null ?
            String.Empty : appSettings.GetDocumentPostfix();

         Items = new AssetDataElementList(ns, type, versionId);
      }

      #endregion
      #region -- 4.00 - Properties Bag Support

      /*
      public void PreparePropertyBag(int schemaOrdinalNo,
         ItemInstanceTypeInfo item, AssetDataElementList children,
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

      private static List<MapInfo> GetMaps(DdlImportItemInfo item)
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
       */

      #endregion
      #region -- 4.00 - Prepare Type Definition

      /// <summary>
      /// Prepare Asset Data Element type instance.
      /// </summary>
      /// <param name="bussinesAreaName">business area name, similar to a 
      /// database schema</param>
      /// <param name="typeName">type name</param>
      /// <param name="originalName">original type name (if different)</param>
      /// <param name="domain">business domain</param>
      /// <param name="dataOwnerId">data owner id</param>
      /// <param name="ns">namespace info</param>
      public static AssetDataElement PrepareTypeDefinition(
         string bussinesAreaName, string typeName, string originalName,
         string domain, string dataOwnerId, NamespaceInfo ns)
      {
         AssetDataElement etype = AssetDataElement.ToAssetElement(ns, typeName);

         string entityPath;
         string areaPath;
         if (String.IsNullOrWhiteSpace(bussinesAreaName))
         {
            entityPath = "";
            areaPath = String.Empty;
         }
         else
         {
            entityPath = ns.Prefix + ":" + bussinesAreaName;
            areaPath = entityPath + "/";
         }

         etype.DataOwnerId = dataOwnerId;
         etype.Root = ns.NamePath.Root;
         etype.Domain = domain;
         etype.EntityPath = entityPath;
         etype.OriginalName = originalName;

         etype.SetFullPath(areaPath + ns.Prefix + ":" + typeName);
         etype.AddAnnotation(Text.Convert.ToProperCase(typeName));

         return etype;
      }

      /// <summary>
      /// Prepare a type definition.
      /// </summary>
      /// <param name="item">item instance to use in the preparation</param>
      /// <param name="originalName">(optional) original item name</param>
      public AssetDataElement PrepareTypeDefinition(ItemInstanceTypeInfo item,
         string originalName = null, bool addItem = true)
      {
         string origName = String.IsNullOrWhiteSpace(originalName) ? 
            item.Name : originalName;

         item.Namespace = DefaultNamespace;

         string tName = item.Name + m_TypePostfix;
         var element = PrepareTypeDefinition(
            item.BusinessAreaName, tName, origName, item.BusinessAreaName,
            m_DataOwnerId, DefaultNamespace);

         if (addItem)
         {
            item.Element = element;
            Items.Add(element);
         }

         return element;
      }

      /// <summary>
      /// Add Property Bag... for given type.
      /// </summary>
      public void AddPropertyBag(ItemInstanceTypeInfo type)
      {
         if (type != null)
         {
            // TODO: implement properties bag...
            //PreparePropertyBag(
            //   BusinessAreaCount, type, type.Children, type.Element);
         }
      }

      #endregion
      #region -- 4.00 - Prepare Element Definition

      /// <summary>
      /// To Asset Element.
      /// </summary>
      /// <param name="parent"></param>
      /// <param name="item"></param>
      /// <param name="ns"></param>
      /// <param name="useItemType"></param>
      /// <param name="isRootDocument"></param>
      /// <returns></returns>
      private AssetDataElement ToAssetElement(
         AssetDataElement parent, ItemInstanceItemInfo item,
         NamespaceInfo ns, bool useItemType = false,
         bool isRootDocument = false)
      {
         string dataType;
         string originalDataType;
         string elementName;
         string elementOriginalName;

         string parentName = item.Parent == null ? item.Name : item.Parent.Name;

         if (!useItemType)
         {
            var dtype = TextMapper.MapText(
               item.DataType, DataTextMapDirection.From);
            dataType = ns.MapItem(dtype);
            originalDataType = item.DataType;
            elementName = item.Name;
            elementOriginalName = item.Name;
         }
         else if (isRootDocument)
         {
            dataType = ns.Prefix + ":" + parentName + TypePostfix;
            originalDataType = parentName;
            elementName = ns.Prefix + ":" + parentName;
            elementOriginalName = parentName;
         }
         else
         {
            elementName = ns.Prefix + ":" + item.Name;
            elementOriginalName = item.Name;
            dataType = elementName + TypePostfix;
            originalDataType = elementName;
         }

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
            Tags = item.Tags == null ? String.Empty : item.Tags
         };

         if (item.Type == ItemInstanceType.Array || item.MaxOccursUnbounded)
         {
            a.Occurs = item.IsRequired ? "(1:*)" : "(0:*)";
         }
         else
         {
            a.Occurs = item.IsRequired ? "(1:1)" : "(0:1)";
         }

         a.Annotation.Clear();
         if (!String.IsNullOrWhiteSpace(item.Description))
         {
            a.AddAnnotation(item.Description);
         }
         else
         {
            a.AddAnnotation(Text.Convert.ToProperCase(
               a.ElementQualifiedName.OriginalName));
         }

         a.QualifiedTypeNames.Add(typeQName);
         a.Length = item.MaximumLength;

         a.Item = item;

         return a;
      }

      /// <summary>
      /// Prepare Element Definition
      /// </summary>
      /// <param name="parent"></param>
      /// <param name="item"></param>
      /// <param name="useItemType"></param>
      /// <param name="isRootDocument"></param>
      /// <returns></returns>
      public AssetDataElement PrepareElementDefinition(
         AssetDataElement parent, ItemInstanceItemInfo item,
         bool useItemType = false, bool isRootDocument = false)
      {
         var eitem = ToAssetElement(
            parent, item, DefaultNamespace, useItemType, isRootDocument);

         eitem.DataOwnerId = m_DataOwnerId;
         eitem.Root = DefaultNamespace.NamePath.Root;
         eitem.Domain = item.BusinessAreaName;

         eitem.SetFullPath((
            parent == null ? String.Empty : parent.FullPath + "/")
               + eitem.ElementName.Trim());
         eitem.EntityPath = parent == null ? String.Empty : parent.FullPath;
         eitem.Namespace = DefaultNamespace.Uri.OriginalString;
         eitem.Namespaces = new NamespaceList();
         eitem.Namespaces.AddRange(Asset.Namespaces);
         eitem.IsNillable = !item.IsPrimaryKey;

         if (item.IsIdentity)
         {
            eitem.AutoGenerateType = ConstraintType.autoGenerate;
         }

         if (!useItemType)
         {
            eitem.EntityName = parent.ElementName.Trim();
         }

         return eitem;
      }

      /// <summary>
      /// Prepare Element Definiton for given item.
      /// </summary>
      /// <param name="item">instance of ItemInstanceItemInfo</param>
      /// <returns>returns an AssetDataElement instance</returns>
      public AssetDataElement PrepareElementDefinition(
         ItemInstanceItemInfo item)
      {
         item.Name = item.Name.Trim();

         var eitem = PrepareElementDefinition(
            item.Parent == null ? null : item.Parent.Element, item);
         item.Element = eitem;
         Items.Add(eitem);

         return eitem;
      }

      /// <summary>
      /// Prepare Additional Elements... for given type.
      /// </summary>
      /// <param name="type">given type</param>
      public void PrepareAdditionalElements(ItemInstanceTypeInfo type)
      {
         if (TextMapper == null || TextMapper.ElementProperty == null)
         {
            return;
         }

         if (type == null)
         {
            return;
         }

         decimal? maxLength = null;

         ItemInstanceItemInfo item = new ItemInstanceItemInfo();

         foreach (var eprop in TextMapper.ElementProperty)
         {
            foreach (var eitem in eprop.RecordTrackingItem)
            {
               maxLength = null;
               if (decimal.TryParse(eitem.DataSize, out var dataSize))
               {
                  maxLength = dataSize;
               }

               // does the child exists? if so update info and continue...
               var child = type.Children.Find(
                  (x) => x.OriginalName == eitem.Name);
               if (child != null)
               {
                  if (String.IsNullOrWhiteSpace(eitem.Description))
                  {
                     child.Element.Annotation.Clear();
                     child.Element.AddAnnotation(eitem.Description);
                  }
                  child.Kind = eitem.Kind;
                  continue;
               }

               // item was not found, add it...
               item.Name = eitem.Name;
               item.DataType = eitem.TypeName;
               if (maxLength.HasValue)
               {
                  item.MaximumLength = maxLength.Value;
               }
               var element = PrepareElementDefinition(item);
               element.Description = eitem.Description;
               element.Kind = eitem.Kind;
            }
         }
      }

      #endregion

   }

}

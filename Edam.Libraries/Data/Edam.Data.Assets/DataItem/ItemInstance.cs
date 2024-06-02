using Edam.Application;
using Edam.Data.Asset;
using Edam.Data.AssetConsole;
using Edam.Data.AssetManagement;
using Edam.Data.Assets.AssetSchema;
using Edam.Data.AssetSchema;
using Edam.DataObjects.Trees;
using Edam.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.DataItem
{

   public class ItemInstance
   {

      #region -- 1.00 - Properties and Fields...

      public AssetItemCollection AssetItem;

      public Dictionary<string, ItemInstanceTypeInfo> Types { get; set; } =
         new Dictionary<string, ItemInstanceTypeInfo>();

      public Dictionary<string, ItemInstanceItemInfo> Elements { get; set; } =
         new Dictionary<string, ItemInstanceItemInfo>();
      
      #endregion
      #region -- 1.50 - Constructor and Initialization

      public ItemInstance(
         string assetName, NamespaceInfo ns, AssetType type = AssetType.Asset,
         NamespaceList namespaces = null, DataTextMap mapper = null,
         string versionId = null)
      {
         AssetItem = new AssetItemCollection(
            assetName, ns, type, namespaces, mapper, versionId);
      }

      #endregion

      /// <summary>
      /// Find type by name.
      /// </summary>
      /// <param name="typeName">type name to find.</param>
      /// <returns>if found instance of ItemInstanceTypeInfo is returned else
      /// null</returns>
      public ItemInstanceTypeInfo Find(string typeName)
      {
         if (String.IsNullOrWhiteSpace(typeName))
         {
            return null;
         }
         return Types.TryGetValue(typeName, out var type) ? type : null;
      }

      /// <summary>
      /// Prepare Asset Data List.
      /// </summary>
      /// <param name="rootItem">root item</param>
      /// <param name="namespaces">namespaces</param>
      /// <param name="rootName">root </param>
      /// <returns>instance of AssetData is returned</returns>
      public AssetData ToAssetData(ItemInstanceTypeInfo rootItem)
      {
         if (rootItem == null || rootItem.Children.Count == 0)
         {
            return null;
         }

         var header = rootItem;

         // register all types...
         foreach (var item in Types.Values)
         {
            AssetItem.PrepareTypeDefinition(item);
            foreach(var child in item.Children)
            {
               AssetItem.PrepareElementDefinition(child);
            }
            AssetItem.PrepareAdditionalElements(item);
            AssetItem.AddPropertyBag(item);
         }

         // prepare catalog document...
         var documentItems = PrepareDocument(
            header, AssetItem.DefaultNamespace);

         foreach (var item in documentItems)
         {
            AssetItem.Items.Add(item);
         }

         AssetItem.Asset.Items.AddRange(AssetItem.Items);

         return AssetItem.Asset;
      }

      /// <summary>
      /// Prepare Document
      /// </summary>
      /// <param name="root"></param>
      /// <param name="ns"></param>
      /// <returns></returns>
      private AssetDataElementList PrepareDocument(
         ItemInstanceTypeInfo root, NamespaceInfo ns)
      {
         // prepare root element
         AssetDataElementList documentList =
            new AssetDataElementList(ns, AssetType.Schema, AssetItem.VersionId);

         // manage entry line numbers
         int index = AssetItem.Items.Count;
         // add document type
         var documentSchema =
            AssetItem.PrepareTypeDefinition(root, root.Name, false);
         documentList.Add(documentSchema);

         documentSchema.OrdinalNo = index;
         index++;

         // prepare type that include all registered schemas... add child types
         ItemInstanceItemInfo itm = null;
         AssetDataElement ditem = null;
         foreach (var child in root.Children)
         {
            // find child type
            var typeItem = Find(child.Name);
            if (typeItem == null)
            {
               continue;
            }

            // prepare an element per root child
            itm = new ItemInstanceItemInfo();
            itm.Name = typeItem.Name;
            itm.Type = typeItem.Type;
            itm.Parent = root;
            itm.Path = typeItem.Path;
            itm.SetName();
            itm.DataType = typeItem.Name + AssetItem.DocumentPostfix;

            ditem = AssetItem.PrepareElementDefinition(
               documentSchema, itm, true, false);
            ditem.Domain = root.BusinessAreaName;
            documentList.Add(ditem);
            ditem.OrdinalNo = index;

            index++;
         }

         // add schema as a child of document
         ItemInstanceItemInfo ritm = new ItemInstanceItemInfo();
         ritm.Name = root.Name;
         ritm.Type = root.Type;
         ritm.Parent = null;
         ritm.Path = root.Path;
         ritm.SetName();

         var rootDocument = AssetItem.PrepareElementDefinition(
            null, ritm, true, true);

         rootDocument.Domain = root.BusinessAreaName;

         documentList.Add(rootDocument);
         rootDocument.OrdinalNo = index;

         return documentList;
      }

   }

}

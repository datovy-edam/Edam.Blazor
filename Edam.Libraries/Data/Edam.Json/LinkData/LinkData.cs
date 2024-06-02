using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetSchema;

namespace Edam.Json.LinkData
{

   public class LinkData
   {

      private readonly List<AssetDataElement> m_ElementTypes = 
         new List<AssetDataElement>();
      private readonly Dictionary<string, List<LinkDataItemInfo>> m_Items;
      public Dictionary<string, List<LinkDataItemInfo>> Items
      {
         get { return m_Items; }
      }

      public List<List<LinkDataItemInfo>> GetDictionaryItems()
      {
         Dictionary<string, List<LinkDataItemInfo>>.ValueCollection values = 
            m_Items.Values;
         return values.ToList();
      }

      public LinkData()
      {
         m_Items = new Dictionary<string, List<LinkDataItemInfo>>();
      }

      /// <summary>
      /// Sort link-data items by namespace...
      /// </summary>
      /// <param name="item"></param>
      /// <returns></returns>
      public static List<LinkDataItemInfo> SortByNamespace(
         List<LinkDataItemInfo> item)
      {
         var l = from i in item
                 orderby i.Namespace.Uri.AbsoluteUri ascending
                 select i;
         return l.ToList();
      }

      /// <summary>
      /// Based on namespace, try to find related items collection;
      /// </summary>
      /// <param name="namespaceText">instance of NamespaceInfo</param>
      /// <returns>list of link-data item-info is returned if found else null
      /// </returns>
      public List<LinkDataItemInfo> Find(string namespaceText)
      {
         if (m_Items.TryGetValue(
            namespaceText, out List<LinkDataItemInfo> item))
         {
            return item;
         }
         return null;
      }

      public static string GetCamelCaseName(string name)
      {
         return char.ToLower(name[0]) + name.Substring(1);
      }

      public static string GetCamelCaseName(IDataElement asset)
      {
         return GetCamelCaseName(GetItemName(asset));
      }

      private static string GetItemName(IDataElement asset)
      {
         return asset.ElementQualifiedName.OriginalName;
      }

      /// <summary>
      /// Add item to collection if not already there.
      /// </summary>
      /// <param name="asset">asset to add</param>
      public void Add(AssetDataElement asset)
      {
         if (String.IsNullOrWhiteSpace(asset.EntityName))
         {
            if (asset.IsType || asset.IsRoot)
            {
               m_ElementTypes.Add(asset);
            }
            //return;
         }

         // find namespace item (collection) reference in dictionary
         var item = Find(asset.EntityName);
         if (item == null)
         {
            item = new List<LinkDataItemInfo>();
            m_Items.Add(asset.EntityName, item);
         }

         // with current item (collection) add asset as needed
         string itemName = GetItemName(asset);
         var link = item.Find((x) =>
            x.ItemName == itemName && x.LinkName == asset.ElementName);
         if (link == null)
         {
            var etype = m_ElementTypes.Find(
               (x) => x.ElementName == asset.EntityName);

            NamespaceInfo ns = asset.ElementNamespace;
            if (ns == null)
            {
               ns = new NamespaceInfo(asset.ElementQualifiedName.Prefix,
                  asset.Namespace);
            }

            link = new LinkDataItemInfo
            {
               Namespace = ns,
               EntityName = asset.EntityName,
               ItemName = itemName,
               LinkName = asset.ElementName,
               ElementType = etype,
               Asset = asset
            };

            item.Add(link);
         }
      }

   }

}

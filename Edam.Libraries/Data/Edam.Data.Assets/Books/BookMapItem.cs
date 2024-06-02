using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edam.Data.AssetConsole;

// -----------------------------------------------------------------------------
using Edam.Data.AssetSchema;
using Edam.Data.AssetUseCases;
using Edam.Data.Books;

namespace Edam.Data.Assets.Books
{

   public class BookMapItemInfo
   {
      public int Index { get; set; }
      public AssetDataMapItem DataMapItem { get; set; }
      public MapItemInfo MapItem { get; set; }
      public AssetDataElement Element { get; set; }
      public BookletInfo Booklet { get; set; }
      public bool Visited { get; set; } = false;
   }

   public class BookMapItem
   {

      public List<BookMapItemInfo> SourceItems { get; set; } = 
         new List<BookMapItemInfo>();
      public List<BookMapItemInfo> TargetItems { get; set; } =
         new List<BookMapItemInfo>();
      public bool Visited { get; set; } = false;

   }

   /// <summary>
   /// Implement Book Map-Items list and provide helper methods.
   /// </summary>
   public class BookMapItemList : List<BookMapItem>
   {

      public BookMapItemList() : base()
      {

      }

      /// <summary>
      /// Given an Asset Use Case Map and a Book Mapper
      /// </summary>
      /// <param name="map">Use Case map to generate Use Case from</param>
      /// <param name="mapper">mapping information</param>
      /// <returns>instance of Book Map Items is returned</returns>
      public static BookMapItemList PrepareList(
         AssetUseCaseMap map, BookMapper mapper)
      {
         // get the list of all book - map - items
         BookMapItemList items = ToBookMapItems(null, map, mapper);

         // put those in kind of order based on individual map-item elements
         return OrderItems(items);
      }

      /// <summary>
      /// Given an Asset Use Case Map prepare a related Book Map Items list...
      /// </summary>
      /// <param name="list">use given BookMapItemList instance, if null
      /// an instance will be created and returned</param>
      /// <param name="map">Use Case map to generate Use Case from</param>
      /// <param name="mapper">mapping information</param>
      /// <returns>instance of Book Map Items is returned</returns>
      public static BookMapItemList ToBookMapItems(
         BookMapItemList list, AssetUseCaseMap map, BookMapper mapper)
      {
         // look over every map-item that defines source and target elements
         int indexCount = 0;
         BookMapItemList items = list ?? new BookMapItemList();
         foreach (var i in map.Items)
         {
            BookMapItem bookMapItem = new BookMapItem();

            // first, identify the source and target elements
            foreach (var sitem in i.SourceElement)
            {
               var element = mapper.FindElement(sitem.Path);
               sitem.DataElement = element;
               var booklet = mapper.FindBooklet(sitem.ItemId);

               BookMapItemInfo bMapItem = new BookMapItemInfo
               {
                  Index = indexCount,
                  DataMapItem = i,
                  MapItem = sitem,
                  Element = element,
                  Booklet = booklet
               };
               bookMapItem.SourceItems.Add(bMapItem);
            }

            foreach (var titem in i.TargetElement)
            {
               var element = mapper.FindElement(i.ItemPath);
               titem.DataElement = element;
               var booklet = mapper.FindBooklet(titem.ItemId);

               BookMapItemInfo bMapItem = new BookMapItemInfo
               {
                  Index = indexCount,
                  DataMapItem = i,
                  MapItem = titem,
                  Element = element,
                  Booklet = booklet
               };
               bookMapItem.TargetItems.Add(bMapItem);
            }
            items.Add(bookMapItem);

            indexCount++;
         }

         return items;
      }

      /// <summary>
      /// Find an item withing the source or target lists...
      /// </summary>
      /// <param name="list">list of items already visited</param>
      /// <param name="item">map item to search for base on its path</param>
      /// <param name="type">list to search (source or target)</param>
      /// <returns>if found a BookMapItem instance is returned</returns>
      public static BookMapItem Find(
         BookMapItemList list, BookMapItemInfo item, MapItemType type)
      {
         foreach(var map in list)
         {
            if (type == MapItemType.Source)
            {
               foreach (var sitem in map.SourceItems)
               {
                  if (sitem.DataMapItem.ItemPath == item.DataMapItem.ItemPath)
                  {
                     sitem.Visited = true;
                     return map;
                  }
               }
            }
            else
            {
               foreach (var titem in map.TargetItems)
               {
                  if (titem.DataMapItem.ItemPath == item.DataMapItem.ItemPath)
                  {
                     titem.Visited = true;
                     return map;
                  }
               }
            }
         }
         return null;
      }

      /// <summary>
      /// Order Items...
      /// </summary>
      /// <param name="items">map-items to order</param>
      /// <returns>ordered list of items is returned</returns>
      public static BookMapItemList OrderItems(BookMapItemList items)
      {
         BookMapItemList outList = new BookMapItemList();
         foreach(var map in items)
         {
            foreach (var sitem in map.SourceItems)
            {
               var fitem = Find(items, sitem, MapItemType.Source);
               if (fitem != null && !fitem.Visited)
               {
                  fitem.Visited = true;
                  outList.Add(fitem);
               }
            }
            foreach (var titem in map.TargetItems)
            {
               var fitem = Find(items, titem, MapItemType.Target);
               if (fitem != null && !fitem.Visited)
               {
                  fitem.Visited = true;
                  outList.Add(fitem);
               }
            }
         }

         // add any remaining item not already visited...
         foreach(var map in items)
         {
            if (map.Visited)
            {
               continue;
            }
            outList.Add(map);
         }

         return outList;
      }

   }

}

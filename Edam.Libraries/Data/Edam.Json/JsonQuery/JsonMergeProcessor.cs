using Edam.Data.AssetSchema;
using Edam.Data.Books;
using Edam.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Json.JsonQuery
{

   public class JsonMergeItem
   {
      public MapItemInfo MapItem { get; set; }
      public BookletInfo Booklet { get; set; }
      public BookletCellInfo Cell { get; set; }

      public string ItemPath { get; set; }

      public string ResultText { get; set; }
   }

   public class JsonMergeProcessor
   {
      public List<MapItemInfo> MapItems { get; set; }
      public BookInfo Book { get; set; }

      public List<JsonMergeItem> Items { get; set; } = 
         new List<JsonMergeItem>();

      /// <summary>
      /// Find Map Item by its ID.
      /// </summary>
      /// <param name="itemId">item id to search</param>
      /// <returns>map-item is returned if found else null</returns>
      public MapItemInfo FindMapItem(string itemId)
      {
         var item = MapItems.Find((x) => x.ItemId == itemId);
         return item;
      }

      /// <summary>
      /// Find Booklet by its ID.
      /// </summary>
      /// <param name="itemId">item id to search</param>
      /// <returns>booklet is returned if found else null</returns>
      public BookletInfo FindBooklet(string bookletId)
      {
         var item = Book.Items.Find((x) => x.BookletId == bookletId);
         return item;
      }

      /// <summary>
      /// Process Parsed Results... by matching map-items, booklet and cells...
      /// </summary>
      /// <param name="items">items to process</param>
      public void Process(List<IParserResults> items)
      {
         // catalog all cell & book/booklet references for ease of processing
         foreach (var item in items)
         {
            BookletCellInfo cell = item.Context as BookletCellInfo;
            if (cell != null)
            {

               JsonMergeItem mitem = new JsonMergeItem();
               mitem.MapItem = FindMapItem(cell.ReferenceId);
               mitem.Booklet = FindBooklet(cell.BookletId);
               mitem.Cell = cell;

               Items.Add(mitem);
            }
         }

         // go through all items to be merged...
         foreach(var item in Items)
         {

         }

      }

   }

}

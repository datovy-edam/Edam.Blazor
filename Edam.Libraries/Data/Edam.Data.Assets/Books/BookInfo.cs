using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

// -----------------------------------------------------------------------------
using Edam.Serialization;
using Edam.TextParse;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Newtonsoft.Json;
using Edam.Data.AssetUseCases;

namespace Edam.Data.Books
{

   /// <summary>
   /// A Book details is used to hold information about a collection of 
   /// booklets.
   /// </summary>
   public class BookInfo
   {
      public string BookId { get; set; } = Guid.NewGuid().ToString();
      public string Name { get; set; }
      public string Description { get; set; }

      public List<BookletInfo> Items { get; set; } = new List<BookletInfo>();

      [JsonIgnore]
      public BookletInfo SelectedBooklet { get; set; }

      private NamespaceInfo m_Namespace = null;
      public NamespaceInfo Namespace
      {
         get { return m_Namespace; }
         set
         {
            m_Namespace = value;
         }
      }

      public BookInfo()
      {

      }

      //public BookInfo(NamespaceInfo ns)
      //{
      //   if (ns == null || !ns.IsWellFormedUriString)
      //   {
      //      throw new Exception(
      //         "BookInfo constructure expects a valid namespace");
      //   }
      //   m_Namespace = ns;
      //}

      /// <summary>
      /// Find a Booklet within the book.
      /// </summary>
      /// <param name="bookletId">booklet ID</param>
      /// <returns>if found the booklet info is returned</returns>
      public BookletInfo Find(string bookletId)
      {
         return Items.Find((x) => x.BookletId == bookletId);
      }

      /// <summary>
      /// Delete all Booklets that reference a given Map Item.
      /// </summary>
      /// <param name="item">map item whose references should be deleted</param>
      public void Delete(MapItemInfo item)
      {
         List<BookletInfo> toRemove = new List<BookletInfo>();
         foreach(var blet in Items)
         {
            if (blet.ReferenceId == item.ItemId)
            {
               toRemove.Add(blet);
            }
         }
         foreach(var blet in toRemove)
         {
            Items.Remove(blet);
         }
      }

   }

}

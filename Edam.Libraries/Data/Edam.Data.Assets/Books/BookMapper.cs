using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Data.Books;

namespace Edam.Data.Assets.Books
{

   /// <summary>
   /// Provide needed resources to manage Use Case Mappings, managing its Book
   /// and Booklets
   /// </summary>
   public class BookMapper
   {

      private readonly AssetDataElementList m_Items;
      public AssetDataElementList Items
      {
         get { return m_Items; }
      }

      public NamespaceInfo Namespace { get; set; }
      public string VersionId { get; set; }

      public BookInfo Book { get; set; }

      public BookMapper(
         AssetDataElementList items, NamespaceInfo ns, string versionID)
      {
         m_Items = items;
         Namespace = ns;
         VersionId = versionID;
      }

      /// <summary>
      /// Find element within Items
      /// </summary>
      /// <param name="elementPath">element path to search</param>
      /// <returns>if found, return the element</returns>
      public AssetDataElement FindElement(string elementPath)
      {
         foreach(var item in m_Items)
         {
            if (item.ElementPath == elementPath)
            {
               return item;
            }
         }
         return null;
      }

      /// <summary>
      /// Find Booklet that contains a cell that its reference ID is equal to
      /// itemId.
      /// </summary>
      /// <param name="itemId">item ID</param>
      /// <returns>if found, the BookletInfo instance is returned</returns>
      public BookletInfo FindBooklet(string itemId)
      {
         foreach(var item in Book.Items)
         {
            foreach(var cell in item.Items)
            {
               if (cell.ReferenceId == itemId)
               {
                  return item;
               }
            }
         }
         return null;
      }

   }

}

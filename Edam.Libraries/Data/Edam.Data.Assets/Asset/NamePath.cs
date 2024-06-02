using Edam.Data.AssetSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Asset
{

   /// <summary>
   /// Helper to parse a Name Path item that may be prexifed.
   /// </summary>
   public class NamePathItem
   {

      public const string NAME_ITEM_DELIMETER = ":";

      public string OriginalItem { get; set; }
      public string Prefix { get; set; }
      public string Name { get; set; }

      public NamePathItem()
      {
         OriginalItem = String.Empty;
         Prefix = null;
         Name = String.Empty;
      }

      /// <summary>
      /// Parse a single name path item.
      /// </summary>
      /// <remarks>
      /// It is assumed that an item has the following form: [Prefix]:[Name]
      /// </remarks>
      /// <param name="item">path item</param>
      /// <returns>instance of NamePathItem is returned</returns>
      public static NamePathItem Parse(
         string item, string delimiter = NAME_ITEM_DELIMETER)
      {
         NamePathItem itm = new NamePathItem();
         itm.OriginalItem = item;
         string[] l = item.Split(delimiter);
         if (l.Length == 2)
         {
            itm.Prefix = l[0];
            itm.Name = l[1];
         }
         else
         {
            itm.Prefix = String.Empty;
            itm.Name = l[0];
         }

         return itm;
      }

   }

   /// <summary>
   /// Helper to parse a Name Path separated by given delimiter whose tokens or
   /// items maybe prefixed or not.
   /// </summary>
   public class NamePath
   {
      public const string DELIMITER_DOT = ".";
      public const string DELIMITER_COLUMN = ".";
      public const string NAME_PATH_DELIMETER = "/";

      public string OriginalPath { get; set; }
      public string Delimiter { get; set; } = NamePathItem.NAME_ITEM_DELIMETER;
      public string[] OriginalItems { get; set; }
      public List<NamePathItem> Items { get; set; } =
         new List<NamePathItem>();

      public NamePathItem FirstItem
      {
         get
         {
            return Items.Count > 0 ? Items[0] : new NamePathItem();
         }
      }

      /// <summary>
      /// Parse a name path with or without prefixes into its name path 
      /// components.
      /// </summary>
      /// <param name="path">original path to parse</param>
      /// <param name="pathDelimiter">path delimiter</param>
      /// <param name="itemDelimiter">path item delimiter for prefixes</param>
      /// <returns>instance of a NamePath is returned</returns>
      public static NamePath Parse(
         string path, string pathDelimiter = NAME_PATH_DELIMETER,
         string itemDelimiter = NamePathItem.NAME_ITEM_DELIMETER)
      {
         NamePath npath = new NamePath
         {
            OriginalPath = path,
            OriginalItems = path.Split(NAME_PATH_DELIMETER)
         };
         foreach (var i in npath.OriginalItems)
         {
            npath.Items.Add(NamePathItem.Parse(i, itemDelimiter));
         }
         return npath;
      }

      /// <summary>
      /// Given a delimiter put together a path whose items are separated by
      /// given delimiter.
      /// </summary>
      /// <param name="pathDelimiter">path items delimiter</param>
      /// <param name="itemDelimiter">(optional) item delimiter, if not given
      /// prefixes are not appended</param>
      /// <returns>a full path is returned</returns>
      public string Join(
         string pathDelimiter = DELIMITER_DOT, string itemDelimiter = null)
      {
         string del = String.Empty;
         StringBuilder sb = new StringBuilder();
         foreach(var i in Items)
         {
            sb.Append(del);
            sb.Append(
               (itemDelimiter == null || String.IsNullOrWhiteSpace(i.Prefix) ? 
                  String.Empty : i.Prefix + itemDelimiter) + i.Name);
            del = pathDelimiter;
         }
         return sb.ToString();
      }
   }

}

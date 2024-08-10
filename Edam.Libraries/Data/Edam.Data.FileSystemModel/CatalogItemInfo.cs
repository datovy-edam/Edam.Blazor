using Edam.DataObjects.Trees;
using System.Data.Common;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Edam.Data.FileSystemModel;

public class CatalogItemInfo : TreeItem, ITreeItem
{

   #region -- 1.00 - Fields and Properties...

   public const string TYPE_Catalog = "Catalog";
   public const string TYPE_FILE = "File";

   public TreeItem? CatalogEntry { get; set; } = null;

   public string TypeText
   {
      get { return Type.ToString(); }
      set
      {
         Type = value == TYPE_Catalog ?
            TreeItemType.Branch : TreeItemType.Leaf;
      }
   }

   public new string Title
   {
      get { return CatalogEntry == null ? base.Title : CatalogEntry.Title; }
      set
      {
         if (CatalogEntry != null)
         {
            CatalogEntry.Title = value;
         }
         else
         {
            base.Title = value;
         }
      }
   }

   public bool HasChildren
   {
      get { return Children != null && Children.Count > 0; }
   }

   public HashSet<CatalogItemInfo> Children { get; set; } =
      new HashSet<CatalogItemInfo>();
   public CatalogItemInfo Parent { get; set; }

   #endregion
   #region -- 1.50 - Initialization

   // call CatalogTreeBuilder.CreateItem(FileItemInfo)...

   #endregion
   #region -- 4.00 - Catalog JSON Serialization Support

   /// <summary>
   /// From a root element to 
   /// </summary>
   /// <param name="root"></param>
   /// <returns></returns>
   public String ToCatalogJsonText(CatalogItemInfo? root)
   {
      return TreeItem.ToDirectoryJsonText(root);
   }

   /// <summary>
   /// From a JSON file system info to 
   /// </summary>
   /// <param name="jsonText"></param>
   /// <returns></returns>
   public CatalogItemInfo? FromCatalogJsonText(string jsonText)
   {
      return TreeItem.FromDirectoryJsonText<CatalogItemInfo?>(jsonText);
   }

   #endregion

}

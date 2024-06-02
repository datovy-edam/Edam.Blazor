using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.AssetSchema
{

   public class AssetDataItem
   {
      public AssetDataElement Element { get; set; } = null;
      public AssetDataElementList Children { get; set; } = null;
      public object Tag { get; set; }
   }

   public class AssetDataItemList : List<AssetDataItem>
   {

      /// <summary>
      /// To Data Items (list)
      /// </summary>
      /// <param name="items">list of Asset Data Elements</param>
      /// <returns>instance of AssetDataItemList is returned</returns>
      public static AssetDataItemList ToDataItems(AssetDataElementList items)
      {
         AssetDataItemList list = new AssetDataItemList();
         var types = AssetDataElementList.GetTypes(items);
         foreach (var type in types)
         {
            var item = AssetDataElementList.GetChildren(items, type);
            list.Add(item);
         }
         return list;
      }

   }

}

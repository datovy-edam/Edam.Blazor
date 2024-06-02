using Edam.Data.DataItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.DataItem
{

   public class ItemInstanceTypeInfo : ItemInstanceItemInfo
   {
      public List<ItemInstanceItemInfo> Children { get; set; } =
         new List<ItemInstanceItemInfo>();
   }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.AssetUseCases
{

   public class AssetUseCaseList : List<AssetUseCase>
   {

      public bool HasItems
      {
         get
         {
            return (this.Count > 0 && this[0].Items.Count > 0);
         }
      }

      public bool HasMapItems
      {
         get
         {
            return (this.Count > 0 && this[0].MappedItems.Count > 0);
         }
      }

      public AssetUseCaseList() : base()
      {

      }

   }

}

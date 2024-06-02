using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.AssetConsole
{
   public class AssetArgumentOptions
   {
      /// <summary>
      /// true when JSON should be handeled as JSON LD
      /// </summary>
      public bool JsonLinkDataIndicator { get; set; }

      public AssetArgumentOptions()
      {
         JsonLinkDataIndicator = false;
      }
   }
}

using Edam.Data.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Dictionary
{

   public class DictionaryItemInfo
   {

      public bool Include { get; set; } = false;
      public bool Add { get; set; } = false;
      public bool Remove { get; set; } = false;

      public ITermInfo Entry { get; set; } = null;
      public ITermInfo OriginalEntry { get; set; } = null;
      public DictionaryType DictionaryType { get; set; } = DictionaryType.Term;

   }

}

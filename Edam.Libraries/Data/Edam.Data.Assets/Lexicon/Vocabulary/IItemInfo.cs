using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Lexicon.Vocabulary
{

   public interface IItemInfo
   {
      string LexiconID { get; set; }
      string KeyID { get; set; }
      LexiconItemInfo Lexicon { get; set; }
      string FullPath { get; }
   }

}

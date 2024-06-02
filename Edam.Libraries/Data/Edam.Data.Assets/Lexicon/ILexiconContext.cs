using Edam.Data.Lexicon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Lexicon
{

   public interface ILexiconContext
   {
      LexiconSettingsInfo LexiconSettings { get; set; }
      ILexiconData LexiconData { get; set; }
   }

}

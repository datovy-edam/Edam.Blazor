using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Lexicon
{

   public class LexiconSettingsInfo
   {
      public bool ManageLexicon { get; set; } = true;
      public bool ManageSemantics { get; set; } = true;
      public bool UseSemanticSimilarityFor2 { get; set; } = true;
      public bool UseSemanticSimilarityScanning { get; set; } = false;
   }

}

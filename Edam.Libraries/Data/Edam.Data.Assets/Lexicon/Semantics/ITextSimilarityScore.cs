using Edam.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Lexicon.Semantics
{

   public interface ITextSimilarityScore
   {
      IResultsLog Results { get; set; }
      string? Text1 { get; set; }
      string? Text2 { get; set; }
      int Index { get; set; }
      float Score { get; set; }
      string ScoreText { get; }
   }

}

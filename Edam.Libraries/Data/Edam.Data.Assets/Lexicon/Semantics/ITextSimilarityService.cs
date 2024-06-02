using Edam.Data.Lexicon.Semantics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Lexicon.Semantics
{

   public interface ITextSimilarityService
   {
      ITextSimilarityScore Similarity(string text1, string text2);
      List<ITextSimilarityScore> Similarity(
         List<string> text1, List<string> text2);
   }

}

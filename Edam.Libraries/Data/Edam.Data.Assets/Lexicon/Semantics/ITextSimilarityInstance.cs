using Edam.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Language;

namespace Edam.Data.Lexicon.Semantics
{

   /// <summary>
   /// 
   /// </summary>
   public interface ITextSimilarityInstance: ISemanticsModule
   {
      IResultsLog? ExecuteScript(
         string scriptName, string functionName, Parameters parameters);
   }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Application;
using Edam.Data.Lexicon;
using Edam.Data.AssetSchema;
using Edam.Language;
using Edam.Data.Lexicon.Semantics;

namespace Edam.Data.Lexicon
{

   public class LexiconHelper
   {

      public static ILexiconData GetLexiconDataInstance()
      {
         return AppAssembly.FetchInstance<ILexiconData>(
            AssetResourceHelper.ASSET_LEXICON);
      }

      public static IInterpreter GetInterpreterInstance()
      {
         return AppAssembly.FetchInstance<IInterpreter>(
            AssetResourceHelper.ASSET_PYTHON_LANGUAGE);
      }

      public static ITextSimilarityInstance GetTextSimilarityInstance()
      {
         return AppAssembly.FetchInstance<ITextSimilarityInstance>(
            TextSimilarityService.SEMANTIC_TEXT_SIMILARITY_INSTANCE);
      }

   }

}

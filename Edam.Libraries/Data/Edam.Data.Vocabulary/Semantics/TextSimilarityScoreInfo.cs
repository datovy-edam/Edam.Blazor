using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Diagnostics;
using Edam.Data.Lexicon.Semantics;

namespace Edam.Data.Lexicon.Semantics
{

   public class TextSimilarityScoreInfo : ITextSimilarityScore
   {

      public IResultsLog Results { get; set; }
      public string? Text1 { get; set; } = null;
      public string? Text2 { get; set; } = null;
      public int Index { get; set; } = -1;
      public float Score { get; set; } = 0;
      public string ScoreText
      {
         get { return Score.ToString("0.0000000000"); }
      }

      /// <summary>
      /// Do score has been instanced?
      /// </summary>
      public bool HaveScore
      {
         get { return Score >= 0; }
      }

      /// <summary>
      /// Initialize Text Similarity Score using given dynamic-object.
      /// </summary>
      /// <param name="dynamicObject">object containing evaluation results
      /// </param>
      public TextSimilarityScoreInfo(dynamic? dynamicObject)
      {
         if (dynamicObject == null)
         {
            return;
         }

         Text1 = dynamicObject.text1;
         Text2 = dynamicObject.text2;
         Index = dynamicObject.index;
         Score = dynamicObject.score;
      }

      /// <summary>
      /// Reset Score (HaveScore == false)
      /// </summary>
      public void Reset()
      {
         Index = -1;
         Score = 0;
         Text1 = null;
         Text2 = null;
      }

      /// <summary>
      /// Given a dynamic object, return corresponding Similarity Score.
      /// </summary>
      /// <param name="results">results from similarity evaluation</param>
      /// <returns>instance of score is return or null</returns>
      public static TextSimilarityScoreInfo ToScore(dynamic? results)
      {
         return new TextSimilarityScoreInfo(results);
      }

   }

}

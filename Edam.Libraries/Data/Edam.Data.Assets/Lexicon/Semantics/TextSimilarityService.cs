using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Language;
using Edam.Diagnostics;

namespace Edam.Data.Lexicon.Semantics
{

   /// <summary>
   /// Helper class to get the semantic text similarities.
   /// </summary>
   public class TextSimilarityService : ITextSimilarityService
   {
      public const string SEMANTIC_TEXT_SIMILARITY_INSTANCE = 
         "SEMANTIC_TEXT_SIMILARITY_INSTANCE";

      public const string SCRIPT_SEMANTIC_TEXT_SIMILARITY = 
         "semanticTextSimilarity";
      public const string METHOD_GET_TEXT_SIMILARITY_SCORE = 
         "get_TextSimilarityScore";

      private ITextSimilarityInstance m_Instance;

      public TextSimilarityService()
      {
         m_Instance = LexiconHelper.GetTextSimilarityInstance();
      }

      #region -- 4.00 - text-similarity requests

      /// <summary>
      /// Do a similarity test and return its score.
      /// </summary>
      /// <param name="text1">sentence / text 1</param>
      /// <param name="text2">sentence / text 2</param>
      /// <returns>similarity score is returned</returns>
      public ITextSimilarityScore Similarity(string text1, string text2)
      {
         Parameters parameters = new Parameters();
         parameters.Add("text1", text1);
         parameters.Add("text2", text2);

         var results = m_Instance.ExecuteScript(
            SCRIPT_SEMANTIC_TEXT_SIMILARITY, METHOD_GET_TEXT_SIMILARITY_SCORE,
            parameters);
         ITextSimilarityScore scores = 
            results.ResultValueObject as ITextSimilarityScore;

         return scores;
      }

      /// <summary>
      /// Do a similarity tests and return its score.
      /// </summary>
      /// <param name="text1">sentence / text 1</param>
      /// <param name="text2">sentence / text 2</param>
      /// <returns>list of similarity scores is returned</returns>
      public List<ITextSimilarityScore> Similarity(
         List<string> text1, List<string> text2)
      {
         List<ITextSimilarityScore> l = new List<ITextSimilarityScore>();
         TextSimilarityService textSimilarity = new TextSimilarityService();
         foreach (string t1 in text1)
         {
            foreach (string t2 in text2)
            {
               ITextSimilarityScore result = textSimilarity.Similarity(t1, t2);
               if (result.Results.Success)
               {
                  l.Add(result);
               }
            }
         }
         return l;
      }

      #endregion
      #region -- 4.00 - static versions of text-similarity requests

      /// <summary>
      /// Do a similarity test and return its score.
      /// </summary>
      /// <param name="text1">sentence / text 1</param>
      /// <param name="text2">sentence / text 2</param>
      /// <returns>similarity score is returned</returns>
      public static ITextSimilarityScore GetSimilarityScore(
         string text1, string text2)
      {
         TextSimilarityService textSimilarity = new TextSimilarityService();
         return textSimilarity.Similarity(text1, text2);
      }

      /// <summary>
      /// Do a similarity tests and return its score.
      /// </summary>
      /// <param name="text1">sentence / text 1</param>
      /// <param name="text2">sentence / text 2</param>
      /// <returns>list of similarity scores is returned</returns>
      public static List<ITextSimilarityScore> GetSimilarityScore(
         List<string> text1, List<string> text2)
      {
         List<ITextSimilarityScore> l = new List<ITextSimilarityScore>();
         TextSimilarityService textSimilarity = new TextSimilarityService();
         return textSimilarity.Similarity(text1, text2);
      }

      #endregion

   }

}

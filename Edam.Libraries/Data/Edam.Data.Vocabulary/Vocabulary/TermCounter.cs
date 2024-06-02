using Edam.Diagnostics;
using Edam.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Lexicon.Vocabulary
{

   /// <summary>
   /// Manage Terms Count within the entities and elements.
   /// </summary>
   public class TermCounter
   {

      /// <summary>
      /// Evaluate the possible Token Confidence that it is something
      /// meaningfull.
      /// </summary>
      /// <param name="token">token to evaluate</param>
      /// <returns>confidence number is returned</returns>
      public static decimal TokenConfidence(string token)
      {
         if (string.IsNullOrWhiteSpace(token))
            return 0;
         bool hasNumbers = token.Any(char.IsDigit);
         decimal tokenConfidence = 
            hasNumbers || token.Length < 3? 0.0M :
           (hasNumbers || token.Length <= 3? 0.2M : 0.4M);
         return tokenConfidence;
      }

      /// <summary>
      /// Add Term.
      /// </summary>
      /// <param name="lexiconData">lexicon data</param>
      /// <param name="lexicon">pointer to lexicon</param>
      /// <param name="token">token as a term</param>
      /// <param name="businessDomainID">business domain ID</param>
      /// <param name="itemName">item (entity or element) name</param>
      public static void AddTerm(
         LexiconData lexiconData, LexiconItemInfo lexicon,
         string token, string businessDomainID, string itemName)
      {
         string? tk = token?.Trim();
         if (String.IsNullOrWhiteSpace(tk))
         {
            return;
         }

         var term = lexiconData.FindTerm(tk);
         if (term == null)
         {
            decimal confidence = TokenConfidence(tk);
            term = new TermItemInfo
            {
               LexiconID = lexicon.KeyID,
               Lexicon = lexicon,
               KeyID = Guid.NewGuid().ToString(),
               BusinessDomainID = businessDomainID,
               Category = itemName,
               TermInclude = confidence >= 0.70M,
               Term = tk,
               OriginalTerm = tk,
               Synonyms = tk,
               Confidence = confidence,
               Description = tk,
               Count = 1
            };

            lexiconData.AddTerm(term);
         }
         else
         {
            term.Count++;
         }
      }

      /// <summary>
      /// Go through all entities and element descriptions tokens adding those
      /// that are not found while counting those.
      /// </summary>
      /// <param name="lexiconData">lexicon data</param>
      /// <returns>results log is returned and the lexiconData has been updated
      /// as needed including the list of counted Tokens that will be found in
      /// terms</returns>
      public static void UpdateTermsCount(LexiconData lexiconData)
      {
         var entities = lexiconData.GetEntities();
         foreach (var i in entities)
         {
            if (i.Description == null)
               continue;

            var tokens = i.Description.Split(' ');
            foreach (var token in tokens)
            {
               AddTerm(lexiconData, i.Lexicon, token, 
                  i.BusinessDomainID, i.EntityName);
            }
         }

         var elements = lexiconData.GetElements();
         foreach(var i in elements)
         {
            if (i.Description == null)
               continue;

            var tokens = i.Description.Split(' ');
            foreach(var token in tokens)
            {
               AddTerm(lexiconData, i.Lexicon, token.Trim(),
                  i.BusinessDomainID, i.EntityName);
            }
         }
      }

   }

}

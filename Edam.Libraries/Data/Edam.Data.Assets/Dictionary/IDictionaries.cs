using Edam.Data.Dictionary;
using Edam.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Dictionary
{

   public interface IDictionaries : IDisposable
   {

      ITermInfo GetTerm(string item, bool tryFinding = false, 
         DictionaryType type = DictionaryType.Term);

      ResultsLog<List<ITermInfo>> FindTerm(
         ITermInfo term, DictionaryType type = DictionaryType.Term);
      ResultsLog<List<ITermInfo>> FindTerm(
         string term, DictionaryType type = DictionaryType.Term);
      ResultLog AddTerm(ITermInfo term,
         DictionaryType type = DictionaryType.Term);
      ResultLog RemoveTerm(ITermInfo term,
         DictionaryType type = DictionaryType.Term);

      IResultsLog SaveAllChanges();

   }

}


using Edam.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Application;
using Edam.Data.AssetSchema;
using Edam.Data.Dictionary.Api;
using System.Globalization;

namespace Edam.Data.Dictionary
{

   public class DictionaryHelper
   {

      /// <summary>
      /// Get Dictionary API Instance that was registered as a dependency
      /// ingestion resource.
      /// </summary>
      /// <returns>instance of API is returned if any was found</returns>
      public static IDictionaryApi GetDictionaryApiInstance()
      {
         return AppAssembly.FetchInstance<IDictionaryApi>(
            AssetResourceHelper.ASSET_DICTIONARY_API);
      }

      /// <summary>
      /// Given a sentence get dictionary entries.
      /// </summary>
      /// <param name="sentence">sentence whose tokens</param>
      /// <param name="entries">entries list to add found items</param>
      /// <param name="type">dictionary to look entries in...</param>
      /// <returns></returns>
      public static List<DictionaryItemInfo> GetSentenceTerms(
         string sentence, List<DictionaryItemInfo> entries = null,
         DictionaryType type = DictionaryType.Term)
      {
         List<DictionaryItemInfo> items = 
            entries ?? new List<DictionaryItemInfo>();

         var api = GetDictionaryApiInstance();
         var dic = api.GetDictionaries();

         string[] tokens = sentence.Split(' ');
         foreach (string token in tokens)
         {
            // see if term is already in the entries...
            var oterm = items.Find((x) => x.Entry.Term == token);
            if (oterm != null)
            {
               continue;
            }

            // try to find and define new entry
            var entry = dic.GetTerm(token, tryFinding: true, type: type);
            if (entry != null)
            {

               // does the term is defined? if not try too...
               if (String.IsNullOrWhiteSpace(entry.Definition))
               {
                  var results = api.LookUp(entry);
                  if (results.Success && results.Data != null)
                  {
                     entry.Definition = results.Data.Definition;
                     entry.Category = results.Data.Category;
                     entry.Status = results.Data.Status;
                     entry.UpdatedDate = results.Data.UpdatedDate;
                  }
               }

               // prepare new entry and add it to the list
               var eitem = new DictionaryItemInfo();

               eitem.Entry = entry;
               eitem.OriginalEntry = entry;
               eitem.DictionaryType = type;

               items.Add(eitem);
            }
         }

         return items;
      }

   }

}

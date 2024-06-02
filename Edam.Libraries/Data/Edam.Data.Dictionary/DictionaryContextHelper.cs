using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Dictionary
{

   public class DictionaryContextHelper
   {

      public const string DICTIONARY_CONNECTION_STRING = 
         "DictionaryConnectionString";

      /// <summary>
      /// Get Connection String associated with default application settings key
      /// associated with the Connection String.
      /// </summary>
      /// <returns>returns the connection string</returns>
      public static string GetConnectionString()
      {
         return Application.AppSettings.
            GetConnectionString(DICTIONARY_CONNECTION_STRING);
      }

      /// <summary>
      /// Update/Insert/Delete Queue Entry context info.
      /// </summary>
      /// <param name="context">context</param>
      /// <param name="term">term</param>
      public static void UpsertQueueEntry(
         DictionaryContext context, ITermInfo term, bool delete = false)
      {
         var results = context.WordQueue.
            SingleOrDefault(w => w.KeyID == term.KeyID);

         if (delete)
         {
            WordQueueInfo entry = new WordQueueInfo();
            entry.Copy(term);
            context.WordQueue.Remove(entry);
            return;
         }

         if (results == null)
         {
            WordQueueInfo entry = new WordQueueInfo();
            entry.Copy(term);
            context.WordQueue.Add(entry);
         }
         else
         {
            results.UpdatedDate = DateTimeOffset.Now;
            results.Status = term.Status;
            results.Definition = term.Definition;
            results.Soundex = term.Soundex;
            results.Category = term.Category;
            results.UpdatedDate = DateTimeOffset.Now;
         }
      }

      /// <summary>
      /// Update/Insert/Delete Word Entry context info.
      /// </summary>
      /// <param name="context">context</param>
      /// <param name="term">term</param>
      public static void UpsertWordEntry(
         DictionaryContext context, ITermInfo term, bool delete = false)
      {
         if (delete)
         {
            WordInfo word = new WordInfo();
            word.Copy(term);
            context.Word.Remove(word);
            return;
         }

         var results = context.Word.
            SingleOrDefault(w => w.KeyID == term.KeyID);

         if (results == null)
         {
            WordInfo word = new WordInfo();
            word.Copy(term);
            context.Word.Add(word);
         }
         else
         {
            results.UpdatedDate = DateTimeOffset.Now;
            results.Status = term.Status;
            results.Definition = term.Definition;
            results.Soundex = term.Soundex;
            results.Category = term.Category;
         }
      }

      /// <summary>
      /// Update/Insert Word Entry context info.
      /// </summary>
      /// <param name="context">context</param>
      /// <param name="term">term</param>
      public static void UpsertTermEntry(
         DictionaryContext context, ITermInfo term, bool delete = false)
      {
         if (delete)
         {
            TermInfo word = new TermInfo();
            word.Copy(term);
            context.Term.Remove(word);
            return;
         }

         var results = context.Term.
            SingleOrDefault(w => w.KeyID == term.KeyID);
         if (results == null)
         {
            TermInfo word = new TermInfo();
            word.Copy(term);
            context.Term.Add(word);
         }
         else
         {
            results.UpdatedDate = DateTimeOffset.Now;
            results.Status = term.Status;
            results.Definition = term.Definition;
            results.Soundex = term.Soundex;
            results.Category = term.Category;
         }
      }

   }

}

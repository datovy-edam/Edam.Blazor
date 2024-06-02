using Edam.Data.Assets.Dictionary;
using Edam.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;

namespace Edam.Data.Dictionary
{

   public class DictionaryContext : DbContext, IDictionaries
   {

      public DbSet<WordInfo> Word { get; set; }
      public DbSet<TermInfo> Term { get; set; }
      public DbSet<WordQueueInfo> WordQueue { get; set; }

      public string ConnectionString { get; }

      public DictionaryContext()
      {
         ConnectionString = DictionaryContextHelper.GetConnectionString();
      }

      public DictionaryContext(string connectionString)
      {
         ConnectionString = connectionString;
      }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         // Configure default schema
         modelBuilder.HasDefaultSchema("Dictionary");
      }

      // Configures EF to create an SQL database using given connection string
      protected override void OnConfiguring(DbContextOptionsBuilder options)
          => options.UseSqlServer(ConnectionString);

      #region -- 4.00 - IDictionary implementation

      /// <summary>
      /// Add Term to dictionary.
      /// </summary>
      /// <remarks>Remember to call SaveChanges when done adding entries
      /// </remarks>
      /// <param name="term">term info to add</param>
      /// <returns>results log is returned</returns>
      public ResultLog AddTerm(ITermInfo term,
         DictionaryType type = DictionaryType.Term)
      {
         ResultsLog<ITermInfo?> results = new ResultsLog<ITermInfo?>();
         switch(type)
         {
            case DictionaryType.Term:
               {
                  TermInfo nterm = new TermInfo();
                  nterm.Copy(term);
                  Term.Add(nterm);
               }
               break;
            case DictionaryType.Word:
               {
                  WordInfo nterm = new WordInfo();
                  nterm.Copy(term);
                  Word.Add(nterm);
               }
               break;
            case DictionaryType.Queue:
               {
                  WordQueueInfo nterm = new WordQueueInfo();
                  nterm.Copy(term);
                  WordQueue.Add(nterm);
               }
               break;
            default:
               {
                  results.Failed(EventCode.ReferenceNotFound);
               }
               break;
         }
         results.Succeeded();
         return results;
      }

      /// <summary>
      /// Find Terms...
      /// </summary>
      /// <param name="term">term details to find</param>
      /// <returns>return list of found items</returns>
      private IQueryable<TermInfo> FindTermItem(ITermInfo term)
      {
         var items = from t in Term
                     where t.Term == term.Term
                     select t;
         return items;
      }

      /// <summary>
      /// Find Words...
      /// </summary>
      /// <param name="term">word item details to find</param>
      /// <returns>return list of found items</returns>
      private IQueryable<WordInfo> FindWordItem(ITermInfo term)
      {
         var items = from t in Word
                     where t.Term == term.Term
                     select t;
         return items;
      }

      /// <summary>
      /// Find Queue items...
      /// </summary>
      /// <param name="term">queue item details to find</param>
      /// <returns>return list of found items</returns>
      private IQueryable<WordQueueInfo> FindQueueItem(ITermInfo term)
      {
         var items = from t in WordQueue
                     where t.Term == term.Term
                     select t;
         return items;
      }

      /// <summary>
      /// Get Term from dictionary.
      /// </summary>
      /// <param name="term">term to find</param>
      /// <returns>results log is returned</returns>
      public ResultsLog<List<ITermInfo>> FindTerm(ITermInfo term,
         DictionaryType type = DictionaryType.Term)
      {
         ResultsLog<List<ITermInfo>> results = 
            new ResultsLog<List<ITermInfo>>();

         switch(type)
         {
            case DictionaryType.Term:
               var items = FindTermItem(term);
               results.Data = items.ToList<ITermInfo>();
               results.Succeeded();
               break;
            case DictionaryType.Word:
               var words = FindWordItem(term);
               results.Data = words.ToList<ITermInfo>();
               results.Succeeded();
               break;
            case DictionaryType.Queue:
               var queueItems = FindQueueItem(term);
               results.Data = queueItems.ToList<ITermInfo>();
               results.Succeeded();
               break;
            default:
               results.Failed(EventCode.ReferenceNotFound);
               break;
         }

         return results;
      }

      /// <summary>
      /// Find Term given as a string.
      /// </summary>
      /// <param name="term">string / term to find</param>
      /// <param name="type">dictionary type</param>
      /// <returns>returns a list of found matching terms</returns>
      public ResultsLog<List<ITermInfo>> FindTerm(
         string term, DictionaryType type = DictionaryType.Term)
      {
         TermInfo termi = new TermInfo();
         termi.Term = term;
         return FindTerm(termi, type);
      }

      /// <summary>
      /// Get Term from dictionary.
      /// </summary>
      /// <param name="term">term to find</param>
      /// <param name="tryFinding">true if term should be looked-up</param>
      /// <param name="type">dictionary in where to look-it-up</param>
      /// <returns>results log is returned</returns>
      public ITermInfo GetTerm(string term, bool tryFinding = false, 
         DictionaryType type = DictionaryType.Term)
      {
         ITermInfo item = new TermInfo();

         item.LexiconID = String.Empty;
         item.Definition = String.Empty;
         item.Term = term;
         item.OriginalTerm = term;
         item.Status = EntryStatus.Unknown;
         item.Confidence = 0;
         item.KeyID = String.Empty;
         item.UpdatedDate = DateTimeOffset.Now;
         item.CreatedDate = DateTimeOffset.Now;
         item.Soundex = Edam.Strings.TextString.Soundex(term);

         if (tryFinding)
         {
            var results = FindTerm(item, type);
            if (results.Success && results.Data.Count > 0)
            {
               item = results.Data[0];
            }
         }

         return item;
      }

      /// <summary>
      /// Remove Term.
      /// </summary>
      /// <param name="term">term to remove</param>
      /// <returns></returns>
      private ResultLog RemoveTermItem(ITermInfo term)
      {
         ResultsLog<ITermInfo?> results = new ResultsLog<ITermInfo?>();
         var items = FindTermItem(term);
         if (items != null)
         {
            Term.RemoveRange(items);
         }
         return results;
      }

      /// <summary>
      /// Remove Word.
      /// </summary>
      /// <param name="term">term to remove</param>
      /// <returns></returns>
      private ResultLog RemoveWordItem(ITermInfo term)
      {
         ResultsLog<ITermInfo?> results = new ResultsLog<ITermInfo?>();
         var items = FindWordItem(term);
         if (items != null)
         {
            Word.RemoveRange(items);
         }
         return results;
      }

      /// <summary>
      /// Remove Queue Item.
      /// </summary>
      /// <param name="term">term to remove</param>
      /// <returns></returns>
      private ResultLog RemoveQueueItem(ITermInfo term)
      {
         ResultLog results = new ResultLog();
         var items = FindQueueItem(term);
         if (items != null)
         {
            WordQueue.RemoveRange(items);
         }
         return results;
      }

      /// <summary>
      /// Remove Term from dictionary.
      /// </summary>
      /// <param name="term">term to remove</param>
      /// <returns>results log is returned</returns>
      public ResultLog RemoveTerm(ITermInfo term,
         DictionaryType type = DictionaryType.Term)
      {
         ResultLog results = null;
         switch(type)
         {
            case DictionaryType.Term:
               results = RemoveTermItem(term);
               break;
            case DictionaryType.Word:
               results = RemoveWordItem(term);
               break;
            case DictionaryType.Queue:
               results = RemoveQueueItem(term);
               break;
            default:
               results = new ResultLog();
               results.Failed(EventCode.ReferenceNotFound);
               break;
         }
         return results;
      }

      /// <summary>
      /// Save all dictionary changes...
      /// </summary>
      /// <returns></returns>
      public IResultsLog SaveAllChanges()
      {
         ResultLog results = new ResultLog();
         try
         {
            SaveChanges();
            results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         return results;
      }

      #endregion

   }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Data.Dictionary;

namespace Edam.Data.Lexicon.Vocabulary
{

   public class TermItemInfo : IItemInfo, ITermBaseInfo
   {

      [MaxLength(128)]
      public string LexiconID { get; set; }

      [Key, MaxLength(40)]
      public string KeyID { get; set; }
      [MaxLength(128)]
      public string? BusinessDomainID { get; set; }
      [MaxLength(128)]
      public string? Category { get; set; }
      [MaxLength(128)]
      public string? Tag { get; set; }
      public bool TermInclude { get; set; }
      [MaxLength(128)]
      public string? Term { get; set; }
      [MaxLength(128)]
      public string? OriginalTerm { get; set; }
      public string? Synonyms { get; set; }
      public decimal? Confidence { get; set; }
      public string? Description { get; set; }
      public int? Count { get; set; }

      public LexiconItemInfo Lexicon { get; set; }

      private string? _fullPath = null;
      public string? FullPath
      {
         get
         {
            return _fullPath;
         }
      }

      [NotMapped]
      public string Soundex { get; set; }

      [NotMapped]
      public string? Definition
      {
         get { return Description; }
         set { Description = value; }
      }

      [NotMapped]
      public EntryStatus? Status { get; set; }

      [NotMapped]
      public string StatusText
      {
         get { return Status.ToString(); }
      }

      /// <summary>
      /// Copy given term info into the Term Item.
      /// </summary>
      /// <param name="term">term (base) to copy info from</param>
      public void Copy(ITermBaseInfo term)
      {
         LexiconID = term.LexiconID;
         KeyID = term.KeyID;
         Category = term.Category;
         Term = term.Term;
         OriginalTerm = term.OriginalTerm;
         Soundex = term.Soundex;
         Confidence = term.Confidence;
         Definition = term.Definition;
         Status = term.Status;
      }

      public string ResetFullPath()
      {
         return _fullPath = BusinessDomainID + "/" + Category + 
            "/" + Tag + "/" + Term;
      }

   }

}



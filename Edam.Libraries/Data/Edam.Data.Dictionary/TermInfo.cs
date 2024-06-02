using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Dictionary
{

   [Table("Term")]
   public class TermInfo : ITermInfo
   {

      [MaxLength(128)]
      public string LexiconID { get; set; }

      [Key, MaxLength(40)]
      public string KeyID { get; set; }
      [MaxLength(128)]
      public string Category { get; set; }
      [MaxLength(128)]
      public string Term { get; set; }
      [MaxLength(128)]
      public string OriginalTerm { get; set; }
      [MaxLength(4)]
      public string Soundex { get; set; }
      public decimal? Confidence { get; set; }
      public string Definition { get; set; }

      public EntryStatus? Status { get; set; } = EntryStatus.Unknown;

      public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
      public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

      [NotMapped]
      public string StatusText
      {
         get { return Status.ToString(); }
      }

      public void Copy(ITermBaseInfo term)
      {
         LexiconID = term.LexiconID;
         KeyID = term.KeyID;
         Category = term.Category;
         Term = term.Term;
         OriginalTerm = term.OriginalTerm;
         Confidence = term.Confidence;
         Definition = term.Definition;
         Status = term.Status;
         //CreatedDate = term.CreatedDate;
         //UpdatedDate = term.UpdatedDate;
      }

   }

   [Table("Word")]
   public class WordInfo : ITermInfo
   {

      [MaxLength(128)]
      public string LexiconID { get; set; }

      [Key, MaxLength(40)]
      public string KeyID { get; set; }
      [MaxLength(128)]
      public string Category { get; set; }
      [MaxLength(128)]
      public string Term { get; set; }
      [MaxLength(128)]
      public string OriginalTerm { get; set; }
      [MaxLength(4)]
      public string Soundex { get; set; }
      public decimal? Confidence { get; set; }
      public string Definition { get; set; }

      public EntryStatus? Status { get; set; } = EntryStatus.Unknown;

      public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
      public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

      [NotMapped]
      public string StatusText
      {
         get { return Status.ToString(); }
      }

      public void Copy(ITermBaseInfo term)
      {
         LexiconID = term.LexiconID;
         KeyID = term.KeyID;
         Category = term.Category;
         Term = term.Term;
         OriginalTerm = term.OriginalTerm;
         Confidence = term.Confidence;
         Definition = term.Definition;
         Status = term.Status;
         //CreatedDate = term.CreatedDate;
         //UpdatedDate = term.UpdatedDate;
      }

   }

   [Table("WordQueue")]
   public class WordQueueInfo : ITermInfo
   {

      [MaxLength(128)]
      public string LexiconID { get; set; }

      [Key, MaxLength(40)]
      public string KeyID { get; set; }
      [MaxLength(128)]
      public string Category { get; set; }
      [MaxLength(128)]
      public string Term { get; set; }
      [MaxLength(128)]
      public string OriginalTerm { get; set; }
      [MaxLength(4)]
      public string Soundex { get; set; }
      public decimal? Confidence { get; set; }
      public string Definition { get; set; }

      public EntryStatus? Status { get; set; } = EntryStatus.Unknown;

      public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
      public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

      [NotMapped]
      public string StatusText
      {
         get { return Status.ToString(); }
      }

      public void Copy(ITermBaseInfo term)
      {
         LexiconID = term.LexiconID;
         KeyID = term.KeyID;
         Category = term.Category;
         Term = term.Term;
         OriginalTerm = term.OriginalTerm;
         Confidence = term.Confidence;
         Definition = term.Definition;
         Status = term.Status;
         //CreatedDate = term.CreatedDate;
         //UpdatedDate = term.UpdatedDate;
      }

   }

}


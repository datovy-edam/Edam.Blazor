using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Dictionary
{

   public interface ITermBaseInfo
   {
      string LexiconID { get; set; }
      string KeyID { get; set; }
      string Category { get; set; }
      string Term { get; set; }
      string OriginalTerm { get; set; }
      string Soundex { get; set; }
      decimal? Confidence { get; set; }
      string Definition { get; set; }

      EntryStatus? Status { get; set; }
      string StatusText { get; }

      void Copy(ITermBaseInfo term);
   }

   public interface ITermInfo : ITermBaseInfo
   {

      DateTimeOffset CreatedDate { get; set; }
      DateTimeOffset UpdatedDate { get; set; }

   }

}

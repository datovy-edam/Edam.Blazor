using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Lexicon.Vocabulary
{

   public class TagItemInfo : IItemInfo
   {

      [MaxLength(128)]
      public string LexiconID { get; set; }

      [Key, MaxLength(40)]
      public string KeyID { get; set; }
      [MaxLength(128)]
      public string? ScopeID { get; set; }
      [MaxLength(128)]
      public string? TagID { get; set; }
      [MaxLength(128)]
      public string? Name { get; set; }
      public string? Notes { get; set; }

      public LexiconItemInfo Lexicon { get; set; }

      private string? _fullPath = null;
      public string? FullPath
      {
         get
         {
            return _fullPath;
         }
      }

      public string ResetFullPath()
      {
         return _fullPath = ScopeID + "/" + TagID;
      }

   }

}



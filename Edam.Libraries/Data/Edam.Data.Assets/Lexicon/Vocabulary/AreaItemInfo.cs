using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Edam.Data.Lexicon.Vocabulary
{

   public partial class AreaItemInfo : IItemInfo
   {

      [MaxLength(128)]
      public string LexiconID { get; set; }

      [Key, MaxLength(40)]
      public string KeyID { get; set; }
      [MaxLength(128)]
      public string? BusinessDomainID { get; set; }
      public bool AreaInclude { get; set; }
      [MaxLength(128)]
      public string? AreaName { get; set; }
      [MaxLength(128)]
      public string? OriginalAreaName { get; set; }
      public string? Synonyms { get; set; }
      public string? Aliases { get; set; }
      [MaxLength(128)]
      public string? Base { get; set; }
      public string? Description { get; set; }
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
         return _fullPath =
             BusinessDomainID + "/" + AreaName;
      }

   }

}

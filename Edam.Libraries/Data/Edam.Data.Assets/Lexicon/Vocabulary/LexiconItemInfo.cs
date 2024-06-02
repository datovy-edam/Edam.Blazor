using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Edam.Data.Lexicon.Vocabulary
{

   public class LexiconItemInfo
   {

      [Key, MaxLength(128)]
      public string KeyID { get; set; }
      [MaxLength(128)]
      public string? Title { get; set; }
      public string? Uri { get; set; }

      public string? Synonyms { get; set; }
      public string? Aliases { get; set; }
      public string? Labels { get; set; }
      public string? Description { get; set; }
      public string? Notes { get; set; }

      public List<AreaItemInfo> Areas { get; set; }
      public List<EntityItemInfo> Entities { get; set; }
      public List<ElementItemInfo> Elements { get; set; }
      public List<RelationshipItemInfo> Relationships { get; set; }
      public List<TagItemInfo> Tags { get; set; }
      public List<MetadataItemInfo> Metadata { get; set; }
      public List<TermItemInfo> Terms { get; set; }
      public List<UriItemInfo> Uris { get; set; }

   }

}

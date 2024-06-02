using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Lexicon.Vocabulary
{

   public class RelationshipItemInfo : IItemInfo
   {

      [MaxLength(128)]
      public string LexiconID { get; set; }

      [Key, MaxLength(40)]
      public string KeyID { get; set; }
      [MaxLength(128)]
      public string? RelationshipID { get; set; }
      [MaxLength(128)]
      public string? BusinessDomainID { get; set; }
      [MaxLength(128)]
      public string? BusinessAreaID { get; set; }
      [MaxLength(128)]
      public string? EntityName { get; set; }
      [MaxLength(128)]
      public string? ElementName { get; set; }
      [MaxLength(128)]
      public string? ReferenceDomainID { get; set; }
      [MaxLength(128)]
      public string? ReferenceAreaID { get; set; }
      [MaxLength(40)]
      public string? ReferenceType { get; set; }
      [MaxLength(128)]
      public string? ReferenceEntityName { get; set; }
      [MaxLength(128)]
      public string? ReferenceElementName { get; set; }
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
         if (String.IsNullOrEmpty(RelationshipID))
         {
            RelationshipID = EntityName + "." + ElementName;
         }
         return _fullPath =
             BusinessDomainID + "/" + BusinessAreaID + "/" + RelationshipID;
      }

   }

}


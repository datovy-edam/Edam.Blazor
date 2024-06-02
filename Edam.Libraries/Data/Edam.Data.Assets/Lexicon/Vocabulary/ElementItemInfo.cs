using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Lexicon.Vocabulary
{

   public class ElementItemInfo : IItemInfo
   {

      [MaxLength(128)]
      public string LexiconID { get; set; }

      [Key, MaxLength(40)]
      public string KeyID { get; set; }
      [MaxLength(128)]
      public string? BusinessDomainID { get; set; }
      [MaxLength(128)]
      public string? BusinessAreaID { get; set; }
      [MaxLength(128)]
      public string? EntityName { get; set; }
      public bool ElementInclude { get; set; }
      [MaxLength(128)]
      public string? ElementName { get; set; }
      [MaxLength(128)]
      public string? OriginalElementName { get; set; }
      public string? Synonyms { get; set; }
      public string? Aliases { get; set; }
      public string? Tags { get; set; }
      public decimal? Confidence { get; set; }
      public string? Description { get; set; }
      public string? Notes { get; set; }

      public string? MetadataBag { get; set; }

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
             BusinessDomainID + "/" + BusinessAreaID + "/" + EntityName + "/" +
             ElementName;
      }

      /// <summary>
      /// Review data elements and enrich or enhance those as needed.
      /// </summary>
      public void Introspect()
      {
         if (String.IsNullOrWhiteSpace(_fullPath))
         {
            ResetFullPath();
         }
         if (String.IsNullOrWhiteSpace(OriginalElementName))
         {
            OriginalElementName = ElementName;
         }
         if (String.IsNullOrWhiteSpace(Description))
         {
            Description = Edam.Text.Convert.ToProperCase(ElementName);
         }
      }

   }

}



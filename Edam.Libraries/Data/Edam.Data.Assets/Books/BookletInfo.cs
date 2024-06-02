using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

// -----------------------------------------------------------------------------
using Edam.Serialization;
using Edam.TextParse;
using Edam.Data.Asset;
using Newtonsoft.Json;

namespace Edam.Data.Books
{

   /// <summary>
   /// A Boolket is used to manage information about a collection of cells.
   /// </summary>
   /// <remarks>
   /// A booklet is defined by a collection of cells whose purpose is to
   /// document, define, or describe a mapping, transformation or an element.
   /// Code cells can be executed based on the underlying language.
   /// </remarks>
   public class BookletInfo
   {
      public string BookletId { get; set; } = Guid.NewGuid().ToString();
      public string Name { get; set; }
      public string Description { get; set; }

      /// <summary>
      /// Identify the related reference-object with its ID whose value may be
      /// a GUID or other unique identifier.
      /// </summary>
      public string ReferenceId { get; set; }

      public List<BookletCellInfo> Items { get; set; } =
         new List<BookletCellInfo>();

      [JsonIgnore]
      public BookletCellInfo SelectedCell { get; set; }

   }

}
